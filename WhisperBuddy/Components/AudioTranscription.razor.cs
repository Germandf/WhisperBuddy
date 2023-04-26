using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NAudio.Wave;
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

                using var stream = _browserFile.OpenReadStream(long.MaxValue);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new WaveFileReader(memoryStream);
                var newFormat = new WaveFormat(16000, reader.WaveFormat.Channels);
                var resampler = new MediaFoundationResampler(reader, newFormat);
                var outputDirectory = Path.Combine(appDataDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".wav"));
                WaveFileWriter.CreateWaveFile(outputDirectory, resampler);

                var file = new FileStream(outputDirectory, FileMode.Open, FileAccess.Read);
                using var outputMemoryStream = new MemoryStream();
                await file.CopyToAsync(outputMemoryStream);
                outputMemoryStream.Seek(0, SeekOrigin.Begin);

                await foreach (var segmentData in processor.ProcessAsync(outputMemoryStream))
                {
                    _segments.Add($"{segmentData.Start} -> {segmentData.End} : {segmentData.Text}");
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}
