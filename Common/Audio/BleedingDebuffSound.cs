using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class BleedingDebuffSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("inflictBlood").WithVolume(0.5f);
        }
    }
}