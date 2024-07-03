using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System.IO;
using Terraria.ModLoader.Assets;

namespace Aequu2.Core.Audio;

public class RawSoundReader {
    private static Mod Mod => Aequu2.Instance;

    public static void ReadOgg(string path, out float[] buffer, out long totalSamples, out int sampleRate, out AudioChannels channels) {
        Stream stream = Mod.GetFileStream(path, newFileStream: true);

        using var reader = new VorbisReader(stream, true);

        totalSamples = reader.TotalSamples;
        sampleRate = reader.SampleRate;
        channels = (AudioChannels)reader.Channels;
        buffer = new float[reader.TotalSamples * reader.Channels];

        reader.ReadSamples(buffer, 0, buffer.Length);
    }

    public static void ReadOggAsByteArray(string path, out byte[] buffer, out long totalSamples, out int sampleRate, out AudioChannels channels) {
        ReadOgg(path, out float[] floatBuffer, out totalSamples, out sampleRate, out channels);

        buffer = new byte[floatBuffer.Length * 2];
        OggReader.Convert(floatBuffer, buffer);
    }
}
