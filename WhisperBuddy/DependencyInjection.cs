﻿using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WhisperBuddy.Services;

namespace WhisperBuddy;

public static class DependencyInjection
{
    public static void AddWhisperBuddyServices(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IWavService, WavService>();
        services.AddSingleton<IWhisperModelService, WhisperModelService>();
        services.AddSingleton<IWhisperService, WhisperService>();
    }
}
