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

        // RAILWAY FIX: ALWAYS use Railway's PORT environment variable and bind to 0.0.0.0
        // This MUST come AFTER builder creation but BEFORE any other WebHost configuration
        var port = Environment.GetEnvironmentVariable("PORT");
        string url;
        
        if (port != null)
        {
            // Railway or production environment
            url = $"http://0.0.0.0:{port}";
            Console.WriteLine($"=== RAILWAY/PRODUCTION MODE ===");
            Console.WriteLine($"PORT from environment: {port}");
            Console.WriteLine($"Binding to: {url}");
            Console.WriteLine($"================================");
        }
        else
        {
            // Local development
            url = serverOptions?.DefaultUrl ?? "http://localhost:5000";
            Console.WriteLine($"=== LOCAL DEVELOPMENT MODE ===");
            Console.WriteLine($"Using config URL: {url}");
            Console.WriteLine($"================================");
        }

        // Clear any existing URLs and set ours
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
            builder.Services.AddSwaggerGen();
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
        // Enable Swagger in all environments
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/", () => "API is running. Use Level0 profile for Richardson Level 0 implementation.");
    }
}