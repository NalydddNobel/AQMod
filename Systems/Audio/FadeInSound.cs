using ReLogic.Utilities;
using System;
using Terraria.Audio;

namespace AequusRemake.Systems.Audio;

public record class FadeInSound(SoundStyle Sound) {
    private SlotId _slotId;
    private float _volume;
    private float _volumeFadeIn;

    public void FadeIn(float amount) {
        _volumeFadeIn = Math.Max(_volumeFadeIn, amount);
        if (SoundEngine.TryGetActiveSound(_slotId, out _) && !Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left)) {
            return;
        }

        _slotId = SoundEngine.PlaySound(Sound with { IsLooped = true, Type = SoundType.Ambient }, updateCallback: AudioCallback);
    }

    public void FadeOut(float amount) {
        _volumeFadeIn = Math.Min(_volumeFadeIn, -amount);
    }

    private bool AudioCallback(ActiveSound sound) {
        _volume = Math.Clamp(_volume + _volumeFadeIn, 0f, 1f);
        if (_volume < 0f) {
            _volumeFadeIn = 0f;
            _volume = 0f;
            _slotId = SlotId.Invalid;
            return false;
        }

        sound.Volume = _volume;
        return true;
    }
}
