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

// AddControllers will automatically discover ErrorsController and DevelopmentController (in Debug)
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});

// Configure common services via extension methods
builder
    .Services.AddTelemetry(builder)
    .AddSecurity(configuration)
    .AddRateLimitingConfig(configuration)
    .AddHealthCheckConfig(configuration)
    .AddValidation()
    .AddCorsPolicy(configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfigured();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Basic UI setup
    app.UseDeveloperExceptionPage();
}
else
{
    // UseExceptionHandler routes errors to the ErrorsController.HandleError endpoint
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseSecurityHeaders();
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// Map controllers and apply rate limiting policy
app.MapControllers().RequireRateLimiting("fixed-by-user");

app.MapHealthChecks("/healthz");

// Minimal API endpoints previously here are now handled by controllers

app.Run();
