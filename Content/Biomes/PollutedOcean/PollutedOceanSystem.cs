using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.BreadOfCthulhu;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using Aequus.Content.Fishing;
using Aequus.Content.Fishing.Fish.BlackJellyfish;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static List<int> BiomeTiles { get; private set; } = new();

    public static int PollutedTileThreshold { get; set; } = 800;
    public static int PollutedTileMax { get; set; } = 300;
    public static int PollutedTileCount { get; set; }

    private static int? _music;
    public static int Music => _music ??= MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/PollutedOcean");

    public override void Unload() {
        BiomeTiles.Clear();
        _music = null;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        PollutedTileCount = 0;
        foreach (var tile in BiomeTiles) {
            PollutedTileCount += tileCounts[tile];
        }
    }

    public static bool CheckBiome(Player player) {
        return WorldGen.oceanDepths((int)player.position.X / 16, (int)Main.worldSurface) && PollutedTileCount >= PollutedTileMax;
    }

    public static void PopulateSurfaceSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool[0] *= 0.5f;

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

    public static void PopulateUndergroundSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();

        pool[ModContent.NPCType<OilSlime>()] = 1f;
        pool[ModContent.NPCType<Scavenger>()] = 1f;
        pool[NPCID.DarkCaster] = 0.33f; // Radio Conductor

        if (spawnInfo.Water) {
            pool[ModContent.NPCType<BlackJellyfish>()] = 1f;
            pool[NPCID.Arapaima] = 1f; // Eel
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            pool[NPCID.LightningBug] = 0.1f; // Sea Firefly
        }

        if (Main.hardMode) {
            pool[NPCID.MushiLadybug] = 0.33f; // Pillbug
        }

        pool[NPCID.Buggy] = 0.1f; // Chromite
        pool[NPCID.Sluggy] = 0.1f; // Horseshoe Crab
    }

    public static void CatchSurfaceFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        CatchCommonFish(in attempt, ref itemDrop, ref npcSpawn);
    }

    public static void CatchUndergroundFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        if (attempt.rare && Main.rand.NextBool(5)) {
            itemDrop = ModContent.ItemType<BlackJellyfishBait>();
            return;
        }

        CatchCommonFish(in attempt, ref itemDrop, ref npcSpawn);
    }

    private static void CatchCommonFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        if (attempt.common && Main.rand.NextBool()) {
            if (Main.rand.NextBool()) {
                itemDrop = FishInstantiator.Killifish.Type;
            }
            else {
                itemDrop = FishInstantiator.Piraiba.Type;
            }
        }

        int chance = BreadOfCthulhu.GetFishingChance(in attempt);
        if (Main.rand.NextBool(chance)) {
            npcSpawn = ModContent.NPCType<BreadOfCthulhu>();
        }
    }
}