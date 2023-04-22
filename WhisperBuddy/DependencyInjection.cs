using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace WhisperBuddy;

public static class DependencyInjection
{
    public static void AddWhisperBuddyServices(this IServiceCollection services)
    {
        services.AddMudServices();
    }
}
