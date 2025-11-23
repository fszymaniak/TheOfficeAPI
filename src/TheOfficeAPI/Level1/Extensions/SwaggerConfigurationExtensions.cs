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
                // Register all API versions
                c.SwaggerDoc("v0", new OpenApiInfo
                {
                    Title = "The Office API - Level 0",
                    Version = "v0",
                    Description = "Richardson Maturity Model Level 0 implementation"
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Office API - Level 1",
                    Version = "v1",
                    Description = "Richardson Maturity Model Level 1 implementation - Introduces resource-based URIs"
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "The Office API - Level 2",
                    Version = "v2",
                    Description = "Richardson Maturity Model Level 2 implementation - Introduces HTTP verbs and proper status codes"
                });

                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Title = "The Office API - Level 3",
                    Version = "v3",
                    Description = "Richardson Maturity Model Level 3 implementation - HATEOAS with hypermedia links"
                });

                c.EnableAnnotations(); // This is crucial for SwaggerSchema to work

                // Filter controllers by namespace for each version
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor == null) return false;

                    var controllerNamespace = controllerActionDescriptor.ControllerTypeInfo.Namespace ?? string.Empty;

                    return docName switch
                    {
                        "v0" => controllerNamespace.StartsWith("TheOfficeAPI.Level0"),
                        "v1" => controllerNamespace.StartsWith("TheOfficeAPI.Level1"),
                        "v2" => controllerNamespace.StartsWith("TheOfficeAPI.Level2"),
                        "v3" => controllerNamespace.StartsWith("TheOfficeAPI.Level3"),
                        _ => false
                    };
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
                // Add all API versions to Swagger UI dropdown
                c.SwaggerEndpoint("/swagger/v0/swagger.json", "The Office API V0 (Level 0)");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Office API V1 (Level 1)");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "The Office API V2 (Level 2)");
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "The Office API V3 (Level 3 - HATEOAS)");
                c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger

                // Ensure the API dropdown selector is visible
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
            });
        }
    }
}
