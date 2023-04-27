using WhisperBuddy.Models;

namespace WhisperBuddy.Services;

public interface IWavService
{
    WavFileInfo ConvertToWav(Stream stream);
}
