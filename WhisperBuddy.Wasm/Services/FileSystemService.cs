using WhisperBuddy.Services;

namespace WhisperBuddy.Wasm.Services;

public class FileSystemService : IFileSystemService
{
    public string GetAppDataDirectory()
    {
        return Path.GetTempPath();
    }
}
