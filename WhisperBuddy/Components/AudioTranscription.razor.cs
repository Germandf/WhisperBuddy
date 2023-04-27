using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.IO;
using System.Text;
using Whisper.net.Ggml;
using WhisperBuddy.Services;

namespace WhisperBuddy.Components;

public partial class AudioTranscription
{
    [Inject] public required ISnackbar Snackbar { get; set; }
    [Inject] public required IWhisperModelService WhisperModelService { get; set; }
    [Inject] public required IWhisperService WhisperService { get; set; }
    [Inject] public required IWavService WavService { get; set; }

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

                var outputDirectory = WavService.ConvertToWav(memoryStream);

                var file = new FileStream(outputDirectory, FileMode.Open, FileAccess.Read);

                await foreach (var segmentData in WhisperService.TranscribeAudio(ggmlType, file))
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
