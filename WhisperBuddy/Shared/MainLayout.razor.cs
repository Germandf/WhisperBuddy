using Microsoft.AspNetCore.Components;
using WhisperBuddy.Services;

namespace WhisperBuddy.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public required ISettingsService SettingsService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    protected override void OnInitialized()
    {
        SettingsService.OnStateHasChanged = StateHasChanged;
    }

    void NavigateToIndex()
    {
        NavigationManager.NavigateTo("");
    }

    void NavigateToSettings()
    {
        NavigationManager.NavigateTo("settings");
    }
}
