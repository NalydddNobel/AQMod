using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;
using Aequus.Content.Critters.HorseshoeCrab;
using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiome : ModBiome {
    public static int BlockPresenceNeeded { get; set; } = 300;

    public static int BlockPresence { get; set; }

    public override string BestiaryIcon => AequusTextures.PollutedOceanBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.MapBG.Path;
    public override string MapBackground => BackgroundPath;

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedOceanWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    private int? MusicSlotId;
    public override int Music => MusicSlotId ??= MusicLoader.GetMusicSlot(Mod, AequusSounds.PollutedOcean.ModPath());

    public override bool IsBiomeActive(Player player) {
        return (player.position.Y > Main.worldSurface * 16.0 || WorldGen.oceanDepths((int)player.position.X / 16, (int)Main.worldSurface)) && BlockPresence >= BlockPresenceNeeded;
    }

    public static void PopulateSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();

        float horseshoeCrabRate = 1f;
        foreach (var pair in HorseshoeCrabInitializer.HorseshoeCrabs) {
            pool[pair.Item1.Type] = horseshoeCrabRate;
            pool[pair.Item2.Type] = 1f / NPC.goldCritterChance * horseshoeCrabRate;
        }
        
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
    }
}