using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Content.World.FallingStars;
using AQMod.Effects;
using AQMod.NPCs.Monsters.GlimmerMonsters;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.World.Events
{
    public sealed class Glimmer : WorldEvent
    {
        public class CustomProgressBar : EventProgressBar
        {
            public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "glimmerevent");
            public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.GlimmerEvent");
            public override Color NameBGColor => new Color(120, 20, 110, 128);
            public override float EventProgress => 1f - (float)GetTileDistanceUsingPlayer(Main.LocalPlayer) / Glimmer.MaxDistance;

            public override bool IsActive() => IsAbleToShowInvasionProgressBar();
            public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.EventProgress.GlimmerEvent", Glimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer));
        }
        public struct EnemyLayer
        {
            /// <summary>
            /// Distance away from the center in tiles
            /// </summary>
            public readonly ushort Distance;
            /// <summary>
            /// Anything below 1 means rarer spawns.
            /// </summary>
            public readonly float SpawnChance;
            /// <summary>
            /// The NPC that spawns in this layer.
            /// </summary>
            public readonly int NPCType;

            public EnemyLayer(int npc, ushort distance, float spawnChance)
            {
                NPCType = npc;
                Distance = distance;
                SpawnChance = spawnChance;
            }

            public EnemyLayer(int npc, ushort distance, float spawnChance, string texturePath)
            {
                NPCType = npc;
                Distance = distance;
                SpawnChance = spawnChance;
            }
        }

        public const ushort MaxDistance = 1650;
        public const ushort SuperStariteDistance = 1200;
        public const ushort HyperStariteDistance = 800;
        public const ushort UltraStariteDistance = 500;
        public const float StariteSpawnChance = 1f;
        public const float SuperStariteSpawnChance = 0.75f;
        public const float HyperStariteSpawnChance = 0.4f;
        public const float UltraStariteSpawnChance = 0.2f;

        internal static Color StariteProjectileColorOrig => new Color(200, 10, 255, 0);
        public static Color TextColor => new Color(238, 17, 68, 255);
        public static List<EnemyLayer> Layers { get; private set; }
        /// <summary>
        /// A cache of the most recently spawned Omega Starite npc. Use this only for visual effects please. Defaults to -1.
        /// </summary>
        public static short omegaStarite = -1;

        public static ushort tileX;
        public static ushort tileY;
        public static int deactivationDelay = -1;
        public static Color stariteProjectileColoring;
        public static bool stariteDiscoParty;
        public static bool renderUltimateSword;

        internal override EventProgressBar ProgressBar => new CustomProgressBar();

        protected override void Setup(AQMod mod)
        {
            Layers = new List<EnemyLayer>
            {
                new EnemyLayer(ModContent.NPCType<Starite>(), MaxDistance, StariteSpawnChance),
                new EnemyLayer(ModContent.NPCType<SuperStarite>(), SuperStariteDistance, SuperStariteSpawnChance),
                new EnemyLayer(ModContent.NPCType<HyperStarite>(), HyperStariteDistance, HyperStariteSpawnChance),
            };
        }

        public override void Initialize()
        {
            tileX = 0;
            tileY = 0;
            stariteDiscoParty = false;
            stariteProjectileColoring = StariteProjectileColorOrig;
            deactivationDelay = -1;
        }

        public override TagCompound Save()
        {
            if (deactivationDelay > 0)
                Deactivate();
            var tag = new TagCompound()
            {
                ["X"] = (int)tileX,
                ["Y"] = (int)tileY,
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            tileX = (ushort)tag.GetInt("X");
            tileY = (ushort)tag.GetInt("Y");

            if (!Main.dayTime)
                SkyGlimmerEvent.InitNight();
        }

        public override void PreUpdate()
        {
            if (PassingDays.OnTurnNight && PassingDays.daysPassedSinceLastGlimmerEvent > 4
                && Main.moonPhase != MoonPhases.FullMoon && !Main.bloodMoon && NPC.AnyNPCs(NPCID.Dryad))
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && player.statLifeMax > 200)
                    {
                        bool activateGlimmerEvent = WorldDefeats.DownedStarite
                            ? PassingDays.daysPassedSinceLastGlimmerEvent > 16 || Main.rand.NextBool(22 - PassingDays.daysPassedSinceLastGlimmerEvent)
                            : PassingDays.daysPassedSinceLastGlimmerEvent > 12 || Main.rand.NextBool(18 - PassingDays.daysPassedSinceLastGlimmerEvent);
                        if (activateGlimmerEvent && Activate())
                        {
                            PassingDays.daysPassedSinceLastGlimmerEvent = 0;
                            if (AQPlayer.IgnoreMoons())
                            {
                                CosmicanonWorldData.GlimmersPrevented++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetHelper.PreventedGlimmer();
                            }
                            else
                            {
                                AQMod.BroadcastMessage("Mods.AQMod.EventWarning.GlimmerEvent", TextColor);
                                if (Main.netMode == NetmodeID.Server)
                                    NetHelper.ActivateGlimmerEvent();
                            }
                        }
                        break;
                    }
                }
            }
        }

        public override void PostUpdate()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (Main.dayTime || Main.pumpkinMoon || Main.snowMoon)
            {
                if (IsGlimmerEventCurrentlyActive())
                    Deactivate();
                return;
            }
            if (!IsGlimmerEventCurrentlyActive())
            {
                deactivationDelay = -1;
                return;
            }

            if (Main.rand.Next(8000) < 10f * Main.maxTilesX / 4200f)
            {
                ImitatedFallingStars.CastFallingStar();
            }

            if (deactivationDelay > 0)
            {
                deactivationDelay--;
                if (deactivationDelay == 0)
                {
                    deactivationDelay = -1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        AQMod.BroadcastMessage("Mods.AQMod.EventEnding.GlimmerEvent", TextColor);
                        NetMessage.SendData(MessageID.WorldData);
                    }
                    Deactivate();
                }
            }
            if (tileY == 0 || tileY == (ushort)Main.worldSurface)
            {
                ManageGlimmerEventYCoordinate();
            }
            else
            {
                if (!Framing.GetTileSafely(tileX, tileY).active() || Framing.GetTileSafely(tileX, tileY - 1).active())
                    ManageGlimmerEventYCoordinate();
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(IsGlimmerEventCurrentlyActive());
            if (IsGlimmerEventCurrentlyActive())
            {
                writer.Write(tileX);
                writer.Write(tileY);
            }
            writer.Write(stariteDiscoParty);
            writer.Write(deactivationDelay);
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                tileX = reader.ReadUInt16();
                tileY = reader.ReadUInt16();
            }
            else
            {
                tileX = 0;
                tileY = 0;
            }
            stariteDiscoParty = reader.ReadBoolean();
            deactivationDelay = reader.ReadInt32();
        }

        public static bool IsGlimmerEventCurrentlyActive()
        {
            return tileX != 0;
        }

        public static bool AreStariteSpawnsCurrentlyActive(Player player)
        {
            return IsGlimmerEventCurrentlyActive() && deactivationDelay <= 0 && player.position.Y < (Main.worldSurface + 50) * 16;
        }

        public static bool IsAbleToShowInvasionProgressBar()
        {
            if (AreStariteSpawnsCurrentlyActive(Main.LocalPlayer) && omegaStarite == -1)
            {
                if (GetTileDistanceUsingPlayer(Main.LocalPlayer) < MaxDistance)
                    return true;
            }
            return false;
        }

        public static byte GetLayerIndexThroughTileDistance(int tileDistance)
        {
            byte layerIndex = 255;
            for (byte i = (byte)(Layers.Count - 1); i >= 0; i--)
            {
                if (tileDistance < Layers[i].Distance)
                {
                    layerIndex = i;
                    break;
                }
            }
            return layerIndex;
        }

        public static void ManageGlimmerEventYCoordinate()
        {
            for (ushort j = 180; j < Main.worldSurface; j++)
            {
                if (Framing.GetTileSafely(tileX, j).active() && Main.tileSolid[Main.tile[tileX, j].type] && !Main.tileSolidTop[Main.tile[tileX, j].type])
                {
                    tileY = j;
                    return;
                }
            }
            tileY = (ushort)Main.worldSurface;
        }

        public static int GetTileDistanceUsingPlayer(Player player)
        {
            return (int)((player.position.X + player.width) / 16 - tileX).Abs();
        }

        public static bool Activate()
        {
            var statuePlacements = new List<TEGlimmeringStatue>();
            foreach (var t in TileEntity.ByID)
            {
                if (t.Value is TEGlimmeringStatue statue && statue.Position.Y < Main.worldSurface)
                    statuePlacements.Add(statue);
            }
            if (statuePlacements.Count > 0 && Main.rand.Next(100) + 1 < 95)
            {
                if (statuePlacements.Count == 1)
                {
                    ActivateAtCoordinates(statuePlacements[0].Position.X, statuePlacements[0].Position.Y);
                }
                else
                {
                    int randIndex = Main.rand.Next(statuePlacements.Count);
                    ActivateAtCoordinates(statuePlacements[randIndex].Position.X, statuePlacements[randIndex].Position.Y);
                }
                return true;
            }
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
                {
                    tileX = (ushort)x;
                    ManageGlimmerEventYCoordinate();
                    return true;
                }
            }
            return false;
        }

        public static void ActivateAtCoordinates(int x, int y)
        {
            renderUltimateSword = true;
            tileX = (ushort)x;
            tileY = (ushort)y;
        }

        public static void Deactivate()
        {
            deactivationDelay = -1;
            tileX = 0;
            tileY = 0;
            if (stariteDiscoParty)
            {
                stariteDiscoParty = false;
                stariteProjectileColoring = StariteProjectileColorOrig;
            }
        }
    }
}