using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class WeaknessDebuffSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("inflictweakness", volume: 2f);
        }
    }
}