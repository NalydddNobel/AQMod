using Terraria.Audio;

namespace Aequus.Common.Net.Sounds {
    public class WeaknessDebuffSound : NetSound {
        protected override SoundStyle InitDefaultSoundStyle() {
            return AequusSounds.inflictweakness with { Volume = 2f };
        }
    }
}