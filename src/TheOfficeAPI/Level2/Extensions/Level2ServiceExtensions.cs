using TheOfficeAPI.Level2.Services;

namespace TheOfficeAPI.Level2.Extensions;

public static class Level2ServiceExtensions
{
    public static IServiceCollection AddLevel2Services(this IServiceCollection services)
    {
        // Register Level2 specific services
        services.AddSingleton<TheOfficeService>();

        return services;
    }
}
