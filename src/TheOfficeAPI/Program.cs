using TheOfficeAPI.Common.Enums;
using TheOfficeAPI.Common.Extensions;

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

        // Check if Level0 profile is active using environment variable
        var maturityLevelString = Environment.GetEnvironmentVariable("MATURITY_LEVEL");
        var maturityLevel = Enum.TryParse<MaturityLevel>(maturityLevelString, out var level)
            ? level
            : (MaturityLevel?)null;

        var isLevel0 = maturityLevel == MaturityLevel.Level0;

        if (isLevel0)
        {
            Console.WriteLine("Starting with Richardson Maturity Level 0 configuration...");

            // Configure services using extension method only for Level0 profile
            builder.Services.ConfigureServices(maturityLevel);
        }
        else
        {
            Console.WriteLine("Starting with basic configuration...");

            // Basic services for other profiles
            builder.Services.AddControllers();
        }

        var app = builder.Build();

        if (isLevel0)
        {
            // Configure pipeline using extension method only for Level0 profile
            app.ConfigurePipeline(maturityLevel);
        }
        else
        {
            // Basic pipeline for other profiles
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.MapGet("/", () => "API is running. Use Level0 profile for Richardson Level 0 implementation.");
        }

        return app;
    }
}