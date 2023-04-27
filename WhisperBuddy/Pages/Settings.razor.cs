using Microsoft.AspNetCore.Components;
using WhisperBuddy.Services;

namespace WhisperBuddy.Pages;

public partial class Settings : ComponentBase
{
    [Inject] public required ISettingsService SettingsService { get; set; }
}