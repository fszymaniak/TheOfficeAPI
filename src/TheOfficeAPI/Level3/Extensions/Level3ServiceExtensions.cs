using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Extensions;

public static class Level3ServiceExtensions
{
    public static IServiceCollection AddLevel3Services(this IServiceCollection services)
    {
        // Register Level3 specific services
        services.AddSingleton<TheOfficeService>();

        return services;
    }
}
