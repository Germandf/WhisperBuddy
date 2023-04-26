using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WhisperBuddy.Data;

namespace WhisperBuddy;

public static class DependencyInjection
{
    public static void AddWhisperBuddyServices(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddSingleton<WeatherForecastService>();
    }
}
