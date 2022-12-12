﻿using Aequus.Content.WorldGeneration;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content
{
    public class ChristmasSeedSystem : ModSystem
    {
        public static bool Active { get => AequusWorld.christmasWorld; set => AequusWorld.christmasWorld = value; }

        public static HashSet<int> DoNotConvert { get; private set; }
        public static Dictionary<int, int> SpecialConversions { get; private set; }
        public static Dictionary<int, int> WallConversions { get; private set; }
        public static Dictionary<int, Action<int, int, Tile>> MoreSpecialConversions { get; private set; }

        public override void Load()
        {
            DoNotConvert = new HashSet<int>()
            {
                TileID.Spikes,
                TileID.Cobweb,
                TileID.WoodenSpikes,
                TileID.BeeHive,
                TileID.Hive,
            };
            SpecialConversions = new Dictionary<int, int>()
            {
                [TileID.SnowBlock] = TileID.SnowBlock,
                [TileID.IceBlock] = TileID.IceBlock,
                [TileID.IceBrick] = TileID.IceBrick,
                [TileID.Dirt] = TileID.SnowBlock,
                [TileID.BorealWood] = TileID.BorealWood,
                [TileID.Slush] = TileID.Slush,
                [TileID.BlueDungeonBrick] = TileID.BlueDungeonBrick,
                [TileID.CrackedBlueDungeonBrick] = TileID.CrackedBlueDungeonBrick,

                [TileID.Silt] = TileID.Slush,
                [TileID.DesertFossil] = TileID.Slush,
                [TileID.FossilOre] = TileID.Slush,

                [TileID.WoodBlock] = TileID.BorealWood,
                [TileID.LivingWood] = TileID.BorealWood,
                [TileID.DynastyWood] = TileID.BorealWood,
                [TileID.PalmWood] = TileID.BorealWood,
                [TileID.SpookyWood] = TileID.BorealWood,
                [TileID.Ebonwood] = TileID.BorealWood,
                [TileID.Pearlwood] = TileID.BorealWood,
                [TileID.Shadewood] = TileID.BorealWood,
                [TileID.RichMahogany] = TileID.BorealWood,
                [TileID.LivingMahogany] = TileID.BorealWood,

                [TileID.WoodenBeam] = TileID.BorealBeam,
                [TileID.RichMahoganyBeam] = TileID.BorealBeam,

                [TileID.ChlorophyteBrick] = TileID.IceBrick,
                [TileID.CobaltBrick] = TileID.IceBrick,
                [TileID.CopperBrick] = TileID.IceBrick,
                [TileID.CrimtaneBrick] = TileID.IceBrick,
                [TileID.DemoniteBrick] = TileID.IceBrick,
                [TileID.GoldBrick] = TileID.IceBrick,
                [TileID.ObsidianBrick] = TileID.IceBrick,
                [TileID.HellstoneBrick] = TileID.IceBrick,
                [TileID.IridescentBrick] = TileID.IceBrick,
                [TileID.IronBrick] = TileID.IceBrick,
                [TileID.TinBrick] = TileID.IceBrick,
                [TileID.TungstenBrick] = TileID.IceBrick,
                [TileID.LeadBrick] = TileID.IceBrick,
                [TileID.MeteoriteBrick] = TileID.IceBrick,
                [TileID.MythrilBrick] = TileID.IceBrick,
                [TileID.AdamantiteBeam] = TileID.IceBrick,
                [TileID.PalladiumColumn] = TileID.IceBrick,
                [TileID.BubblegumBlock] = TileID.IceBrick,
                [TileID.PlatinumBrick] = TileID.IceBrick,
                [TileID.RainbowBrick] = TileID.IceBrick,
                [TileID.SilverBrick] = TileID.IceBrick,
                [TileID.Sunplate] = TileID.IceBrick,

                [TileID.PurpleMossBrick] = TileID.SnowBrick,
                [TileID.RedBrick] = TileID.SnowBrick,
                [TileID.RedMossBrick] = TileID.SnowBrick,
                [TileID.SandstoneBrick] = TileID.SnowBrick,
                [TileID.PearlstoneBrick] = TileID.SnowBrick,
                [TileID.ArgonMossBrick] = TileID.SnowBrick,
                [TileID.BlueMossBrick] = TileID.SnowBrick,
                [TileID.BrownMossBrick] = TileID.SnowBrick,
                [TileID.CrimstoneBrick] = TileID.SnowBrick,
                [TileID.EbonstoneBrick] = TileID.SnowBrick,
                [TileID.GrayBrick] = TileID.SnowBrick,
                [TileID.GreenMossBrick] = TileID.SnowBrick,
                [TileID.KryptonMossBrick] = TileID.SnowBrick,
                [TileID.SolarBrick] = TileID.SnowBrick,
                [TileID.StardustBrick] = TileID.SnowBrick,
                [TileID.VortexBrick] = TileID.SnowBrick,
                [TileID.XenonMossBrick] = TileID.SnowBrick,

                [TileID.GreenDungeonBrick] = TileID.BlueDungeonBrick,
                [TileID.PinkDungeonBrick] = TileID.BlueDungeonBrick,

                [TileID.CrackedGreenDungeonBrick] = TileID.CrackedBlueDungeonBrick,
                [TileID.CrackedPinkDungeonBrick] = TileID.CrackedBlueDungeonBrick,
            };
            MoreSpecialConversions = new Dictionary<int, Action<int, int, Tile>>()
            {
                [TileID.Copper] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Tin] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Iron] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Lead] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Tungsten] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Gold] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Platinum] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.Demonite] = (x, y, t) => t.TileColor = PaintID.DeepRedPaint,
                [TileID.Crimtane] = (x, y, t) => t.TileColor = PaintID.DeepVioletPaint,
                [TileID.Hellstone] = (x, y, t) => t.TileColor = PaintID.DeepVioletPaint,
                [TileID.Ash] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.LihzahrdBrick] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
                [TileID.LeafBlock] = (x, y, t) => t.TileColor = PaintID.WhitePaint,
                [TileID.LivingMahoganyLeaves] = (x, y, t) => t.TileColor = PaintID.WhitePaint,
                [TileID.Heart] = (x, y, t) => t.TileColor = PaintID.ShadowPaint,
            };
            WallConversions = new Dictionary<int, int>()
            {
                [WallID.BorealWood] = WallID.BorealWood,
                [WallID.SnowWallUnsafe] = WallID.SnowWallUnsafe,
                [WallID.SnowBrick] = WallID.SnowBrick,
                [WallID.IceBrick] = WallID.IceBrick,

                [WallID.Planked] = WallID.BorealWood,
                [WallID.Wood] = WallID.BorealWood,
                [WallID.LivingWood] = WallID.BorealWood,
                [WallID.PalmWood] = WallID.BorealWood,
                [WallID.SpookyWood] = WallID.BorealWood,
                [WallID.RichMaogany] = WallID.BorealWood,
                [WallID.Ebonwood] = WallID.BorealWood,
                [WallID.Pearlwood] = WallID.BorealWood,
                [WallID.Shadewood] = WallID.BorealWood,
                [WallID.LihzahrdBrick] = WallID.LihzahrdBrickUnsafe,
                [WallID.LihzahrdBrickUnsafe] = WallID.LihzahrdBrickUnsafe,
                [WallID.Hive] = WallID.HiveUnsafe,
                [WallID.HiveUnsafe] = WallID.HiveUnsafe,
                [WallID.CopperBrick] = WallID.SnowBrick,
                [WallID.TinBrick] = WallID.SnowBrick,
                [WallID.LeadBrick] = WallID.SnowBrick,
                [WallID.IronBrick] = WallID.SnowBrick,
                [WallID.TungstenBrick] = WallID.SnowBrick,
                [WallID.PlatinumBrick] = WallID.SnowBrick,
                [WallID.SilverBrick] = WallID.SnowBrick,
                [WallID.GoldBrick] = WallID.SnowBrick,
                [WallID.DiscWall] = WallID.SnowBrick,
                [WallID.ObsidianBrickUnsafe] = WallID.IceBrick,
                [WallID.HellstoneBrickUnsafe] = WallID.IceBrick,
            };
        }

        public override void PreWorldGen()
        {
            if (WorldGen.currentWorldSeed.Trim().ToLower() == "rockman")
            {
                Active = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            if (!Active)
            {
                return;
            }
            Load();
            AequusWorldGenerator.CopyPass("Floating Islands", 5, tasks);
            AequusWorldGenerator.CopyPass("Silt", 5, tasks);
            AequusWorldGenerator.CopyPass("Webs", 10, tasks);
            AequusWorldGenerator.CopyPass("Corruption", 3, tasks);
            AequusWorldGenerator.CopyPass("Dungeon", 2, tasks);
            AequusWorldGenerator.CopyPass("Mountain Caves", 3, tasks);
            AequusWorldGenerator.CopyPass("Beaches", 10, tasks);
            AequusWorldGenerator.CopyPass("Create Ocean Caves", 3, tasks);
            AequusWorldGenerator.CopyPass("Jungle Temple", 1, tasks);
            AequusWorldGenerator.CopyPass("Hives", 3, tasks);
            AequusWorldGenerator.CopyPass("Jungle Chests", 3, tasks);
            AequusWorldGenerator.CopyPass("Piles", 10, tasks);
            AequusWorldGenerator.CopyPass("Planting Trees", 5, tasks);
            AequusWorldGenerator.CopyPass("Gems In Ice Biome", 5, tasks);

            AequusWorldGenerator.AddPass("Slush", "Christmas Seed", (progress, configuration) =>
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.UnderworldLayer; j++)
                    {
                        CheckIce(i, j);
                    }
                }
            }, tasks);
            AequusWorldGenerator.AddPass("Wet Jungle", "Christmas Temple", (progress, configuration) =>
            {
                int size = 150;
                for (int i = Main.maxTilesX / 2 - size; i < Main.maxTilesX / 2 + size; i++)
                {
                    for (int j = Main.maxTilesY / 2 - size; j < Main.maxTilesY / 2 + size; j++)
                    {
                        Junglify(i, j);
                    }
                }
                int divX3 = Main.maxTilesX / 3;
                for (int i = divX3 - size; i < divX3 + size; i++)
                {
                    for (int j = Main.maxTilesY / 2 - size; j < Main.maxTilesY / 2 + size; j++)
                    {
                        Junglify(i, j);
                    }
                }
                divX3 *= 2;
                for (int i = divX3 - size; i < divX3 + size; i++)
                {
                    for (int j = Main.maxTilesY / 2 - size; j < Main.maxTilesY / 2 + size; j++)
                    {
                        Junglify(i, j);
                    }
                }
            }, tasks);
            AequusWorldGenerator.AddPass("Micro Biomes", "Christmas Seed 2", (progress, configuration) =>
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        CheckIce(i, j);
                    }
                }
            }, tasks);
            tasks.RemoveAll((t) => t.Name == "Wall Variety");
            tasks.RemoveAll((t) => t.Name == "Moss");
        }

        public static void CheckIce(int x, int y)
        {
            int xC = x + WorldGen.genRand.Next(-50, 50);
            if (xC < 200 || xC > Main.maxTilesX - 200)
            {
                return;
            }

            var t = Main.tile[x, y];
            if (TileID.Sets.Grass[t.TileType] || TileID.Sets.Conversion.Sand[t.TileType])
            {
                t.TileType = TileID.SnowBlock;
            }
            else if (Main.tileFrameImportant[t.TileType] || DoNotConvert.Contains(t.TileType))
            {
                if (!Main.tileContainer[t.TileType] && !TileID.Sets.IsATreeTrunk[t.TileType] && !TileID.Sets.TreeSapling[t.TileType])
                    t.TileColor = PaintID.DeepCyanPaint;
            }
            else if (SpecialConversions.TryGetValue(t.TileType, out int tileType))
            {
                t.TileType = (ushort)tileType;
            }
            else if (MoreSpecialConversions.TryGetValue(t.TileType, out var action))
            {
                action(x, y, t);
            }
            else if (t.IsFullySolid() && !Main.tileFrameImportant[t.TileType])
            {
                if (TileID.Sets.Corrupt[t.TileType])
                {
                    t.TileType = TileID.CorruptIce;
                }
                else if (TileID.Sets.Crimson[t.TileType])
                {
                    t.TileType = TileID.FleshIce;
                }
                else
                {
                    t.TileType = TileID.IceBlock;
                }
            }

            if (t.WallType > 0 && !Main.wallDungeon[t.WallType])
            {
                if (WallConversions.TryGetValue(t.WallType, out int wallID))
                {
                    t.WallType = (ushort)wallID;
                }
                else if ((y + WorldGen.genRand.Next(15)) > (int)WorldGen.worldSurface)
                {
                    t.WallType = WallID.IceUnsafe;
                }
                else
                {
                    t.WallType = WallID.SnowWallUnsafe;
                }
                if (t.WallType == WallID.LihzahrdBrickUnsafe)
                {
                    t.WallColor = PaintID.DeepCyanPaint;
                }
            }
        }

        public static void Junglify(int x, int y)
        {
            if (Main.tile[x, y].IsFullySolid() && !Main.tileFrameImportant[Main.tile[x, y].TileType])
            {
                switch (WorldGen.genRand.Next(30))
                {
                    default:
                        {
                            Main.tile[x, y].TileType = TileID.Mud;
                        }
                        break;
                    case 1:
                        {
                            Main.tile[x, y].TileType = TileID.JungleGrass;
                        }
                        break;
                    case > 15:
                        {
                            Main.tile[x, y].Active(false);
                        }
                        break;
                }
            }
        }
    }

    public class ChristmasSeedTile : GlobalTile
    {
        public override void FloorVisuals(int type, Player player)
        {
            if (ChristmasSeedSystem.Active && type == TileID.IceBrick)
            {
                player.AddBuff(BuffID.Frostburn, 1);
            }
        }
    }
}