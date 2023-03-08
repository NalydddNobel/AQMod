using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class BlueFireDebuffSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("inflictFire", variance: 0.3f);
        }
    }
}