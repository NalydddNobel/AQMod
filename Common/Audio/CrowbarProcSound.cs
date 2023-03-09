using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class CrowbarProcSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.proc;
        }
    }
}