using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TheOfficeAPI.Level0.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0", new OpenApiInfo
                {
                    Title = "The Office API - Level 0",
                    Version = "v0",
                    Description = "Richardson Maturity Model Level 0 implementation"
                });
                
                c.EnableAnnotations(); // This is crucial for SwaggerSchema to work
                
                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void UseSwaggerMiddleware(this WebApplication app)
        {
            // Enable Swagger in all environments for documentation generation
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v0/swagger.json", "The Office API V0");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            });
        }
    }
}
