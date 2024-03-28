using Aequus.Old.Content.Enemies.BloodMoon;
using Aequus.Old.Content.Events.DemonSiege;
using Aequus.Old.Content.Events.Glimmer;
using System.Collections.Generic;

namespace Aequus.Old.Common;

public class LegacyEnemySpawns : GlobalNPC {
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (spawnInfo.Player.InModBiome<DemonSiegeZone>()) {
            DemonSiegeZone.AddEnemies(pool, spawnInfo);
            return;
        }

        // Determine if you can actually edit the spawn pool
        // Prevents aequus enemies from improperly spawning in the Dungeon, Temple,
        // or during invasions and events
        if (CanEditSpawnPool(spawnInfo)) {
            return;
        }

        bool surface = spawnInfo.SpawnTileY < Main.worldSurface;

        // Meteor Spawns
        //if (spawnInfo.Sky && !Main.dayTime && spawnInfo.Player.Center.InOuterX(outer: 3)) {
        //    AdjustSpawns(pool, 0.75f);
        //    if (Helper.ZoneSkyHeight(spawnInfo.SpawnTileY)) {
        //        pool.Add(ModContent.NPCType<Meteor>(), 2f);
        //    }
        //}

        // Gale Streams Spawns
        //if (spawnInfo.Player.InModBiome<GaleStreamsZone>() && !spawnInfo.PlayerSafe) {
        //    GaleStreamsEnemies(pool, spawnInfo);
        //}

        if (!Main.dayTime && surface) {
            if (GlimmerZone.EventActive) {
                int tiles = GlimmerSystem.GetTileDistance(spawnInfo.Player);
                if (tiles < GlimmerZone.MaxTiles) {
                    GlimmerZone.AddEnemies(tiles, pool, spawnInfo);
                    return;
                }
            }

            if (Main.bloodMoon) {
                if (!NPC.AnyNPCs(ModContent.NPCType<BloodMimic>())) {
                    pool.Add(ModContent.NPCType<BloodMimic>(), 0.01f);
                }
            }
            //else {
            //    pool.Add(ModContent.NPCType<DwarfStarite>(), spawnInfo.Player.InModBiome<PeacefulGlimmerZone>() ? 3f : 0.01f);
            //}
        }

        if (!Main.remixWorld || Main.rand.NextBool()) {
            return;
        }

        // Normal Spawns

        //PreHardmodeMimics(pool, spawnInfo);
        //TrapSkeleton.CheckSpawn(spawnInfo, pool);
        //AddHardmodeTierEnemies(pool, spawnInfo);
    }

    public static bool CanEditSpawnPool(NPCSpawnInfo spawnInfo) {
        if (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight) {
            if (Main.eclipse || Main.pumpkinMoon || Main.snowMoon) {
                return true;
            }
            if (spawnInfo.Invasion) {
                return true;
            }
        }
        return spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerStardust || spawnInfo.Player.ZoneTowerVortex
            || spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneLihzhardTemple;
    }
}
