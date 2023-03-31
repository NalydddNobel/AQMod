using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class WarHornSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.warhorn with { Volume = 0.3f };
        }
    }
}