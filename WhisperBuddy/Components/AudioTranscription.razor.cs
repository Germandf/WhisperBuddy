using Microsoft.AspNetCore.Components.Forms;
using Whisper.net;

namespace WhisperBuddy.Components;

public partial class AudioTranscription
{
    private IBrowserFile? _file;
    private List<string> _segments = new();

    private void UploadFiles(IBrowserFile file)
    {
        _file = file;
    }

    private void Generate()
    {
        using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");

        using var processor = whisperFactory.CreateBuilder()
            .WithSegmentEventHandler(OnNewSegment)
            .WithPrompt("ggml-base.bin")
            .WithTranslate()
            .WithLanguage("auto").Build();

        if (_file is null)
        {

        }
        else
        {
            _segments = new();

            using var fileStream = _file.OpenReadStream();

            processor.Process(fileStream);
        }
    }

    private void OnNewSegment(SegmentData e)
    {
        var prettySegment = $"CSSS {e.Start} ==> {e.End} : {e.Text}";
        _segments.Add(prettySegment);
    }
}