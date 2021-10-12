using AQMod.Common;
using AQMod.Common.Commands;
using AQMod.Common.Utilities;
using AQMod.Localization;
using AQMod.NPCs.Monsters.CosmicEvent;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.Glimmer
{
    public sealed class GlimmerEvent
    {
        public const ushort MaxDistance = 1650;
        public const ushort SuperStariteDistance = 1200;
        public const ushort HyperStariteDistance = 800;
        public const ushort UltraStariteDistance = 500;
        public const float StariteSpawnChance = 1f;
        public const float SuperStariteSpawnChance = 0.75f;
        public const float HyperStariteSpawnChance = 0.4f;
        public const float UltraStariteSpawnChance = 0.2f;
        public const int EventBaseRarity = 300;
        public const int GlimmerDownedRarityAdd = 150;
        public const int HardmodeRarityAdd = 150;

        internal static Color StariteProjectileColorOrig => new Color(200, 10, 255, 0);
        public static Color TextColor => new Color(238, 17, 68, 255);
        /// <summary>
        /// A list of the Glimmer Event Layers, do not cache this list please.
        /// </summary>
        public static List<GlimmerEventLayer> Layers { get; private set; }

        public bool IsActive => tileX != 0;
        public ushort tileX;
        public ushort tileY;
        public int spawnChance;
        public int deactivationTimer = -1;
        internal Color stariteProjectileColor;
        public bool StariteDisco { get; internal set; }

        internal static void Setup()
        {
            Layers = new List<GlimmerEventLayer>
            {
                new GlimmerEventLayer(ModContent.NPCType<Starite>(), MaxDistance, StariteSpawnChance),
                new GlimmerEventLayer(ModContent.NPCType<SuperStarite>(), SuperStariteDistance, SuperStariteSpawnChance),
                new GlimmerEventLayer(ModContent.NPCType<HyperStarite>(), HyperStariteDistance, HyperStariteSpawnChance),
            };
            //_sortTest();
        }

        private static void _sortTest()
        {
            SortLayers();
            for (int i = 0; i < Layers.Count; i++)
            {
                GlimmerEventLayer l = Layers[i];
                AQMod.Instance.Logger.Debug(i + ": " + l.Distance);
            }
        }

        public static int GetLayerIndex(int tileDistance)
        {
            int layerIndex = -1;
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                if (tileDistance < Layers[i].Distance)
                {
                    layerIndex = i;
                    break;
                }
            }
            return layerIndex;
        }

        public static void SortLayers()
        {
            Layers.Sort((l, l2) => l.Distance.CompareTo(l2.Distance));
            var list = new List<GlimmerEventLayer>();
            for (int i = Layers.Count - 1; i >= 0; i--) // reverses the order of the layers
            {
                list.Add(Layers[i]);
            }
            Layers = list;
        }

        public static int GetBaseRarity()
        {
            int rarity = EventBaseRarity;
            if (Main.hardMode)
            {
                rarity += HardmodeRarityAdd;
            }
            if (WorldDefeats.DownedGlimmer)
            {
                rarity += GlimmerDownedRarityAdd;
            }
            return rarity;
        }

        internal void Init()
        {
            tileX = 0;
            tileY = 0;
            spawnChance = -1;
            StariteDisco = false;
            stariteProjectileColor = StariteProjectileColorOrig;
            deactivationTimer = -1;
        }

        /// <summary>
        /// Whether or not the invasion progress for the Glimmer Event can be shown
        /// </summary>
        /// <returns></returns>
        public bool CanShowInvasionProgress()
        {
            if (IsActive && OmegaStariteSceneManager.OmegaStariteIndexCache == -1)
            {
                if (GetTileDistance(Main.LocalPlayer) < MaxDistance)
                    return true;
            }
            return false;
        }

        public bool Activate()
        {
            List<TEGlimmeringStatue> statuePlacements = new List<TEGlimmeringStatue>();
            foreach (var t in TileEntity.ByID)
            {
                if (t.Value is TEGlimmeringStatue statue && statue.Position.Y < Main.worldSurface)
                {
                    statuePlacements.Add(statue);
                }
            }
            if (statuePlacements.Count > 0 && (Main.rand.Next(100) + 1) < 95)
            {
                if (statuePlacements.Count == 1)
                {
                    Activate(statuePlacements[0].Position.X, statuePlacements[0].Position.Y);
                }
                else
                {
                    int randIndex = Main.rand.Next(statuePlacements.Count);
                    Activate(statuePlacements[randIndex].Position.X, statuePlacements[randIndex].Position.Y);
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
                    tileY = ManageGlimmerY();
                    return true;
                }
            }
            return false;
        }

        public void Activate(int x, int y)
        {
            OmegaStariteSceneManager.Scene = 0;
            spawnChance = GetBaseRarity();
            tileX = (ushort)x;
            tileY = (ushort)y;
        }

        public void Deactivate()
        {
            deactivationTimer = -1;
            OmegaStariteSceneManager.Scene = 0;
                tileX = 0;
                tileY = 0;
            if (StariteDisco)
            {
                StariteDisco = false;
                stariteProjectileColor = StariteProjectileColorOrig;
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

        internal ushort ManageGlimmerY()
        {
            for (ushort j = 180; j < Main.worldSurface; j++)
            {
                if (Framing.GetTileSafely(tileX, j).active() && Main.tileSolid[Main.tile[tileX, j].type] && !Main.tileSolidTop[Main.tile[tileX, j].type])
                    return j;
            }
            return (ushort)Main.worldSurface;
        }

        public int GetTileDistance(Player player)
        {
            return (int)(((player.position.X + player.width) / 16) - tileX).Abs();
        }

        public void UpdateWorld()
        {
            if (Main.dayTime || Main.pumpkinMoon || Main.snowMoon || GlimmerEventCommands.ForceGlimmerOff)
            {
                if (IsActive)
                    Deactivate();
                return;
            }
            if (!IsActive)
            {
                deactivationTimer = -1;
                return;
            }
            if (deactivationTimer > 0)
            {
                deactivationTimer--;
                if (deactivationTimer == 0)
                {
                    deactivationTimer = -1;
                    AQMod.BroadcastMessage(AQText.GlimmerEventEnding().Value, TextColor);
                    Deactivate();
                }
            }
            if (tileY == 0)
            {
                tileY = ManageGlimmerY();
            }
            else
            {
                if (!Framing.GetTileSafely(tileX, tileY).active() || Framing.GetTileSafely(tileX, tileY - 1).active())
                    tileY = ManageGlimmerY();
            }
        }

        public void OnTurnNight()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            if (Main.moonPhase != MoonPhaseID.FullMoon && NPC.AnyNPCs(NPCID.Dryad))
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && player.statLifeMax > 200)
                    {
                        if (spawnChance > 0)
                            spawnChance = Main.rand.Next(spawnChance);
                        if (spawnChance <= 0 && Activate())
                        {
                            AQMod.BroadcastMessage(AQText.GlimmerEventWarning().Value, TextColor);
                            Networking.GlimmerEventNetUpdate();
                        }
                        break;
                    }
                }
            }
        }
    }
}