using Microsoft.Xna.Framework.Audio;
using System;

namespace Aequu2.Core.Audio;

/// <summary>Currently only supports .ogg</summary>
public class DynamicSoundStyle {
    public readonly string Path;
    public readonly IAudioEffect[] Effects;
    public readonly int SampleRate;
    public readonly AudioChannels Channels;
    private readonly byte[] _originalBuffer;
    private byte[] _outputBuffer;
    private DynamicSoundEffectInstance _sound;

    public DynamicSoundStyle(string path, params IAudioEffect[] effects) {
        Path = path;
        Effects = effects;

        RawSoundReader.ReadOggAsByteArray(path, out _originalBuffer, out _, out SampleRate, out Channels);
        _sound = new DynamicSoundEffectInstance(SampleRate, Channels);
        //_sound.BufferNeeded += OnBufferNeeded;
    }

    private void ApplyEffects(byte[] inputBuffer) {
        foreach (var effect in Effects) {
            _outputBuffer = effect.ModifyBuffer(inputBuffer, SampleRate, (int)Channels);
        }
    }

    private void OnBufferNeeded(object sender, EventArgs e) {
        //ApplyEffects(_originalBuffer);
        //_sound.SubmitBuffer(_outputBuffer);
        //Stop();
    }

    public void Play() {
        ApplyEffects(_originalBuffer);
        _sound.Play();
        _sound.SubmitBuffer(_outputBuffer);
        _sound.Volume = Main.soundVolume;
    }

    public void Stop() {
        _sound.Stop();
    }

    public void Pause() {
        _sound.Pause();
    }

    public void Resume() {
        _sound.Resume();
    }

    ~DynamicSoundStyle() {
        if (_sound != null && !_sound.IsDisposed) {
            Main.QueueMainThreadAction(_sound.Dispose);
        }
    }
}
