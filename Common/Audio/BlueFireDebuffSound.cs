using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class BlueFireDebuffSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.inflictFire with { PitchVariance = 0.3f };
        }
    }
}