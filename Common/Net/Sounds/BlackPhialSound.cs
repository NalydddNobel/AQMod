using System;
using Terraria.Audio;

namespace Aequus.Common.Net.Sounds {
    public class BlackPhialSound : NetSound {
        protected override SoundStyle InitDefaultSoundStyle() {
            return AequusSounds.concoction1 with { Volume = 0.7f };
        }
    }
}