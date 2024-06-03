using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;
using Aequus.Content.Tiles.Furniture.Trash;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiomeSurface : ModBiome {
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => PollutedOceanSystem.Music;

    public override int BiomeTorchItemType => ModContent.GetInstance<TrashTorch>().Item.Type;
    public override int BiomeCampfireItemType => ModContent.GetInstance<TrashTorch>().CampfireItem.Type;

    public override string MapBackground => BackgroundPath;

    public override float GetWeight(Player player) {
        // Increase weight depending on how many tiles are nearby.
        return PollutedOceanSystem.PollutedTileCount / (float)PollutedOceanSystem.PollutedTileThreshold;
    }

    public override bool IsBiomeActive(Player player) {
        return player.position.Y < Main.worldSurface * 16.0 && PollutedOceanSystem.CheckBiome(player);
    }
}
