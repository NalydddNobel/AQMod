using System;
using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class BlackPhialSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return Aequus.GetSound("concoction1").WithVolume(0.7f);
        }
    }
}