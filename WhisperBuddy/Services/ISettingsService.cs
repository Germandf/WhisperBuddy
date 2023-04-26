namespace WhisperBuddy.Services;

public interface ISettingsService
{
    bool IsDarkMode { get; set; }
    Action? OnStateHasChanged { get; set; }
}
