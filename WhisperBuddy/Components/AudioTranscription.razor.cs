using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Whisper.net;
using Whisper.net.Ggml;
using WhisperBuddy.Services;

namespace WhisperBuddy.Components;

public partial class AudioTranscription
{
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IFileSystemService FileSystemService { get; set; }

    private IBrowserFile? _browserFile;
    private List<string> _segments = new();

    private void UploadFiles(IBrowserFile file)
    {
        _browserFile = file;
    }

    private async Task Generate()
    {
        try
        {
            var appDataDirectory = FileSystemService.GetAppDataDirectory();
            var modelName = "ggml-base.bin";
            var modelNameDirectory = Path.Combine(appDataDirectory, modelName);

            if (!File.Exists(modelNameDirectory))
            {
                using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(GgmlType.Base);
                using var fileWriter = File.OpenWrite(modelNameDirectory);
                await modelStream.CopyToAsync(fileWriter);
            }

            using var whisperFactory = WhisperFactory.FromPath(modelNameDirectory);

            using var processor = whisperFactory.CreateBuilder().Build();

            if (_browserFile is null)
            {
                Snackbar.Add("File is null", Severity.Error);
            }
            else
            {
                _segments = new();

                await using FileStream fileStream = new(
                    Path.Combine(appDataDirectory, _browserFile.Name), FileMode.Create);

                await _browserFile.OpenReadStream(long.MaxValue).CopyToAsync(fileStream);

                await foreach (var segmentData in processor.ProcessAsync(fileStream))
                {
                    _segments.Add(segmentData.Text);
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}
