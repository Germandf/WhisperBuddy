using Whisper.net;
using Whisper.net.Ggml;

namespace WhisperBuddy.Services;

public interface IWhisperService
{
    IAsyncEnumerable<SegmentData> TranscribeAudio(
        GgmlType ggmlType, Stream wavStream, CancellationToken cancellationToken = default);
}
