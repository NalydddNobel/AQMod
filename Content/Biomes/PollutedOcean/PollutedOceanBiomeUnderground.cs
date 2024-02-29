using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using Aequus.Content.Fishing;
using Aequus.Content.Fishing.Fish.BlackJellyfish;
using System.Collections.Generic;
using Terraria.DataStructures;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace Aequus.Content.Biomes.PollutedOcean;

/// <summary>Main class for general things related to the Polluted Ocean biome. Also counts as the Underground Polluted Ocean.</summary>
public class PollutedOceanBiomeUnderground : PollutedOceanBiomeSurface {
    public static Vector3 CavernLight { get; set; } = Color.Cyan.ToVector3();

    public override bool IsBiomeActive(Player player) {
        return player.position.Y > Main.worldSurface * 16.0 && PollutedOceanSystem.CheckBiome(player);
    }

    public static new void PopulateSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
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

    public static void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        if (attempt.rare && Main.rand.NextBool(5)) {
            itemDrop = ModContent.ItemType<BlackJellyfishBait>();
        }
        else if (attempt.common && Main.rand.NextBool()) {
            if (Main.rand.NextBool()) {
                itemDrop = FishInstantiator.Killifish.Type;
            }
            else {
                itemDrop = FishInstantiator.Piraiba.Type;
            }
        }
    }
}