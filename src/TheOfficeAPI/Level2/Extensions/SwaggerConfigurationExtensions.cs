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
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "The Office API - Level 2",
                    Version = "v2",
                    Description = "Richardson Maturity Model Level 2 implementation - Introduces HTTP verbs and proper status codes"
                });

                c.EnableAnnotations(); // This is crucial for SwaggerSchema to work

                // Only include Level 2 controllers in Swagger documentation
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor == null) return false;

                    var controllerNamespace = controllerActionDescriptor.ControllerTypeInfo.Namespace ?? string.Empty;
                    return controllerNamespace.StartsWith("TheOfficeAPI.Level2");
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        public static void UseSwaggerMiddleware(this WebApplication app)
        {
            // Enable Swagger in all environments for documentation generation
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "The Office API V2");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            });
        }
    }
}
