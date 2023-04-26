using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NAudio.Wave;
using System.Diagnostics;
using Whisper.net;
using Whisper.net.Ggml;
using WhisperBuddy.Services;

namespace WhisperBuddy.Components;

public partial class AudioTranscription
{
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IFileSystemService FileSystemService { get; set; }
    [Inject] public required IWhisperModelService WhisperModelService { get; set; }
    [Inject] public required IWhisperService WhisperService { get; set; }

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
            var ggmlType = GgmlType.Base;

            if (!WhisperModelService.IsDownloaded(ggmlType))
            {
                await WhisperModelService.Download(ggmlType);
            }

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

                var reader = new StreamMediaFoundationReader(memoryStream);
                var newFormat = new WaveFormat(16000, reader.WaveFormat.Channels);
                var resampler = new MediaFoundationResampler(reader, newFormat);
                var outputDirectory = Path.Combine(appDataDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".wav"));
                WaveFileWriter.CreateWaveFile(outputDirectory, resampler);

                var file = new FileStream(outputDirectory, FileMode.Open, FileAccess.Read);
                using var outputMemoryStream = new MemoryStream();
                await file.CopyToAsync(outputMemoryStream);
                outputMemoryStream.Seek(0, SeekOrigin.Begin);

                await foreach (var segmentData in WhisperService.TranscribeAudio(ggmlType, outputMemoryStream))
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
