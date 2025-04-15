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
        services.AddValidatorsFromAssemblyContaining<WebhookPayloadValidator>(); // Scans assembly for FluentValidation validators
        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Reads allowed origins from "Cors:AllowedOrigins" in appsettings.json
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
                    // Consider .AllowCredentials() if frontend needs to send cookies/auth headers
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
        // Configures OpenTelemetry for tracing and metrics, exporting via OTLP
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(serviceName: builder.Environment.ApplicationName)
            )
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter() // Exports to endpoint defined by OTEL_EXPORTER_OTLP_ENDPOINT env var
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter() // Exports to endpoint defined by OTEL_EXPORTER_OTLP_ENDPOINT env var
            );

        return services;
    }

    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
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
                    // Values read from "Jwt:*" section in appsettings.json or environment variables
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "YOUR_ISSUER",
                    ValidAudience = configuration["Jwt:Audience"] ?? "YOUR_AUDIENCE",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            // IMPORTANT: Use User Secrets for Jwt:Key in Development!
                            configuration["Jwt:Key"] ?? "YOUR_SUPER_SECRET_KEY_REPLACE_ME"
                        )
                    ),
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddRateLimitingConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Reads configuration from "RateLimiting:*" sections in appsettings.json
        var fixedByUserOptions = configuration.GetSection("RateLimiting:FixedByUser");
        var globalOptions = configuration.GetSection("RateLimiting:Global");

        // Fallback defaults if configuration is missing
        var fixedPermitLimit = fixedByUserOptions.GetValue<int?>("PermitLimit") ?? 10;
        var fixedWindowSeconds = fixedByUserOptions.GetValue<int?>("WindowSeconds") ?? 10;
        var fixedQueueLimit = fixedByUserOptions.GetValue<int?>("QueueLimit") ?? 0;

        var globalPermitLimit = globalOptions.GetValue<int?>("PermitLimit") ?? 100;
        var globalWindowMinutes = globalOptions.GetValue<int?>("WindowMinutes") ?? 1;
        var globalQueueLimit = globalOptions.GetValue<int?>("QueueLimit") ?? 0;

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Policy per user (based on NameIdentifier claim) or host as fallback
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
                            PermitLimit = fixedPermitLimit,
                            Window = TimeSpan.FromSeconds(fixedWindowSeconds),
                            QueueLimit = fixedQueueLimit,
                        }
                    );
                }
            );

            // Global limiter applied to all requests
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: "global",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = globalPermitLimit,
                            Window = TimeSpan.FromMinutes(globalWindowMinutes),
                            QueueLimit = globalQueueLimit,
                        }
                    )
            );
        });

        return services;
    }

    public static IServiceCollection AddHealthCheckConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHealthChecks();
        // Add specific health checks for dependencies (database, external APIs, etc.) here
        // Example: .AddNpgSql(configuration.GetConnectionString("DefaultConnection"), name: "database")
        return services;
    }

    // Configures SwaggerGen with Bearer Token authentication support
    public static IServiceCollection AddSwaggerGenConfigured(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Define the main Swagger document (Title is replaced by template engine)
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "PingyThingy API", Version = "v1" });

            // Define the BearerAuth security scheme for JWT
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // Standard is lowercase
                    BearerFormat = "JWT",
                }
            );

            // Make Swagger UI require a Bearer token globally
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer", // Matches the AddSecurityDefinition ID
                            },
                        },
                        Array.Empty<string>() // No specific OAuth2 scopes needed
                    },
                }
            );

            // Optional: Add a separate document for development-specific endpoints
            // options.SwaggerDoc("dev", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PingyThingy API - Development", Version = "dev" });
        });

        return services;
    }
}
