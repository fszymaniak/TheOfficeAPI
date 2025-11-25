using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Common.Extensions;
using TheOfficeAPI.Configuration;

namespace TheOfficeAPI;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebApplication(args).Run();
    }

    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure strongly-typed options
        builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(ServerOptions.SectionName));
        builder.Services.Configure<EnvironmentOptions>(
            builder.Configuration.GetSection(EnvironmentOptions.SectionName));

        // Get configuration values
        var serverOptions = builder.Configuration.GetSection(ServerOptions.SectionName).Get<ServerOptions>();
        var environmentOptions =
            builder.Configuration.GetSection(EnvironmentOptions.SectionName).Get<EnvironmentOptions>();

        // RAILWAY: Use Railway's PORT environment variable and bind to 0.0.0.0
        var port = Environment.GetEnvironmentVariable("PORT");
        string url;
        
        if (port != null)
        {
            url = $"http://0.0.0.0:{port}";
            Console.WriteLine($"=== RAILWAY/PRODUCTION MODE ===");
            Console.WriteLine($"PORT from environment: {port}");
            Console.WriteLine($"Binding to: {url}");
            Console.WriteLine($"================================");
        }
        else
        {
            url = serverOptions?.DefaultUrl ?? "http://localhost:5000";
            Console.WriteLine($"=== LOCAL DEVELOPMENT MODE ===");
            Console.WriteLine($"Using config URL: {url}");
            Console.WriteLine($"================================");
        }

        builder.WebHost.UseUrls(url);

        var maturityLevel = DetermineMaturityLevel(environmentOptions?.MaturityLevelVariable ?? "MATURITY_LEVEL");
        var hasMaturityLevel = maturityLevel == MaturityLevel.Level0 || maturityLevel == MaturityLevel.Level1 || maturityLevel == MaturityLevel.Level2 || maturityLevel == MaturityLevel.Level3;

        if (hasMaturityLevel)
        {
            Console.WriteLine($"Starting with Richardson Maturity {maturityLevel} configuration...");
            builder.Services.ConfigureServices(maturityLevel);
            builder.Services.AddEndpointsApiExplorer();
        }
        else
        {
            Console.WriteLine("Starting with basic configuration...");
            builder.Services.AddSingleton<TheOfficeAPI.Common.Services.HealthCheckService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Register all API versions
                c.SwaggerDoc("v0", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The Office API - Level 0",
                    Version = "v0",
                    Description = "Richardson Maturity Model Level 0 implementation"
                });

                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The Office API - Level 1",
                    Version = "v1",
                    Description = "Richardson Maturity Model Level 1 implementation - Introduces resource-based URIs"
                });

                c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The Office API - Level 2",
                    Version = "v2",
                    Description = "Richardson Maturity Model Level 2 implementation - Introduces HTTP verbs and proper status codes"
                });

                c.SwaggerDoc("v3", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The Office API - Level 3",
                    Version = "v3",
                    Description = "Richardson Maturity Model Level 3 implementation - HATEOAS with hypermedia links"
                });

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

                // Include XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (System.IO.File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Register services for all levels to support all API versions
            builder.Services.AddSingleton<TheOfficeAPI.Level0.Services.TheOfficeService>();
            builder.Services.AddSingleton<TheOfficeAPI.Level1.Services.TheOfficeService>();
            builder.Services.AddSingleton<TheOfficeAPI.Level2.Services.TheOfficeService>();
            builder.Services.AddSingleton<TheOfficeAPI.Level3.Services.TheOfficeService>();
        }

        var app = builder.Build();

        if (hasMaturityLevel)
        {
            app.ConfigurePipeline(maturityLevel);
        }
        else
        {
            ConfigureBasicPipeline(app);
        }

        return app;
    }

    private static MaturityLevel? DetermineMaturityLevel(string environmentVariable)
    {
        var maturityLevelString = Environment.GetEnvironmentVariable(environmentVariable);
        return Enum.TryParse<MaturityLevel>(maturityLevelString, out var level) ? level : null;
    }

    private static void ConfigureBasicPipeline(WebApplication app)
    {
        Console.WriteLine("=== CONFIGURING BASIC PIPELINE ===");
    
        // Root endpoint
        app.MapGet("/", () => Results.Ok(new {
            message = "The Office API is running",
            endpoints = new {
                swagger = "/swagger",
                health = "/health",
                api = "/api"
            }
        }));
    
        // Enable Swagger with all API versions
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            // Add all API versions to Swagger UI dropdown
            c.SwaggerEndpoint("/swagger/v0/swagger.json", "The Office API V0 (Level 0)");
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Office API V1 (Level 1)");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "The Office API V2 (Level 2)");
            c.SwaggerEndpoint("/swagger/v3/swagger.json", "The Office API V3 (Level 3 - HATEOAS)");
            c.RoutePrefix = "swagger";

            // Ensure the API dropdown selector is visible
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
        });
    
        Console.WriteLine("Health endpoint: /health");
        Console.WriteLine("Swagger UI: /swagger");
        Console.WriteLine("Root: /");
        Console.WriteLine("==================================");
    
        // Don't use HTTPS redirect on Railway
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}