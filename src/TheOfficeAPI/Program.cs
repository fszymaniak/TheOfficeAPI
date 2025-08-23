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

        // Configure URL consistently
        builder.WebHost.UseUrls(serverOptions?.DefaultUrl ?? "http://localhost:5000");

        var maturityLevel = DetermineMaturityLevel(environmentOptions?.MaturityLevelVariable ?? "MATURITY_LEVEL");
        var isLevel0 = maturityLevel == MaturityLevel.Level0;

        if (isLevel0)
        {
            Console.WriteLine("Starting with Richardson Maturity Level 0 configuration...");
            builder.Services.ConfigureServices(maturityLevel);
        }
        else
        {
            Console.WriteLine("Starting with basic configuration...");
            builder.Services.AddControllers();
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
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/", () => "API is running. Use Level0 profile for Richardson Level 0 implementation.");
    }
}