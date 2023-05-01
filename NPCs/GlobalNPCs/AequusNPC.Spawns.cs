using Aequus.Content.Biomes;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Boss.RedSpriteMiniboss;
using Aequus.Content.Boss.SpaceSquidMiniboss;
using Aequus.Content.Boss.UltraStariteMiniboss;
using Aequus.Content.Events;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.NPCs.Critters;
using Aequus.Content.Town.SkyMerchantNPC;
using Aequus.NPCs.Monsters.Night;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.NPCs.Monsters.Sky;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.NPCs.Monsters.Underground;
using Aequus.NPCs.Monsters.Underworld;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Aequus.NPCs {
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
            return player.Aequus().ZoneGlimmer && player.townNPCs < 2f && GlimmerSystem.CalcTiles(player) > 100;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
            if (player.GetModPlayer<AequusPlayer>().ZoneDemonSiege) {
                spawnRate = Math.Min(spawnRate, 100);
                maxSpawns = Math.Max(maxSpawns, 7);
                return;
            }
            if (player.Aequus().forceZen) {
                player.Aequus().forceZen = false;
                spawnRate *= 10000;
                maxSpawns = 0;
                return;
            }
            var aequus = player.Aequus();
            if (player.InModBiome<FakeUnderworldBiome>()) {
                spawnRate /= 6;
            }
            spawnRate = (int)(spawnRate * aequus.spawnrateMultiplier);
            maxSpawns = (int)(maxSpawns / aequus.maxSpawnsDivider);
            if (player.ZoneSkyHeight) {
                if (!Main.dayTime && NPC.downedBoss2
                    && player.Center.InOuterThirds()
                    && !NPC.AnyNPCs(NPCID.MartianProbe)) {
                    spawnRate /= 2;
                    maxSpawns *= 2;
                }
                if (IsClose<SkyMerchant>(player)) {
                    spawnRate *= 3;
                    maxSpawns /= 3;
                }
            }
            if (aequus.ZoneGaleStreams) {
                spawnRate /= 2;
            }
            if (CanSpawnGlimmerEnemies(player)) {
                spawnRate /= 3;
            }
            else if (aequus.ZonePeacefulGlimmer) {
                spawnRate /= 2;
                maxSpawns /= 2;
            }
            if (aequus.ZoneCrabCrevice) {
                spawnRate /= 2;
                maxSpawns = (int)(maxSpawns * 1.25f);
            }
        }

        private static void GlimmerEnemies(int tiles, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (tiles < GlimmerBiomeManager.SuperStariteTile) {
                pool.Clear();
            }
            pool.Add(ModContent.NPCType<DwarfStarite>(), GlimmerBiomeManager.StariteSpawn);
            pool.Add(ModContent.NPCType<Starite>(), GlimmerBiomeManager.StariteSpawn);

            if (CanSpawnGlimmerEnemies(spawnInfo.Player)) {
                if (tiles < GlimmerBiomeManager.SuperStariteTile) {
                    pool.Add(ModContent.NPCType<SuperStarite>(), GlimmerBiomeManager.SuperStariteSpawn);
                }
                int hyperStariteCount = tiles < GlimmerBiomeManager.UltraStariteTile ? 2 : 1;
                if (tiles < GlimmerBiomeManager.HyperStariteTile) {
                    pool[ModContent.NPCType<Starite>()] *= 0.5f;
                    pool[ModContent.NPCType<SuperStarite>()] *= 0.75f;
                    if (NPC.CountNPCS(ModContent.NPCType<HyperStarite>()) < hyperStariteCount) {
                        pool.Add(ModContent.NPCType<HyperStarite>(), GlimmerBiomeManager.HyperStariteSpawn);
                    }
                }
                if (tiles < GlimmerBiomeManager.UltraStariteTile && !NPC.AnyNPCs(ModContent.NPCType<UltraStarite>())) {
                    pool[ModContent.NPCType<Starite>()] *= 0.5f;
                    pool[ModContent.NPCType<SuperStarite>()] *= 0.75f;
                    pool.Add(ModContent.NPCType<UltraStarite>(), GlimmerBiomeManager.UltraStariteSpawn);
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
            if (Aequus.HardmodeTier && !(IsClose<RedSprite>(spawnInfo.Player) || IsClose<SpaceSquid>(spawnInfo.Player))) {
                pool.Add(ModContent.NPCType<RedSprite>(), (!AequusWorld.downedRedSprite ? 0.2f : 0.06f) * SpawnCondition.Sky.Chance);
                pool.Add(ModContent.NPCType<SpaceSquid>(), (!AequusWorld.downedSpaceSquid ? 0.2f : 0.06f) * SpawnCondition.Sky.Chance);
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>()))
                pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
            if (WorldGen.SolidTile(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY)) {
                pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f * SpawnCondition.Sky.Chance);
            }
            pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f * SpawnCondition.Sky.Chance);
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (spawnInfo.Player.Aequus().ZoneDemonSiege) {
                DemonSiegeEnemies(pool, spawnInfo);
                return;
            }
            if (DontAddNewSpawns(spawnInfo)) {
                return;
            }
            bool surface = spawnInfo.SpawnTileY < Main.worldSurface;
            if (spawnInfo.Sky && !Main.dayTime && spawnInfo.Player.Center.InOuterThirds()) {
                AdjustSpawns(pool, 0.75f);
                if (GaleStreamsBiomeManager.IsThisSpace(spawnInfo.SpawnTileY * 16f))
                    pool.Add(ModContent.NPCType<Meteor>(), 2f);
            }
            if (spawnInfo.Player.GetModPlayer<AequusPlayer>().ZoneGaleStreams && !spawnInfo.PlayerSafe) {
                GaleStreamsEnemies(pool, spawnInfo);
            }
            if (!Main.dayTime && surface) {
                if (GlimmerBiomeManager.EventActive) {
                    int tiles = GlimmerSystem.CalcTiles(spawnInfo.Player);
                    if (tiles < GlimmerBiomeManager.MaxTiles) {
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
                    pool.Add(ModContent.NPCType<DwarfStarite>(), spawnInfo.Player.Aequus().ZonePeacefulGlimmer ? 3f : 0.01f);
                }
            }
            if (CrabCreviceBiome.SpawnCrabCreviceEnemies(pool, spawnInfo)) {
                return;
            }

            PreHardmodeMimics(pool, spawnInfo);
            TrapSkeleton.CheckSpawn(spawnInfo, pool);
            AddHardmodeTierEnemies(pool, spawnInfo);
        }

        private static void AddHardmodeTierEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (Main.hardMode || !Aequus.HardmodeTier) {
                return;
            }

            if (spawnInfo.Player.ZoneMarble) {
                pool[NPCID.Medusa] = 0.1f;
            }
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
}