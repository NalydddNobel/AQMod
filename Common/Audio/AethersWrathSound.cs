using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class AethersWrathSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("inflictaetherfire", volume: 1f, pitch: 0.2f, variance: 0.3f);
        }
    }
}