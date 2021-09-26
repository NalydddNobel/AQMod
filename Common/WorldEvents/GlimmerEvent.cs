using AQMod.Common.Commands;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.WorldEvents
{
    public static class GlimmerEvent
    {
        public const ushort MaxDistance = 1650;
        public const ushort SuperStariteDistance = 1200;
        public const ushort HyperStariteDistance = 800;
        public const ushort UltraStariteDistance = 500;

        public const float SuperStariteSpawnMult = 0.75f;
        public const float HyperStariteSpawnMult = 0.4f;
        public const float UltraStariteSpawnMult = 0.2f;

        public const int GlimmerChanceMax = 300;

        public static bool ActuallyActive { get; set; }
        public static bool FakeActive { get; set; }
        public static bool IsActive => ActuallyActive || FakeActive;

        public static Color TextColor => new Color(238, 17, 68, 255);

        public static int _ultimateSwordChatTimer = 0;
        public static float _ultimateSwordOffsetY;

        public static bool CanShowInvasionProgress()
        {
            if (ActuallyActive && AQMod.OmegaStariteIndexCache == -1)
            {
                if (GetTileDistance(Main.LocalPlayer) < MaxDistance)
                    return true;
            }
            return false;
        }

        public static int GetTileDistance(Player player)
        {
            return (int)(Main.LocalPlayer.Center.X / 16 - X).Abs();
        }

        public static ushort X;
        public static ushort Y;
        public static int GlimmerChance;

        public static int DeactivationTimer = -1;

        public static void Activate(bool genuine = true)
        {
            ushort x = FindGlimmerPosition();
            Activate(x, getGlimmerPos(x));
        }

        public static void Activate(ushort x, ushort y, bool genuine = true)
        {
            if (genuine)
            {
                ActuallyActive = true;
                GlimmerChance = GlimmerChanceMax;
            }
            else
            {
                FakeActive = true;
            }
            AQMod.omegaStariteScene = 0;
            X = x;
            Y = y;
        }

        public static void Deactivate()
        {
            ActuallyActive = false;
            DeactivationTimer = -1;
            AQMod.omegaStariteScene = 0;
            if (!FakeActive)
            {
                X = 0;
                Y = 0;
            }
            if (AQNPC.StariteDisco)
            {
                AQNPC.StariteDisco = false;
                AQNPC.StariteProjectileColor = AQNPC.StariteProjectileColorOrig;
            }
        }

        private static ushort FindGlimmerPosition()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = Main.rand.Next(200, Main.maxTilesX - 200);
                if ((x - Main.spawnTileX).Abs() < 100)
                    continue;
                bool invaildSpot = false;
                for (int j = 0; j < Main.maxPlayers; j++)
                {
                    if (Main.player[j].active && !Main.player[j].dead)
                    {
                        int playerX = (int)(Main.player[j].position.X / 16);
                        if ((x - playerX).Abs() < SuperStariteDistance)
                        {
                            invaildSpot = true;
                            break;
                        }
                    }
                }
                if (!invaildSpot)
                    return (ushort)x;
            }
            return (ushort)Main.rand.Next(200, Main.maxTilesX - 200);
        }

        internal static ushort getGlimmerPos(int x)
        {
            for (ushort j = 180; j < Main.worldSurface; j++)
            {
                if (Framing.GetTileSafely(x, j).active() && Main.tileSolid[Main.tile[x, j].type] && !Main.tileSolidTop[Main.tile[x, j].type])
                    return j;
            }
            return (ushort)Main.worldSurface;
        }

        public static void UpdateWorld()
        {
            if (Main.dayTime || Main.pumpkinMoon || Main.snowMoon || GlimmerEventCommands.ForceGlimmerOff)
            {
                if (Main.dayTime)
                    FakeActive = false;
                if (IsActive)
                    Deactivate();
                return;
            }
            if (!ActuallyActive)
            {
                DeactivationTimer = -1;
                return;
            }
            if (DeactivationTimer > 0)
            {
                DeactivationTimer--;
                if (DeactivationTimer == 0)
                {
                    DeactivationTimer = -1;
                    AQMod.BroadcastMessage(AQText.GlimmerEventEnding().Value, TextColor);
                    Deactivate();
                }
            }
            if (Y == 0)
            {
                Y = getGlimmerPos(X);
            }
            else
            {
                if (!Framing.GetTileSafely(X, Y).active() || Framing.GetTileSafely(X, Y - 1).active())
                    Y = getGlimmerPos(X);
            }
        }

        public static void UpdateNight()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                GlimmerChance = Main.rand.Next(GlimmerChance);
                if (GlimmerChance == 0)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (player.active && player.statLifeMax > 100)
                        {
                            Activate();
                            AQMod.BroadcastMessage(AQText.GlimmerEventWarning().Value, TextColor);
                            break;
                        }
                    }
                }
            }
        }

        public static void ModifySpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!ActuallyActive || DeactivationTimer > 0)
                return;
            int poolCount = pool.Count;
            int tileDistance = (int)(spawnInfo.player.Center.X / 16 - X).Abs();
            if (tileDistance < MaxDistance && spawnInfo.player.position.Y < (Main.worldSurface + 50) * 16)
            {
                if (tileDistance > HyperStariteDistance) // shouldn't divide by 0...
                {
                    float normalSpawnsMult = 1f - 1f / (tileDistance - HyperStariteDistance);
                    IEnumerator<int> keys = pool.Keys.GetEnumerator();
                    int[] keyValue = new int[pool.Count];
                    for (int i = 0; i < poolCount; i++)
                    {
                        keyValue[i] = keys.Current;
                        if (!keys.MoveNext())
                        {
                            Array.Resize(ref keyValue, i);
                            break;
                        }
                    }
                    keys.Dispose();
                    for (int i = 0; i < poolCount; i++)
                    {
                        pool[keyValue[i]] *= normalSpawnsMult;
                    }
                }
                else
                {
                    int[] keyValue = new int[pool.Count];
                    IEnumerator<int> keys = pool.Keys.GetEnumerator();
                    for (int i = 0; i < poolCount; i++)
                    {
                        keyValue[i] = keys.Current;
                        if (!keys.MoveNext())
                        {
                            Array.Resize(ref keyValue, i);
                            break;
                        }
                    }
                    for (int i = 0; i < poolCount; i++)
                    {
                        pool[keyValue[i]] = 0f;
                    }
                }
                // it checks if {distance > HyperStariteDistance} twice, too lazy to do change that at the moment
                if (tileDistance > SuperStariteDistance)
                {
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.Starite>(), 1f - AQUtils.GetGrad(SuperStariteDistance, MaxDistance, tileDistance));
                }
                else if (tileDistance > HyperStariteDistance)
                {
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.Starite>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.SuperStarite>(), 1f - AQUtils.GetGrad(HyperStariteDistance, SuperStariteDistance, tileDistance) * SuperStariteSpawnMult);
                }
                else if (tileDistance > UltraStariteDistance)
                {
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.Starite>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.SuperStarite>(), SuperStariteSpawnMult);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.HyperStarite>(), 1f - AQUtils.GetGrad(UltraStariteDistance, HyperStariteDistance, tileDistance) * HyperStariteSpawnMult);
                }
                else if (tileDistance > 0)
                {
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.Starite>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.SuperStarite>(), SuperStariteSpawnMult);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.HyperStarite>(), HyperStariteSpawnMult);
                }
                else
                {
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.Starite>(), 1f);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.SuperStarite>(), SuperStariteSpawnMult);
                    pool.Add(ModContent.NPCType<NPCs.Glimmer.HyperStarite>(), HyperStariteSpawnMult);
                }
            }
        }
    }
}