using NAudio.Wave;

namespace WhisperBuddy.Services;

public class WavService : IWavService
{
    private readonly IFileSystemService _fileSystemService;

    public WavService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    public string ConvertToWav(Stream stream)
    {
        var appDataDirectory = _fileSystemService.GetAppDataDirectory();
        var reader = new StreamMediaFoundationReader(stream);
        var newFormat = new WaveFormat(16000, reader.WaveFormat.Channels);
        var resampler = new MediaFoundationResampler(reader, newFormat);
        var outputDirectory = Path.Combine(appDataDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".wav"));
        WaveFileWriter.CreateWaveFile(outputDirectory, resampler);
        return outputDirectory;
    }
}
