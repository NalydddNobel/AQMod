using AequusRemake.Content.Biomes.PollutedOcean.Background;
using AequusRemake.Content.Biomes.PollutedOcean.Water;
using AequusRemake.Content.Tiles.Furniture.Trash;
using AequusRemake.Core.Entites.Bestiary;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Content.Biomes.PollutedOcean;

public class PollutedOceanBiomeSurface : ModBiome, IPostSetupContent {
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => ModContent.GetInstance<PollutedOceanSystem>().Music;

    public override int BiomeTorchItemType => ModContent.GetInstance<SeaPickleTorch>().Item.Type;
    public override int BiomeCampfireItemType => ModContent.GetInstance<TrashFurniture>().CampfireItem.Type;

    public override string MapBackground => BackgroundPath;

    public override float GetWeight(Player player) {
        // Increase weight depending on how many tiles are nearby.
        var tileCount = ModContent.GetInstance<PollutedOceanSystem>();
        return tileCount.PollutedTileCount / (float)tileCount.PollutedTileThreshold;
    }

    public override bool IsBiomeActive(Player player) {
        return player.position.Y < Main.worldSurface * 16.0 && ModContent.GetInstance<PollutedOceanSystem>().CheckBiome(player);
    }

    public virtual void PostSetupContent(Mod mod) {
        BestiaryTags.Ocean.Add(this);
    }
}
