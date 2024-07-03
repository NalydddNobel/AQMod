using Aequu2.Core.ContentGeneration;

namespace Aequu2.Content.Biomes.PollutedOcean.Water;

public class PollutedWater : UnifiedWaterStyle {
    public override void OnLoad() {
        (DropletType as InstancedWaterDroplet)?.OverrideSound(SoundID.Drip with { Volume = 0.066f });
        HairColor = Color.Turquoise;
    }
}