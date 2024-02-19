using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Tiles.Furniture.Trash;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiomeSurface : ModBiome {
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedOceanWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

    public override int Music => PollutedOceanSystem.Music;

    public override int BiomeTorchItemType => ModContent.GetInstance<TrashTorch>().Item.Type;
    public override int BiomeCampfireItemType => ModContent.GetInstance<TrashTorch>().CampfireItem.Type;

    public override string MapBackground => BackgroundPath;

    public override float GetWeight(Player player) {
        // Increase weight depending on how many tiles are nearby.
        return PollutedOceanSystem.PollutedTileCount / (float)PollutedOceanSystem.PollutedTileThreshold;
    }

    public override bool IsBiomeActive(Player player) {
        return player.position.Y > Main.worldSurface * 16.0 && PollutedOceanSystem.CheckBiome(player);
    }

    public static void PopulateSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();

        pool[ModContent.NPCType<OilSlime>()] = 1f;

        if (spawnInfo.Water) {
            pool[NPCID.Arapaima] = 1f; // Eel
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            pool[NPCID.LightningBug] = 0.1f; // Sea Firefly
        }

        if (Main.hardMode) {
            pool[NPCID.AngryNimbus] = 0.33f; // Mirage
        }
    }
}
