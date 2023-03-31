using System;
using Terraria.Audio;

namespace Aequus.Common.Audio
{
    public class BlackPhialSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.concoction1 with { Volume = 0.7f };
        }
    }
}