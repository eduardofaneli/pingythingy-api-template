using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PingyThingy.Api.Validation;

namespace PingyThingy.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<WebhookPayloadValidator>(); // Scans and registers validators
        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Read allowed origins from configuration, fallback to a default dev origin
        var allowedOrigins =
            configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:3000"]; // Default for local frontend dev

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
                    // Consider .AllowCredentials() if you need cookies/auth headers from browser JS
                }
            );
        });
        return services;
    }

    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        // Configure OpenTelemetry
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName: builder.Environment.ApplicationName)
            )
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter()
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter()
            );

        return services;
    }

    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration // Inject IConfiguration
    )
    {
        // --- Add Authentication ---
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "YOUR_ISSUER",
                    ValidAudience = configuration["Jwt:Audience"] ?? "YOUR_AUDIENCE",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Jwt:Key"] ?? "YOUR_SUPER_SECRET_KEY_REPLACE_ME"
                        )
                    ),
                };
            });

        // --- Add Authorization ---
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddRateLimitingConfig(
        this IServiceCollection services,
        IConfiguration configuration // Inject IConfiguration
    )
    {
        // Read configuration values with defaults
        var fixedByUserOptions = configuration.GetSection("RateLimiting:FixedByUser");
        var globalOptions = configuration.GetSection("RateLimiting:Global");

        var fixedPermitLimit = fixedByUserOptions.GetValue<int?>("PermitLimit") ?? 10;
        var fixedWindowSeconds = fixedByUserOptions.GetValue<int?>("WindowSeconds") ?? 10;
        var fixedQueueLimit = fixedByUserOptions.GetValue<int?>("QueueLimit") ?? 0;

        var globalPermitLimit = globalOptions.GetValue<int?>("PermitLimit") ?? 100;
        var globalWindowMinutes = globalOptions.GetValue<int?>("WindowMinutes") ?? 1;
        var globalQueueLimit = globalOptions.GetValue<int?>("QueueLimit") ?? 0;

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(
                "fixed-by-user",
                httpContext =>
                {
                    var userClaim = httpContext.User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier
                    );
                    var partitionKey = userClaim?.Value ?? httpContext.Request.Host.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = fixedPermitLimit, // Use configured value
                            Window = TimeSpan.FromSeconds(fixedWindowSeconds), // Use configured value
                            QueueLimit = fixedQueueLimit, // Use configured value
                        }
                    );
                }
            );

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: "global",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = globalPermitLimit, // Use configured value
                            Window = TimeSpan.FromMinutes(globalWindowMinutes), // Use configured value
                            QueueLimit = globalQueueLimit, // Use configured value
                        }
                    )
            );
        });

        return services;
    }

    public static IServiceCollection AddHealthCheckConfig(
        this IServiceCollection services,
        IConfiguration configuration // Inject IConfiguration
    )
    {
        services.AddHealthChecks();
        // Example using configuration:
        // .AddNpgSql(configuration.GetConnectionString("DefaultConnection"), name: "database")
        // .AddUrlGroup(new Uri(configuration["ExternalServices:ApiHealthUrl"]), name: "external-api");
        return services;
    }

    // New method for configuring SwaggerGen
    public static IServiceCollection AddSwaggerGenConfigured(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Define the Swagger document
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "PingyThingy API", Version = "v1" });

            // Define the BearerAuth security scheme
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // Lowercase 'bearer'
                    BearerFormat = "JWT",
                }
            );

            // Make sure Swagger UI requires a Bearer token
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer", // Must match the id defined in AddSecurityDefinition
                            },
                        },
                        Array.Empty<string>() // No specific scopes required for Bearer
                    },
                }
            );

            // Optional: Configure Swagger to potentially separate Development endpoints
            // options.SwaggerDoc("dev", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PingyThingy API - Development", Version = "dev" });
        });

        return services;
    }
}
