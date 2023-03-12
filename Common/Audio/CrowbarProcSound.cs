using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class CrowbarProcSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.proc_PrecisionGloves.Sound with { Volume = 0.7f, PitchVariance = 0.2f, MaxInstances = 8 };
        }
    }
}