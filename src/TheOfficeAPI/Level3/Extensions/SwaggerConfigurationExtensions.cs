using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TheOfficeAPI.Level3.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Office API - Level 3 (HATEOAS)",
                    Version = "v1",
                    Description = "Richardson Maturity Model Level 3 implementation - HATEOAS (Hypermedia as the Engine of Application State). " +
                                  "Responses include hypermedia links for resource discovery and navigation."
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
