using TheOfficeAPI.Level0.Services;

namespace TheOfficeAPI.Level0.Extensions;

public static class Level0ServiceExtensions
{
    public static IServiceCollection AddLevel0Services(this IServiceCollection services)
    {
        // Register Level0 specific services
        services.AddSingleton<TheOfficeService>();

        return services;
    }
}