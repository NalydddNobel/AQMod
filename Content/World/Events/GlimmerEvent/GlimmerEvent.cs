using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Common.NetCode;
using AQMod.Content.World.Events.ProgressBars;
using AQMod.Items.BossItems.Starite;
using AQMod.Localization;
using AQMod.NPCs.Monsters.GlimmerEvent;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events.GlimmerEvent
{
    public sealed class GlimmerEvent : WorldEvent
    {
        public const ushort MaxDistance = 1650;
        public const ushort SuperStariteDistance = 1200;
        public const ushort HyperStariteDistance = 800;
        public const ushort UltraStariteDistance = 500;
        public const float StariteSpawnChance = 1f;
        public const float SuperStariteSpawnChance = 0.75f;
        public const float HyperStariteSpawnChance = 0.4f;
        public const float UltraStariteSpawnChance = 0.2f;
        public const int EventBaseRarity = 200;
        public const int GlimmerDownedRarityAdd = 250;
        public const int HardmodeRarityAdd = 150;

        internal static Color StariteProjectileColorOrig => new Color(200, 10, 255, 0);
        public static Color TextColor => new Color(238, 17, 68, 255);
        public static List<GlimmerEventLayer> Layers { get; private set; }

        public static bool IsActive => tileX != 0;
        public static ushort tileX;
        public static ushort tileY;
        public static int spawnChance;
        public static int deactivationTimer = -1;
        public static Color stariteProjectileColor { get; internal set; }
        public static bool StariteDisco { get; internal set; }

        internal override EventEntry? BossChecklistEntry => new EventEntry(
            () => WorldDefeats.DownedGlimmer,
            2.1f,
            new List<int>() {
                ModContent.NPCType<Starite>(),
                ModContent.NPCType<SuperStarite>(),
                ModContent.NPCType<HyperStarite>(),
            },
            AQText.chooselocalizationtext("Glimmer Event", "微光事件"),
            ModContent.ItemType<MythicStarfruit>(),
            new List<int>()
            {
                ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>(),
                ItemID.Nazar,
                ModContent.ItemType<Items.Tools.MapMarkers.RetroGoggles>(),
                ModContent.ItemType<Items.Weapons.Ranged.SpaceShot>(),
                ModContent.ItemType<Items.Weapons.Summon.StariteStaff>(),
                ModContent.ItemType<Items.Accessories.MoonShoes>(),
                ModContent.ItemType<Items.Accessories.Ultranium>(),
            },
            new List<int>()
            {
                ModContent.ItemType<Items.Vanities.CelesitalEightBall>(),
                ModContent.ItemType<Items.Vanities.Dyes.HypnoDye>(),
                ModContent.ItemType<Items.Vanities.Dyes.OutlineDye>(),
                ModContent.ItemType<Items.Vanities.Dyes.ScrollDye>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Happens naturally at night. Can alternatively summoned with a [i:" + ModContent.ItemType<MythicStarfruit>() + "]. Ends when the sun rises",
                zh_Hans: null),
            "AQMod/Assets/BossChecklist/GlimmerEvent",
            "AQMod/Assets/EventIcons/GlimmerEvent");
        internal override EventProgressBar ProgressBar => new BasicEventProgressBar(
                    () => CanShowInvasionProgress(),
                    () => 1f - (float)GetTileDistance(Main.LocalPlayer) / MaxDistance,
                    "AQMod/Assets/EventIcons/GlimmerEvent",
                    "Mods.AQMod.EventName.GlimmerEvent",
                     new Color(120, 20, 110, 128));

        protected override void Setup(AQMod mod)
        {
            Layers = new List<GlimmerEventLayer>
            {
                new GlimmerEventLayer(ModContent.NPCType<Starite>(), MaxDistance, StariteSpawnChance),
                new GlimmerEventLayer(ModContent.NPCType<SuperStarite>(), SuperStariteDistance, SuperStariteSpawnChance),
                new GlimmerEventLayer(ModContent.NPCType<HyperStarite>(), HyperStariteDistance, HyperStariteSpawnChance),
            };
        }

        public override void Initialize()
        {
            tileX = 0;
            tileY = 0;
            spawnChance = -1;
            StariteDisco = false;
            stariteProjectileColor = StariteProjectileColorOrig;
            deactivationTimer = -1;
        }

        public override void PostUpdate()
        {
            if (Main.dayTime || Main.pumpkinMoon || Main.snowMoon)
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
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        AQMod.BroadcastMessage("Mods.AQMod.EventEnding.GlimmerEvent", TextColor);
                        NetMessage.SendData(MessageID.WorldData);
                    }
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

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(IsActive);
            if (IsActive)
            {
                writer.Write(tileX);
                writer.Write(tileY);
            }
            writer.Write(spawnChance);
            writer.Write(StariteDisco);
            writer.Write(deactivationTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                tileX = reader.ReadUInt16();
                tileY = reader.ReadUInt16();
            }
            spawnChance = reader.ReadInt32();
            StariteDisco = reader.ReadBoolean();
            deactivationTimer = reader.ReadInt32();
        }

        public static bool SpawnsActive(Player player)
        {
            return IsActive && deactivationTimer <= 0 && player.position.Y < (Main.worldSurface + 50) * 16;
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
                rarity += HardmodeRarityAdd;
            if (WorldDefeats.DownedGlimmer)
                rarity += GlimmerDownedRarityAdd;
            return rarity;
        }


        /// <summary>
        /// Whether or not the invasion progress for the Glimmer Event can be shown
        /// </summary>
        /// <returns></returns>
        public static bool CanShowInvasionProgress()
        {
            if (SpawnsActive(Main.LocalPlayer) && OmegaStariteScenes.OmegaStariteIndexCache == -1)
            {
                if (GetTileDistance(Main.LocalPlayer) < MaxDistance)
                    return true;
            }
            return false;
        }

        public static bool Activate(bool resetSpawnChance = true)
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
                    Activate(statuePlacements[0].Position.X, statuePlacements[0].Position.Y, resetSpawnChance);
                }
                else
                {
                    int randIndex = Main.rand.Next(statuePlacements.Count);
                    Activate(statuePlacements[randIndex].Position.X, statuePlacements[randIndex].Position.Y, resetSpawnChance);
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
                    if (resetSpawnChance)
                    {
                        spawnChance = GetBaseRarity();
                    }
                    return true;
                }
            }
            return false;
        }

        public static void Activate(int x, int y, bool resetSpawnChance = true)
        {
            OmegaStariteScenes.SceneType = 0;
            if (resetSpawnChance)
                spawnChance = GetBaseRarity();
            tileX = (ushort)x;
            tileY = (ushort)y;
        }

        public static void Deactivate()
        {
            deactivationTimer = -1;
            OmegaStariteScenes.SceneType = 0;
            tileX = 0;
            tileY = 0;
            if (StariteDisco)
            {
                StariteDisco = false;
                stariteProjectileColor = StariteProjectileColorOrig;
            }
        }

        internal static ushort ManageGlimmerY()
        {
            for (ushort j = 180; j < Main.worldSurface; j++)
            {
                if (Framing.GetTileSafely(tileX, j).active() && Main.tileSolid[Main.tile[tileX, j].type] && !Main.tileSolidTop[Main.tile[tileX, j].type])
                    return j;
            }
            return (ushort)Main.worldSurface;
        }

        public static int GetTileDistance(Player player)
        {
            return (int)((player.position.X + player.width) / 16 - tileX).Abs();
        }

        public static void OnTurnNight()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (spawnChance == -1)
            {
                spawnChance = GetBaseRarity();
                return;
            }
            if (Main.moonPhase != Constants.MoonPhases.FullMoon && !Main.bloodMoon && NPC.AnyNPCs(NPCID.Dryad))
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && player.statLifeMax > 200)
                    {
                        if (spawnChance <= 2 && Activate(resetSpawnChance: true))
                        {
                            if (AQPlayer.IgnoreMoons())
                            {
                                CosmicanonCounts.GlimmersPrevented++;
                                NetHelper.PreventedGlimmer();
                            }
                            else
                            {
                                AQMod.BroadcastMessage("Mods.AQMod.EventWarning.GlimmerEvent", TextColor);
                                NetHelper.GlimmerEventNetUpdate();
                            }
                        }
                        else
                        {
                            spawnChance -= Main.rand.Next(spawnChance) / 2;
                        }
                        break;
                    }
                }
            }
        }

        public static bool ShouldKillStar(NPC npc)
        {
            return Main.dayTime;
        }
    }
}