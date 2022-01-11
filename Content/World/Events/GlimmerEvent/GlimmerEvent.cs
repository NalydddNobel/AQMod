using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Content.World.Events.ProgressBars;
using AQMod.Content.World.FallingStars;
using AQMod.Items.BossItems;
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
using Terraria.ModLoader.IO;

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

        internal static Color StariteProjectileColorOrig => new Color(200, 10, 255, 0);
        public static Color TextColor => new Color(238, 17, 68, 255);
        public static List<GlimmerEventLayer> Layers { get; private set; }

        public static ushort tileX;
        public static ushort tileY;
        public static int deactivationDelay = -1;
        public static Color stariteProjectileColoring;
        public static bool stariteDiscoParty;
        public static bool renderUltimateSword;

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
                    () => IsAbleToShowInvasionProgressBar(),
                    () => 1f - (float)GetTileDistanceUsingPlayer(Main.LocalPlayer) / MaxDistance,
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
                GlimmerEventSky.InitNight();
        }

        public override void PreUpdate()
        {
            if (PassingDays.OnTurnNight && PassingDays.daysPassedSinceLastGlimmerEvent > 4 
                && Main.moonPhase != Constants.MoonPhases.FullMoon && !Main.bloodMoon && NPC.AnyNPCs(NPCID.Dryad))
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
                                CosmicanonCounts.GlimmersPrevented++;
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

            if (Main.rand.Next(8000) < (10f * Main.maxTilesX / 4200f))
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
            if (AreStariteSpawnsCurrentlyActive(Main.LocalPlayer) && OmegaStariteScenes.OmegaStariteIndexCache == -1)
            {
                if (GetTileDistanceUsingPlayer(Main.LocalPlayer) < MaxDistance)
                    return true;
            }
            return false;
        }

        public static int GetLayerIndexThroughTileDistance(int tileDistance)
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
            OmegaStariteScenes.SceneType = 0;
            tileX = (ushort)x;
            tileY = (ushort)y;
        }

        public static void Deactivate()
        {
            deactivationDelay = -1;
            OmegaStariteScenes.SceneType = 0;
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