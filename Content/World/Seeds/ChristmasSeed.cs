using Aequus.Content.World.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content.World.Seeds {
    public class ChristmasSeedSystem : ModSystem
    {
        public static bool Active { get => AequusWorld.xmasWorld; set => AequusWorld.xmasWorld = value; }

        public static HashSet<int> DoNotConvert { get; private set; }
        public static HashSet<int> SnowTiles { get; private set; }
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
            SnowTiles = new HashSet<int>()
            {
                TileID.SnowBlock,
                TileID.IceBlock,
                TileID.IceBrick,
                TileID.SnowBrick,
                TileID.BorealWood,
                TileID.Slush,
                TileID.BlueDungeonBrick,
                TileID.CrackedBlueDungeonBrick,
                TileID.SnowCloud,
                TileID.BreakableIce,
            };
            SpecialConversions = new Dictionary<int, int>(SnowTiles.ToDictionary((t) => t))
            {
                [TileID.Mud] = TileID.BreakableIce,

                [TileID.Dirt] = TileID.SnowBlock,

                [TileID.Cloud] = TileID.SnowCloud,
                [TileID.RainCloud] = TileID.SnowCloud,

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
                [TileID.Silver] = (x, y, t) => t.TileColor = PaintID.DeepCyanPaint,
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
                [WallID.ObsidianBrickUnsafe] = WallID.SnowWallUnsafe,
                [WallID.HellstoneBrickUnsafe] = WallID.SnowWallUnsafe,
            };
        }

        public override void Unload()
        {
            DoNotConvert?.Clear();
            SnowTiles?.Clear();
            SpecialConversions?.Clear();
            WallConversions?.Clear();
            MoreSpecialConversions?.Clear();
        }

        public override void OnWorldLoad()
        {
            Active = false;
        }

        public override void OnWorldUnload()
        {
            Active = false;
        }

        public override void PreUpdateEntities()
        {
            if (!Active)
            {
                return;
            }

            Main.xMas = true;
        }

        public override void PreWorldGen()
        {
            if (WorldGen.currentWorldSeed.Trim().ToLower() == "rockman")
            {
                Active = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            if (!Active)
            {
                return;
            }

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
            AequusWorldGenerator.AddPass("Guide", "SANTA", (progress, configuration) =>
            {
                NPC.NewNPC(null, Main.spawnTileX * 16, Main.spawnTileY * 16, NPCID.SantaClaus);
            }, tasks);
            AequusWorldGenerator.AddPass("Micro Biomes", "Christmas Seed 2", (progress, configuration) =>
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        CheckIce(i, j);
                        if (WorldGen.InWorld(i, j, 400) && WorldGen.genRand.NextBool(30))
                        {
                            WorldGen.PlaceTile(i, j, TileID.Presents, style: WorldGen.genRand.Next(8));
                        }
                        if (WorldGen.InWorld(i, j, 5) && Main.tile[i, j].TileType == TileID.SnowCloud && Main.tile[i, j + 1].IsFullySolid())
                        {
                            Main.tile[i, j].TileType = TileID.SnowBlock;
                        }
                    }
                }
            }, tasks);

            AequusWorldGenerator.RemovePass("Wall Variety", tasks);
            AequusWorldGenerator.RemovePass("Moss", tasks);
            AequusWorldGenerator.RemovePass("Guide", tasks);
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            if (Active)
                Main.SceneMetrics.SnowTileCount += 300;
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
            else if (SpecialConversions.TryGetValue(t.TileType, out int tileType))
            {
                t.TileType = (ushort)tileType;
            }
            else if (MoreSpecialConversions.TryGetValue(t.TileType, out var action))
            {
                action(x, y, t);
            }
            else if (Main.tileFrameImportant[t.TileType] || DoNotConvert.Contains(t.TileType))
            {
                if (!Main.tileContainer[t.TileType] && !TileID.Sets.IsATreeTrunk[t.TileType] && !TileID.Sets.TreeSapling[t.TileType])
                    t.TileColor = PaintID.CyanPaint;
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
                else if (y + WorldGen.genRand.Next(15) > (int)GenVars.worldSurface)
                {
                    t.WallType = WallID.IceUnsafe;
                }
                else
                {
                    t.WallType = WallID.SnowWallUnsafe;
                }
                if (t.WallType == WallID.HiveUnsafe || t.WallType == WallID.LihzahrdBrickUnsafe)
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

        public override void RandomUpdate(int i, int j, int type)
        {
            if (!ChristmasSeedSystem.Active)
                return;

            if (Main.raining)
            {
                if (!ChristmasSeedSystem.SnowTiles.Contains(Main.tile[i, j].TileType) && !ChristmasSeedSystem.DoNotConvert.Contains(Main.tile[i, j].TileType) && !Main.tile[i, j - 1].IsFullySolid())
                {
                    Christmasify(i, j);
                }
                else if (ChristmasSeedSystem.SnowTiles.Contains(Main.tile[i, j - 1].TileType) || ChristmasSeedSystem.DoNotConvert.Contains(Main.tile[i, j - 1].TileType))
                {
                    int y = j;
                    do
                    {
                        Christmasify(i, y - 1);
                        Christmasify(i, y);
                        y++;
                    }
                    while (y < j + 50 && WorldGen.InWorld(i, y, 5) && !Main.tile[i, y].IsFullySolid() && !Main.tileFrameImportant[Main.tile[i, y].TileType]);
                }
            }
        }

        public static void Christmasify(int x, int y)
        {
            int tileID = Main.tile[x, y].TileType;
            byte paint = Main.tile[x, y].TileColor;
            int wall = Main.tile[x, y].WallType;
            byte paintWall = Main.tile[x, y].WallColor;
            ChristmasSeedSystem.CheckIce(x, y);
            if (tileID != Main.tile[x, y].TileType || wall != Main.tile[x, y].WallType || paintWall != Main.tile[x, y].WallColor || paint != Main.tile[x, y].TileColor)
            {
                WorldGen.TileFrame(x, y);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, x - 1, y - 1, 3, 3);
                }
            }
        }
    }
}