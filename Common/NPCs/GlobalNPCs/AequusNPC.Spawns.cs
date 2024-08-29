using Aequus.Common.Preferences;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Events.GaleStreams;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using Aequus.NPCs.Critters;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.BloodMoon;
using Aequus.NPCs.Monsters.DemonSiege;
using Aequus.NPCs.Monsters.GaleStreams;
using Aequus.NPCs.Monsters.Glimmer;
using Aequus.NPCs.Monsters.Glimmer.UltraStarite;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.Utilities;

namespace Aequus;

public partial class AequusNPC {
    /// <summary>
    /// Parameters for whether or not certain locations are valid spots for regular entities to spawn.
    /// </summary>
    public struct ValidSpawnParameters {
        /// <summary>
        /// Set to true to allow spawning during the Pillar event. (<see cref="Player.ZoneTowerSolar"/> / <see cref="Player.ZoneTowerVortex"/> / <see cref="Player.ZoneTowerNebula"/> / <see cref="Player.ZoneTowerStardust"/>)
        /// </summary>
        public bool Pillars;
        /// <summary>
        /// Set to true to allow spawning during the Eclipse, Pumpkin Moon, or Frost Moon (<see cref="Main.eclipse"/> / <see cref="Main.pumpkinMoon"/> / <see cref="Main.snowMoon"/>)
        /// </summary>
        public bool SunMoonEvents;
        /// <summary>
        /// Set to true to allow spawning during invasions (<see cref="Main.invasionType"/> / <see cref="NPCSpawnInfo.Invasion"/>)
        /// </summary>
        public bool Invasion;
        /// <summary>
        /// Set to true to allow spawning while the player is inside of the Dungeon or Lihzahrd Temple (<see cref="Player.ZoneDungeon"/> / <see cref="Player.ZoneLihzhardTemple"/>)
        /// </summary>
        public bool DungeonTemple;
    }

    public static bool CanSpawnGlimmerEnemies(Player player) {
        return player.InModBiome<GlimmerZone>() && player.townNPCs < 2f && GlimmerSystem.GetTileDistance(player) > 100;
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
        if (player.InModBiome<DemonSiegeZone>()) {
            spawnRate = Math.Min(spawnRate, 50);
            maxSpawns = Math.Max(maxSpawns, 7);
            return;
        }
        if (player.Aequus().forceZen) {
            player.Aequus().forceZen = false;
            spawnRate = 1000000;
            maxSpawns = 0;
            return;
        }
        var aequus = player.Aequus();
        spawnRate = (int)(spawnRate * aequus.spawnrateMultiplier);
        maxSpawns = (int)(maxSpawns / aequus.maxSpawnsDivider);
        if (player.ZoneSkyHeight) {
            if (!Main.dayTime && NPC.downedBoss2
                && player.Center.InOuterX()
                && !NPC.AnyNPCs(NPCID.MartianProbe)) {
                spawnRate /= 2;
                maxSpawns *= 2;
            }
        }
        if (player.InModBiome<GaleStreamsZone>()) {
            spawnRate /= 2;
        }
        if (CanSpawnGlimmerEnemies(player)) {
            spawnRate /= 3;
        }
        else if (player.InModBiome<PeacefulGlimmerZone>()) {
            spawnRate /= 2;
            maxSpawns /= 2;
        }
        if (player.InModBiome<CrabCreviceBiome>()) {
            spawnRate /= 2;
            maxSpawns = (int)(maxSpawns * 1.25f);
        }
    }

    private static void GlimmerEnemies(int tiles, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (tiles < GlimmerZone.SuperStariteTile) {
            pool.Clear();
        }
        pool.Add(ModContent.NPCType<DwarfStarite>(), GlimmerZone.StariteSpawn);
        pool.Add(ModContent.NPCType<Starite>(), GlimmerZone.StariteSpawn);

        if (CanSpawnGlimmerEnemies(spawnInfo.Player)) {
            if (tiles < GlimmerZone.SuperStariteTile) {
                pool.Add(ModContent.NPCType<SuperStarite>(), GlimmerZone.SuperStariteSpawn);
            }
            int hyperStariteCount = tiles < GlimmerZone.UltraStariteTile ? 2 : 1;
            if (tiles < GlimmerZone.HyperStariteTile) {
                pool[ModContent.NPCType<Starite>()] *= 0.5f;
                pool[ModContent.NPCType<SuperStarite>()] *= 0.75f;
                if (NPC.CountNPCS(ModContent.NPCType<HyperStarite>()) < hyperStariteCount) {
                    pool.Add(ModContent.NPCType<HyperStarite>(), GlimmerZone.HyperStariteSpawn);
                }
            }
            if (tiles < GlimmerZone.UltraStariteTile && !NPC.AnyNPCs(ModContent.NPCType<UltraStarite>())) {
                pool[ModContent.NPCType<Starite>()] *= 0.5f;
                pool[ModContent.NPCType<SuperStarite>()] *= 0.75f;
                pool.Add(ModContent.NPCType<UltraStarite>(), GlimmerZone.UltraStariteSpawn);
                if (AequusWorld.downedUltraStarite) {
                    pool[ModContent.NPCType<UltraStarite>()] *= 0.4f;
                }
                else {
                    pool[ModContent.NPCType<UltraStarite>()] *= 2f;
                }
            }
        }
    }
    private static void DemonSiegeEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();
        pool.Add(ModContent.NPCType<TrapperImp>(), 0.33f);
        pool.Add(ModContent.NPCType<Cindera>(), 0.33f);
        pool.Add(ModContent.NPCType<Magmabubble>(), 0.33f);
    }
    private static void GaleStreamsEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        AdjustSpawns(pool, MathHelper.Lerp(1f, 0.25f, SpawnCondition.Sky.Chance));
        if (Aequus.MediumMode && !(IsClose<NPCs.RedSprite.RedSprite>(spawnInfo.Player) || IsClose<NPCs.SpaceSquid.SpaceSquid>(spawnInfo.Player))) {
            pool.Add(ModContent.NPCType<NPCs.RedSprite.RedSprite>(), (!AequusWorld.downedRedSprite ? 0.2f : 0.06f) * SpawnCondition.Sky.Chance);
            pool.Add(ModContent.NPCType<NPCs.SpaceSquid.SpaceSquid>(), (!AequusWorld.downedSpaceSquid ? 0.2f : 0.06f) * SpawnCondition.Sky.Chance);
        }

        if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>())) {
            pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
        }
        if (WorldGen.SolidTile(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY)) {
            pool.Add(ModContent.NPCType<WhiteSlime>(), 1f * SpawnCondition.Sky.Chance);
        }
        pool.Add(ModContent.NPCType<StreamingBalloon>(), 1f * SpawnCondition.Sky.Chance);
    }
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (spawnInfo.Player.InModBiome<DemonSiegeZone>()) {
            DemonSiegeEnemies(pool, spawnInfo);
            return;
        }
        if (DontAddNewSpawns(spawnInfo)) {
            return;
        }
        bool surface = spawnInfo.SpawnTileY < Main.worldSurface;
        if (spawnInfo.Sky && !Main.dayTime && spawnInfo.Player.Center.InOuterX(outer: 3)) {
            AdjustSpawns(pool, 0.75f);
            if (Helper.ZoneSkyHeight(spawnInfo.SpawnTileY)) {
                pool.Add(ModContent.NPCType<Meteor>(), 2f);
            }
        }

        if (spawnInfo.Player.InModBiome<GaleStreamsZone>() && !spawnInfo.PlayerSafe) {
            GaleStreamsEnemies(pool, spawnInfo);
        }

        if (!Main.dayTime && surface) {
            if (GlimmerZone.EventActive) {
                int tiles = GlimmerSystem.GetTileDistance(spawnInfo.Player);
                if (tiles < GlimmerZone.MaxTiles) {
                    GlimmerEnemies(tiles, pool, spawnInfo);
                    return;
                }
            }

            if (Main.bloodMoon) {
                if (!NPC.AnyNPCs(ModContent.NPCType<BloodMimic>())) {
                    pool.Add(ModContent.NPCType<BloodMimic>(), 0.01f);
                }
            }
            else {
                pool.Add(ModContent.NPCType<DwarfStarite>(), spawnInfo.Player.InModBiome<PeacefulGlimmerZone>() ? 3f : 0.01f);
            }
        }
        if ((!Main.remixWorld || Main.rand.NextBool()) && CrabCreviceBiome.SpawnCrabCreviceEnemies(pool, spawnInfo)) {
            return;
        }

        TrapSkeleton.CheckSpawn(spawnInfo, pool);
        AddHardmodeTierEnemies(pool, spawnInfo);
    }

    private static void AddHardmodeTierEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (Main.hardMode || !Aequus.MediumMode) {
            return;
        }

        if (GameplayConfig.Instance.EarlyHardmodeMonsters) {
            if (spawnInfo.Player.ZoneMarble) {
                pool[NPCID.Medusa] = 0.1f;
            }
        }

        if (GameplayConfig.Instance.EarlyHardmodeVillagers) {
            if (spawnInfo.Player.ZoneRockLayerHeight) {
                if (!NPC.savedWizard && !NPC.AnyNPCs(NPCID.Wizard)) {
                    pool[NPCID.BoundWizard] = 0.01f;
                }
            }
            if (spawnInfo.Player.ZoneUnderworldHeight) {
                if (!NPC.savedTaxCollector && !NPC.AnyNPCs(NPCID.DemonTaxCollector)) {
                    pool[NPCID.DemonTaxCollector] = 0.05f;
                }
            }
        }
    }

    #region Helpers
    private static void AdjustSpawns(IDictionary<int, float> pool, float amt) {
        var enumerator = pool.GetEnumerator();
        while (enumerator.MoveNext()) {
            pool[enumerator.Current.Key] *= amt;
        }
    }

    public static bool IsClose<T>(Player player) where T : ModNPC {
        return player.isNearNPC(ModContent.NPCType<T>(), 2000f);
    }

    public static bool DontAddNewSpawns(NPCSpawnInfo spawnInfo, ValidSpawnParameters valid = default(ValidSpawnParameters)) {
        if (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneSkyHeight) {
            if (!valid.SunMoonEvents && (Main.eclipse || Main.pumpkinMoon || Main.snowMoon)) {
                return true;
            }
            if (!valid.Invasion && spawnInfo.Invasion) {
                return true;
            }
        }
        return (!valid.Pillars && (spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerStardust || spawnInfo.Player.ZoneTowerVortex))
            || (!valid.DungeonTemple && (spawnInfo.Player.ZoneDungeon || spawnInfo.Player.ZoneLihzhardTemple));
    }

    public static void ForceZen(Vector2 mySpot, float zenningDistance) {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Vector2.Distance(mySpot, Main.player[i].Center) < 2000f) {
                Main.player[i].GetModPlayer<AequusPlayer>().forceZen = true;
            }
        }
    }
    public static void ForceZen(NPC npc) {
        ForceZen(npc.Center, 2000f);
    }
    #endregion
}

public class SpawnsManagerSystem : ModSystem {
    public struct MagicPlayerMover {
        public Player player;
        public Vector2 previousLocation;
    }
    private static List<MagicPlayerMover> enemySpawnManipulators;

    public static void PreCheckCreatureSpawns() {
        enemySpawnManipulators.Clear();
        for (int i = 0; i < Main.maxPlayers; i++) {
            var location = Main.player[i].position;
            if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Aequus().PreCreatureSpawns()) {
                enemySpawnManipulators.Add(new MagicPlayerMover() { player = Main.player[i], previousLocation = location, });
            }
        }
    }

    public static void PostCheckCreatureSpawns() {
        foreach (var plr in enemySpawnManipulators) {
            plr.player.position = plr.previousLocation;
        }
        enemySpawnManipulators.Clear();
        AequusNPC.spawnNPCYOffset = 0f;
    }

    public override void Load() {
        enemySpawnManipulators = new List<MagicPlayerMover>();
    }

    public override void Unload() {
        enemySpawnManipulators = null;
    }

    public override void OnWorldLoad() {
        enemySpawnManipulators.Clear();
    }

    public override void OnWorldUnload() {
        enemySpawnManipulators.Clear();
    }
}