using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class WeaknessDebuffSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.inflictweakness with { Volume = 2f };
        }
    }
}