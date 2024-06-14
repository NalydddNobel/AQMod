using System;

namespace Aequus.Core.Audio;

public class EchoEffect(int DelayInMilliseconds, float DecayFactor) : IAudioEffect {
    private readonly int delayMilliseconds = DelayInMilliseconds;
    private readonly float decayFactor = DecayFactor;

    byte[] IAudioEffect.ModifyBuffer(byte[] buffer, int sampleRate, int channels) {
        int delaySamples = sampleRate * channels * delayMilliseconds / 1000;
        int sampleSize = 2; // Assuming 16-bit audio (2 bytes per sample)

        byte[] newBuffer = new byte[buffer.Length];
        Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);

        float volume = decayFactor;
        int count = 1;
        while (volume > 0.05f) {
            Array.Resize(ref newBuffer, newBuffer.Length + delaySamples);
            int start = delaySamples * count;
            // Fixes issue where "start" is on an odd number, which will read the bytes incorrectly.
            start -= start % 2;

            for (int i = 0; i < buffer.Length / 2; i++) {
                int sampleIndex = i * sampleSize;
                int echoIndex = (i) * sampleSize + start;

                short originalSample = BitConverter.ToInt16(newBuffer, echoIndex);
                short echoSample = BitConverter.ToInt16(buffer, sampleIndex);

                short mixedSample = (short)(originalSample + echoSample * volume);

                byte[] mixedBytes = BitConverter.GetBytes(mixedSample);
                Buffer.BlockCopy(mixedBytes, 0, newBuffer, echoIndex, sampleSize);
            }
            volume *= decayFactor;
            count++;
        }

        return newBuffer;
        /*

        byte[] echoBuffer = new byte[buffer.Length + delaySamples * 2];

        for (int i = 0; i < buffer.Length / sampleSize; i++) {
            int sampleIndex = i * sampleSize;
            short originalSample = BitConverter.ToInt16(buffer, sampleIndex);

            Buffer.BlockCopy(buffer, sampleIndex, echoBuffer, sampleIndex, sampleSize);

            int echoIndex = sampleIndex + delaySamples * sampleSize;
            if (echoIndex < echoBuffer.Length) {
                short echoSample = BitConverter.ToInt16(echoBuffer, echoIndex);
                short mixedSample = (short)(originalSample + echoSample * decayFactor);

                byte[] mixedBytes = BitConverter.GetBytes(mixedSample);
                Buffer.BlockCopy(mixedBytes, 0, echoBuffer, echoIndex, sampleSize);
            }
        }

        return echoBuffer;
        */
    }
}
