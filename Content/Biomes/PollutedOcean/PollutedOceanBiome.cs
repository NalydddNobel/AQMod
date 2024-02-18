using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;
using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using Aequus.Content.Tiles.Furniture.Trash;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiome : ModBiome {
    public static int BlockPresenceNeeded { get; set; } = 300;

    public static int BlockPresence { get; set; }

    public static Vector3 CavernLight { get; set; } = Color.Cyan.ToVector3();

    public override string BestiaryIcon => AequusTextures.PollutedOceanBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.MapBG.Path;
    public override string MapBackground => BackgroundPath;

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedOceanWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    private int? MusicSlotId;
    public override int Music => MusicSlotId ??= MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/PollutedOcean");

    public override int BiomeTorchItemType => ModContent.GetInstance<TrashTorch>().Item.Type;
    public override int BiomeCampfireItemType => ModContent.GetInstance<TrashTorch>().CampfireItem.Type;

    public override float GetWeight(Player player) {
        // Increase weight depending on how many tiles are nearby.
        return BlockPresence / (BlockPresenceNeeded * 2f);
    }

    public override bool IsBiomeActive(Player player) {
        return (player.position.Y > Main.worldSurface * 16.0 || WorldGen.oceanDepths((int)player.position.X / 16, (int)Main.worldSurface)) && BlockPresence >= BlockPresenceNeeded;
    }

    public static void PopulateSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();

        pool[ModContent.NPCType<OilSlime>()] = 1f;
        pool[ModContent.NPCType<Scavenger>()] = 1f;
        pool[NPCID.DarkCaster] = 0.33f; // Radio Conductor

        if (spawnInfo.Water) {
            pool[ModContent.NPCType<BlackJellyfish>()] = 1f;
            pool[NPCID.AnglerFish] = 1f; // Other Fish

            pool[NPCID.LightningBug] = 0.1f; // Sea Firefly
        }

        if (Main.hardMode) {
            pool[NPCID.AngryNimbus] = 0.33f; // Mirage
            pool[NPCID.MushiLadybug] = 0.33f; // Pillbug
        }

        pool[NPCID.Buggy] = 0.1f; // Chromite
        pool[NPCID.Sluggy] = 0.1f; // Horseshoe Crab
    }
}