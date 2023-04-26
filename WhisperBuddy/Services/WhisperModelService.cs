using Whisper.net.Ggml;

namespace WhisperBuddy.Services;

public class WhisperModelService : IWhisperModelService
{
    private IFileSystemService _fileSystemService;

    public WhisperModelService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    public bool IsDownloaded(GgmlType ggmlType)
    {
        var modelNameDirectory = GetModelNameDirectory(ggmlType);
        return File.Exists(modelNameDirectory);
    }

    public async Task Download(GgmlType ggmlType)
    {
        var modelNameDirectory = GetModelNameDirectory(ggmlType);
        using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(GgmlType.Base);
        using var fileWriter = File.OpenWrite(modelNameDirectory);
        await modelStream.CopyToAsync(fileWriter);
    }

    public string GetPath(GgmlType ggmlType)
    {
        var modelNameDirectory = GetModelNameDirectory(ggmlType);
        return modelNameDirectory;
    }

    private string GetModelNameDirectory(GgmlType ggmlType)
    {
        var appDataDirectory = _fileSystemService.GetAppDataDirectory();
        var modelName = GgmlTypeToString(ggmlType);
        var modelNameDirectory = Path.Combine(appDataDirectory, modelName);
        return modelNameDirectory;
    }

    private static string GgmlTypeToString(GgmlType modelType)
    {
        var modelTypeString = modelType switch
        {
            GgmlType.Base => "base",
            GgmlType.BaseEn => "base-en",
            GgmlType.Large => "large",
            GgmlType.LargeV1 => "lage-v1",
            GgmlType.Medium => "medium",
            GgmlType.MediumEn => "medium-en",
            GgmlType.Small => "small",
            GgmlType.SmallEn => "small-en",
            GgmlType.Tiny => "tiny",
            GgmlType.TinyEn => "tiny-en",
            _ => "unknown"
        };

        return $"ggml-{modelTypeString}.bin";
    }
}
