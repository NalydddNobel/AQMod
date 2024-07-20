using Terraria.Audio;

namespace Aequus.Common.Net.Sounds;
public class AethersWrathSound : NetSound {
    protected override SoundStyle InitDefaultSoundStyle() {
        return AequusSounds.inflictaetherfire with { Pitch = 0.2f, PitchVariance = 0.3f };
    }
}