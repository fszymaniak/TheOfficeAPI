using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TheOfficeAPI.Level2.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Office API - Level 2",
                    Version = "v1",
                    Description = "Richardson Maturity Model Level 2 implementation - Introduces HTTP verbs and proper status codes"
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Office API V1");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            });
        }
    }
}
