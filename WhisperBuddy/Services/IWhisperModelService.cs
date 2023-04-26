using Whisper.net.Ggml;

namespace WhisperBuddy.Services;

public interface IWhisperModelService
{
    bool IsDownloaded(GgmlType ggmlType);
    Task Download(GgmlType ggmlType);
    string GetPath(GgmlType ggmlType);
}
