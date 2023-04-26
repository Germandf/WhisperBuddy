namespace WhisperBuddy.Services;

internal class SettingsService : ISettingsService
{
    private bool _isDarkMode;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            _isDarkMode = value;
            OnStateHasChanged?.Invoke();
        }
    }

    public Action? OnStateHasChanged { get; set; }
}
