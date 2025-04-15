using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using PingyThingy.Api;
using PingyThingy.Api.Filters;
using PingyThingy.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// AddControllers will automatically discover ErrorsController and DevelopmentController (in Debug)
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});

// Configure Telemetry, Security, Rate Limiting, Health Checks, Validation, and CORS
builder
    .Services.AddTelemetry(builder)
    .AddSecurity(configuration)
    .AddRateLimitingConfig(configuration)
    .AddHealthCheckConfig(configuration)
    .AddValidation()
    .AddCorsPolicy(configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfigured();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Optional: Configure Swagger UI if using separate docs
        // options.SwaggerEndpoint("/swagger/v1/swagger.json", "PingyThingy API v1");
        // options.SwaggerEndpoint("/swagger/dev/swagger.json", "Development");
    });
    app.UseDeveloperExceptionPage();
}
else
{
    // UseExceptionHandler will now route to ErrorsController.HandleError
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseSecurityHeaders();
app.UseHttpsRedirection();

// --- Configure Middleware Pipeline ---
app.UseRouting();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// Map Controllers will map ErrorsController and DevelopmentController routes
app.MapControllers().RequireRateLimiting("fixed-by-user");

app.MapHealthChecks("/healthz");

// --- REMOVE Minimal API Endpoints ---
// The /error endpoint is removed (handled by ErrorsController)
// The /dev/generate-token endpoint is removed (handled by DevelopmentController in Debug builds)

app.Run();
