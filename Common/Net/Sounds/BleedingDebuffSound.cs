using Terraria.Audio;

namespace Aequus.Common.Net.Sounds {
    public class BleedingDebuffSound : NetSound {
        protected override SoundStyle InitDefaultSoundStyle() {
            return AequusSounds.inflictBlood with { Volume = 0.33f };
        }
    }
}