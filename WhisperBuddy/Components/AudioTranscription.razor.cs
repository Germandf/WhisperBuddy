using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
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
    private double _progressPercentage;
    private bool _generating;

    private void UploadFiles(IBrowserFile file)
    {
        _browserFile = file;
    }

    private async Task Generate()
    {
        _generating = true;

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

                var wavFileInfo = WavService.ConvertToWav(memoryStream);

                var file = new FileStream(wavFileInfo.Path, FileMode.Open, FileAccess.Read);

                await foreach (var segmentData in WhisperService.TranscribeAudio(ggmlType, file))
                {
                    _segments.Add($"{segmentData.Start} -> {segmentData.End} : {segmentData.Text}");
                    _progressPercentage = (double) segmentData.End.TotalMilliseconds / wavFileInfo.TotalTime.TotalMilliseconds * 100;
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }

        _generating = false;
    }
}
