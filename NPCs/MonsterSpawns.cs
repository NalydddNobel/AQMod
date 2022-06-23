using Aequus.Biomes;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.Sky;
using Aequus.NPCs.Monsters.Underworld;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Aequus.NPCs
{
    public class MonsterSpawns : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<AequusPlayer>().eventDemonSiege.X != 0)
            {
                spawnRate = 100;
                maxSpawns = 8;
                return;
            }
            if (player.Zen())
            {
                spawnRate *= 10000;
                maxSpawns = 0;
                return;
            }
            if (player.ZoneSkyHeight && GaleStreamsInvasion.TimeForMeteorSpawns())
            {
                spawnRate /= 2;
                maxSpawns *= 2;
            }
            if (player.GetModPlayer<AequusPlayer>().eventGaleStreams)
            {
                spawnRate /= 2;
            }
            if (IsClose<RedSprite>(player) || IsClose<SpaceSquid>(player))
            {
                spawnRate *= 3;
                maxSpawns = Math.Min(maxSpawns, 2);
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Aequus().eventDemonSiege.X != 0)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<TrapperImp>(), 0.33f);
                pool.Add(ModContent.NPCType<Cindera>(), 0.33f);
                pool.Add(ModContent.NPCType<Magmabubble>(), 0.33f);
                return;
            }
            if (DontAddNewSpawns(spawnInfo))
            {
                return;
            }
            if (spawnInfo.Player.ZoneSkyHeight && GaleStreamsInvasion.TimeForMeteorSpawns())
            {
                AdjustSpawns(pool, 0.75f);
                pool.Add(ModContent.NPCType<Meteor>(), 2f);
            }
            if (spawnInfo.Player.GetModPlayer<AequusPlayer>().eventGaleStreams && !spawnInfo.PlayerSafe)
            {
                AdjustSpawns(pool, MathHelper.Lerp(1f, 0.25f, SpawnCondition.Sky.Chance));
                if (Aequus.HardmodeTier && !(IsClose<RedSprite>(spawnInfo.Player) || IsClose<SpaceSquid>(spawnInfo.Player)))
                {
                    pool.Add(ModContent.NPCType<RedSprite>(), 0.06f * SpawnCondition.Sky.Chance);
                    pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f * SpawnCondition.Sky.Chance);
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<Vraine>()))
                    pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
                if (WorldGen.SolidTile(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
                {
                    pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f * SpawnCondition.Sky.Chance);
                }
                pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f * SpawnCondition.Sky.Chance);
            }
            else
            {
                if (!Main.dayTime && Main.bloodMoon && spawnInfo.SpawnTileY < Main.worldSurface)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<BloodMimic>()))
                    {
                        pool.Add(ModContent.NPCType<BloodMimic>(), 0.02f);
                    }
                }
            }
        }

        private void AdjustSpawns(IDictionary<int, float> pool, float amt)
        {
            var enumerator = pool.GetEnumerator();
            while (enumerator.MoveNext())
            {
                pool[enumerator.Current.Key] *= amt;
            }
        }

        public static bool IsClose<T>(Player player) where T : ModNPC
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<T>())
                {
                    if (Main.npc[i].Distance(player.Center) < 2000f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool DontAddNewSpawns(NPCSpawnInfo spawnInfo, bool checkPillars = true, bool checkMoonSunEvents = true, bool checkInvasion = true)
        {
            if (checkPillars && ArePillarsActiveAndInZone(spawnInfo))
            {
                return true;
            }
            if (checkMoonSunEvents && AreMoonInvasionsActive(spawnInfo))
            {
                return true;
            }
            if (checkInvasion && AreVanillaInvasionsActive(spawnInfo))
            {
                return true;
            }
            return false;
        }

        public static bool ArePillarsActiveAndInZone(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerStardust || spawnInfo.Player.ZoneTowerVortex;
        }
        public static bool AreMoonInvasionsActive(NPCSpawnInfo spawnInfo)
        {
            return (!spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneSkyHeight) || (!Main.eclipse && !Main.pumpkinMoon && !Main.snowMoon);
        }
        public static bool AreVanillaInvasionsActive(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneSkyHeight || Main.invasionType <= 0;
        }

        public static void ForceZen(Vector2 mySpot, float zenningDistance)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    Main.player[i].GetModPlayer<MonsterSpawnsPlayer>().forceZen = true;
                }
            }
        }
        public static void ForceZen(NPC npc)
        {
            ForceZen(npc.Center, 2000f);
        }
    }

    public class MonsterSpawnsPlayer : ModPlayer
    {
        public bool forceZen;

        public override void ResetEffects()
        {
            forceZen = false;
        }
    }
}