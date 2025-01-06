using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApplication1.Core;

public static class DependencyInjection
{
    public static void AppMiddleWare(this IApplicationBuilder app)
    {
        app.UseCors("AllowSpecificOrigin"); // Specify the CORS policy
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void AppInjection(this IServiceCollection services, IConfiguration configuration)
    {
        _assemblyInjection(services, "Service");
        _assemblyInjection(services, "SingletonService");
        _assemblyInjection(services, "Repository");
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddControllers();
        services.AppSwagger();
        services.AddApiVersioning();
        // Set up JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AppEnvironment.JwtIssuer, // Use AppEnvironment for issuer
                    ValidAudience = AppEnvironment.JwtAudience, // Use AppEnvironment for audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppEnvironment.JwtKey)) // Use AppEnvironment for key
                };
            });

        // Add authorization policies (example)
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
        });

        // Add logging
        services.AddLogging();
    }

    private static void _assemblyInjection(IServiceCollection service, string subFix)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(a => a.Name.EndsWith(subFix) && a is { IsAbstract: false, IsInterface: false })
            .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            .ToList()
            .ForEach(typesToRegister =>
            {
                if (subFix.Contains("Singleton"))
                {
                    typesToRegister.serviceTypes.ForEach(typeToRegister =>
                        service.AddSingleton(typeToRegister, typesToRegister.assignedType));
                }
                else
                {
                    typesToRegister.serviceTypes.ForEach(typeToRegister =>
                        service.AddScoped(typeToRegister, typesToRegister.assignedType));
                }
            });
    }
}
