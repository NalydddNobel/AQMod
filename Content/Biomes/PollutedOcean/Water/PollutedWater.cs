using AequusRemake.Core.ContentGeneration;

namespace AequusRemake.Content.Biomes.PollutedOcean.Water;

public class PollutedWater : UnifiedWaterStyle {
    public override void OnLoad() {
        (DropletType as InstancedWaterDroplet)?.OverrideSound(SoundID.Drip with { Volume = 0.066f });
        HairColor = Color.Turquoise;
    }
}