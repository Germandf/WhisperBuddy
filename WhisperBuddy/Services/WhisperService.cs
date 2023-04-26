using System.Runtime.CompilerServices;
using Whisper.net;
using Whisper.net.Ggml;

namespace WhisperBuddy.Services;

public class WhisperService : IWhisperService
{
    private readonly IWhisperModelService _whisperModelService;

    public WhisperService(IWhisperModelService whisperModelService)
    {
        _whisperModelService = whisperModelService;
    }

    public async IAsyncEnumerable<SegmentData> TranscribeAudio(GgmlType ggmlType, Stream wavStream, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var whisperModelPath = _whisperModelService.GetPath(ggmlType);

        using var whisperFactory = WhisperFactory.FromPath(whisperModelPath);

        using var processor = whisperFactory
            .CreateBuilder()
            .WithLanguageDetection()
            .Build();

        await foreach (var segmentData in processor.ProcessAsync(wavStream, cancellationToken))
        {
            yield return segmentData;
        }
    }
}
