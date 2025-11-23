using System.Reflection;
using Microsoft.OpenApi.Models;

namespace TheOfficeAPI.Level1.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Office API - Level 1",
                    Version = "v1",
                    Description = "Richardson Maturity Model Level 1 implementation - Introduces resource-based URIs"
                });

                c.EnableAnnotations(); // This is crucial for SwaggerSchema to work

                // Only include Level 1 controllers in Swagger documentation
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor == null) return false;

                    var controllerNamespace = controllerActionDescriptor.ControllerTypeInfo.Namespace ?? string.Empty;
                    return controllerNamespace.StartsWith("TheOfficeAPI.Level1");
                });

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
