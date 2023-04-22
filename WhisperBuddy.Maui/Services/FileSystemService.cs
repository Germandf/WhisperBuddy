using WhisperBuddy.Services;

namespace WhisperBuddy.Maui.Services;

public class FileSystemService : IFileSystemService
{
    public string GetAppDataDirectory()
    {
        return FileSystem.Current.AppDataDirectory;
    }
}
