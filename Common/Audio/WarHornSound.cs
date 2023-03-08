using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class WarHornSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("Item/warhorn").WithVolume(0.3f);
        }
    }
}