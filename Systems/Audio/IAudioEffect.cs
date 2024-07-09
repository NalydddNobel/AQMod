namespace AequusRemake.Systems.Audio;

public interface IAudioEffect {
    byte[] ModifyBuffer(byte[] buffer, int sampleRate, int channels);
}
