using TheOfficeAPI.Level1.Services;

namespace TheOfficeAPI.Level1.Extensions;

public static class Level1ServiceExtensions
{
    public static IServiceCollection AddLevel1Services(this IServiceCollection services)
    {
        // Register Level1 specific services
        services.AddSingleton<TheOfficeService>();

        return services;
    }
}
