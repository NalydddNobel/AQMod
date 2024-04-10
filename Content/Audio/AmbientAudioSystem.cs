using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.Audio;
using Terraria.Audio;

namespace Aequus.Content.Audio;

[Autoload(Side = ModSide.Client)]
public class AmbientAudioSystem : ModSystem {
    private int _ambientAudioCooldown;

    private void handleRainInDebug(in SoundStyle style) {
        SoundStyle realStyle = style with { Volume = 0.7f, Type = SoundType.Ambient };
        ActiveSound sound = SoundEngine.FindActiveSound(in realStyle);
        if (sound == null) {
            SoundEngine.PlaySound(realStyle);
        }
    }
    private void stopSoundInDebug(in SoundStyle style) {
        SoundStyle realStyle = style with { Volume = 0.5f, Type = SoundType.Ambient };
        ActiveSound sound = SoundEngine.FindActiveSound(in realStyle);
        sound?.Stop();
    }

    private void UpdatePollutedOceanAmbience(Player player) {
        float rainIntensity = Main.raining ? Main.maxRaining : 0f;

        if (rainIntensity > 0.5f) {
            if (Main.rand.NextBool(1200)) {
                AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientB, player.Center);
                _ambientAudioCooldown = 600;
            }
            else if (Main.rand.NextBool(1200)) {
                AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientA, player.Center);
                _ambientAudioCooldown = 300;
            }

            stopSoundInDebug(in AequusSounds.PollutedOcean_RainA);
            handleRainInDebug(in AequusSounds.PollutedOcean_RainB);
        }
        else {
            if (Main.rand.NextBool(4800 - (int)(4800 * Main.maxRaining))) {
                AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientA, player.Center);
                _ambientAudioCooldown = 500;
            }

            stopSoundInDebug(in AequusSounds.PollutedOcean_RainB);
            handleRainInDebug(in AequusSounds.PollutedOcean_RainA);
        }
    }

    public override void PreUpdateEntities() {
        if (Main.ambientVolume <= 0f) {
            return;
        }

        // Tick-based cooldown for ambient clips.
        if (_ambientAudioCooldown > 0) {
            _ambientAudioCooldown--;
        }

        Player player = Main.LocalPlayer;
        if (player.InModBiome<PollutedOceanBiomeUnderground>() && player.position.Y > Main.worldSurface * 16) {
            UpdatePollutedOceanAmbience(player);
        }
    }
}
