using Aequu2.Content.Biomes.PollutedOcean;
using Aequu2.Core.Audio;

namespace Aequu2.Content.Audio;

[Autoload(Side = ModSide.Client)]
public class AmbientAudioSystem : ModSystem {
    private int _ambientAudioCooldown;

    private readonly FadeInSound _pollutedCaveRainA = new FadeInSound(AequusSounds.PollutedOcean_RainA with { Volume = 0.3f });
    private readonly FadeInSound _pollutedCaveRainB = new FadeInSound(AequusSounds.PollutedOcean_RainB with { Volume = 0.3f });
    private readonly FadeInSound _pollutedCaveDrone = new FadeInSound(AequusSounds.PollutedOcean_CaveDrone with { Volume = 0.1f });

    private void UpdatePollutedOceanAmbience(Player player) {
        float rainIntensity = Main.raining ? Main.maxRaining : 0f;

        if (rainIntensity > 0.5f) {
            if (_ambientAudioCooldown == 0) {
                if (Main.rand.NextBool(1200)) {
                    AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientB, player.Center);
                    _ambientAudioCooldown = 600;
                }
                else if (Main.rand.NextBool(1200)) {
                    AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientA, player.Center);
                    _ambientAudioCooldown = 300;
                }
            }

            _pollutedCaveRainB.FadeIn(rainIntensity * 0.05f);
            _pollutedCaveRainA.FadeOut(rainIntensity * 0.05f);
        }
        else {
            if (_ambientAudioCooldown == 0 && Main.rand.NextBool(4800 - (int)(4800 * Main.maxRaining))) {
                AudioEffects.PlayUnpanningAttenuationSound(AequusSounds.PollutedOcean_AmbientA, player.Center);
                _ambientAudioCooldown = 500;
            }

            _pollutedCaveRainA.FadeIn(rainIntensity * 0.05f + 0.01f);
            _pollutedCaveRainB.FadeOut(rainIntensity * 0.05f + 0.01f);
        }

        _pollutedCaveDrone.FadeIn(0.01f);
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
        else {
            _pollutedCaveRainA.FadeOut(0.01f);
            _pollutedCaveRainB.FadeOut(0.01f);
            _pollutedCaveDrone.FadeOut(0.01f);
        }
    }
}
