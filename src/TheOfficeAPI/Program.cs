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
        var isLevel0 = maturityLevel == MaturityLevel.Level0;

        if (isLevel0)
        {
            Console.WriteLine("Starting with Richardson Maturity Level 0 configuration...");
            builder.Services.ConfigureServices(maturityLevel);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
        else
        {
            Console.WriteLine("Starting with basic configuration...");
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "The Office API",
                    Version = "v1"
                });

                // Include XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (System.IO.File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Register TheOfficeService for Level0 controllers
            builder.Services.AddSingleton<TheOfficeAPI.Level0.Services.TheOfficeService>();
        }

        var app = builder.Build();

        if (isLevel0)
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
    
        // Enable Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "The Office API v1");
            c.RoutePrefix = "swagger";
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