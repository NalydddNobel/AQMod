using Terraria.Audio;

namespace Aequus.Common.Net.Sounds;
public class WarHornSound : NetSound {
    protected override SoundStyle InitDefaultSoundStyle() {
        return AequusSounds.warhorn with { Volume = 0.3f };
    }
}