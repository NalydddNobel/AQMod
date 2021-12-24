using AQMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace AQMod.Content.Seasonal.Christmas
{
    public class XmasSeeds : ModWorld
    {
        public static float snowflakeWind;
        public static UnifiedRandom snowflakeRandom;
        public static List<FarBGSnowflake> farBGSnowflakes;
        public static List<CloseBGSnowflake> closeBGSnowflakes;
        public static GenerationProgress generationProgress;
        public static GenerationProgress realGenerationProgress;
        public static bool generatingSnowBiomeText;

        public static bool XmasWorld { get; private set; }

        private static bool IsChristmasSeed(string seed)
        {
            switch (seed.ToLower())
            {
                case "christmas":
                case "aqchristmas":
                case "aq christmas":
                case "xmas":
                case "aqxmas":
                case "aq xmas":
                return true;
            }
            return false;
        }

        public override void PreWorldGen()
        {
            XmasWorld = false;
            if (Main.ActiveWorldFileData != null && IsChristmasSeed(Main.ActiveWorldFileData.SeedText))
            {
                XmasWorld = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            InsertInto("Remove Water From Sand", UpdateSnow(0), tasks, 1); // these tasks have been chosen to be the most safest ones to insert into... I hope
            InsertInto("Temple", UpdateSnow(1), tasks, 1);
            InsertInto("Quick Cleanup", UpdateSnow(2), tasks, 1);
            InsertInto("Spawn Point", UpdateSnow(3), tasks);
            InsertInto("Micro Biomes", UpdateSnow(4), tasks, 1);
            InsertInto("Slush Check", new PassLegacy("AQMod: UpdateChristmasText", (p) =>
            {
                if (XmasWorld)
                {
                    generatingSnowBiomeText = true;
                    if (generationProgress != null)
                        generationProgress.Message = Lang.gen[56].Value;
                }
            }), tasks);
        }

        private static bool InsertInto(string name, PassLegacy pass, List<GenPass> passes, int offset = 0)
        {
            int index = passes.FindIndex((t) => t.Name == name);
            if (index != -1)
            {
                passes.Insert(index + offset, pass);
            }
            return false;
        }

        private static PassLegacy UpdateSnow(int index) => new PassLegacy("AQMod: UpdateSnow " + index, (p) =>
        {
            if (!XmasWorld)
            {
                return;
            }
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    bool replaceBlock = true;
                    if (Main.tile[i, j].type == TileID.SnowBlock ||
                        Main.tile[i, j].type == TileID.SnowBrick ||
                        Main.tile[i, j].type == TileID.SnowballLauncher ||
                        Main.tile[i, j].type == TileID.IceBlock ||
                        Main.tile[i, j].type == TileID.IceBrick ||
                        Main.tile[i, j].type == TileID.BreakableIce ||
                        Main.tile[i, j].type == TileID.CorruptIce ||
                        Main.tile[i, j].type == TileID.FleshIce ||
                        Main.tile[i, j].type == TileID.HallowedIce ||
                        Main.tile[i, j].type == TileID.Cobweb ||
                        Main.tile[i, j].type == TileID.CandyCaneBlock ||
                        Main.tile[i, j].type == TileID.GreenCandyCaneBlock ||
                        Main.tile[i, j].type == TileID.PineTree ||
                        Main.tile[i, j].type == TileID.BorealWood)
                    {
                        replaceBlock = false;
                    }
                    bool replaceWall = true;
                    if (Main.tile[i, j].type == TileID.Containers2)
                    {
                        Main.tile[i, j].type = TileID.Containers;
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].type == TileID.Vines ||
                        Main.tile[i, j].type == TileID.HallowedVines ||
                        Main.tile[i, j].type == TileID.CrimsonVines ||
                        Main.tile[i, j].type == TileID.JungleVines)
                    {
                        Main.tile[i, j].type = TileID.IceBlock;
                        Main.tile[i, j].inActive(true);
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].type == ModContent.TileType<Tiles.Torches>())
                    {
                        Main.tile[i, j].type = TileID.Torches;
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].wall == WallID.SnowWallUnsafe || 
                        Main.tile[i, j].wall == WallID.SnowBrick || 
                        Main.tile[i, j].wall == WallID.IceBrick || 
                        Main.tile[i, j].wall == WallID.IceUnsafe)
                    {
                        replaceWall = false;
                    }
                    if (Main.tile[i, j].type == TileID.BlueDungeonBrick)
                    {
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.PinkDungeonBrick || 
                    Main.tile[i, j].type == TileID.GreenDungeonBrick)
                    {
                        Main.tile[i, j].type = TileID.BlueDungeonBrick;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Containers)
                    {
                        if (!Chest.isLocked(i, j) && (Main.tile[i, j].frameX < 648 || Main.tile[i, j].frameX >= 1008) && (Main.tile[i, j].frameX < 1188 || Main.tile[i, j].frameX >= 1244))
                        {
                            Main.tile[i, j].frameX = (short)(Main.tile[i, j].frameX % 36 + 396);
                            Main.tile[i, j].color(Constants.Paint.SkyBlue);
                            replaceBlock = false;
                        }
                    }
                    else if (Main.tile[i, j].type == TileID.Torches)
                    {
                        Main.tile[i, j].frameY = 198;
                        Main.tile[i, j].color(Constants.Paint.SkyBlue);
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Platforms)
                    {
                        Main.tile[i, j].frameY = 630;
                        Main.tile[i, j].color(Constants.Paint.SkyBlue);
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.WoodenBeam)
                    {
                        Main.tile[i, j].type = TileID.CandyCaneBlock;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Stone ||
                        Main.tile[i, j].type == TileID.Ash ||
                        Main.tile[i, j].type == TileID.Granite ||
                        Main.tile[i, j].type == TileID.Marble ||
                        Main.tile[i, j].type == TileID.Cloud ||
                        Main.tile[i, j].type == TileID.RainCloud)
                    {
                        Main.tile[i, j].type = TileID.IceBlock;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Ebonstone)
                    {
                        Main.tile[i, j].type = TileID.CorruptIce;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Crimstone)
                    {
                        Main.tile[i, j].type = TileID.FleshIce;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.Pearlstone)
                    {
                        Main.tile[i, j].type = TileID.HallowedIce;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.GraniteBlock ||
                        Main.tile[i, j].type == TileID.MarbleBlock ||
                        Main.tile[i, j].type == TileID.WoodBlock ||
                        Main.tile[i, j].type == TileID.DynastyWood ||
                        Main.tile[i, j].type == TileID.LivingWood ||
                        Main.tile[i, j].type == TileID.PalmWood ||
                        Main.tile[i, j].type == TileID.Ebonwood ||
                        Main.tile[i, j].type == TileID.Shadewood ||
                        Main.tile[i, j].type == TileID.Pearlwood ||
                        Main.tile[i, j].type == TileID.RichMahogany)
                    {
                        Main.tile[i, j].type = TileID.IceBrick;
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.BlueMoss ||
                       Main.tile[i, j].type == TileID.BrownMoss ||
                       Main.tile[i, j].type == TileID.GreenMoss ||
                       Main.tile[i, j].type == TileID.PurpleMoss ||
                       Main.tile[i, j].type == TileID.RedMoss ||
                       Main.tile[i, j].type == TileID.LavaMoss ||
                       Main.tile[i, j].type == TileID.ObsidianBrick)
                    {
                        Main.tile[i, j].type = TileID.SnowBrick;
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].type == TileID.HellstoneBrick)
                    {
                        Main.tile[i, j].type = TileID.IceBrick;
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].wall == WallID.BlueDungeon ||
                        Main.tile[i, j].wall == WallID.BlueDungeonSlab ||
                        Main.tile[i, j].wall == WallID.BlueDungeonSlabUnsafe ||
                        Main.tile[i, j].wall == WallID.BlueDungeonTile ||
                        Main.tile[i, j].wall == WallID.BlueDungeonTileUnsafe ||
                        Main.tile[i, j].wall == WallID.BlueDungeonUnsafe)
                    {
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeon ||
                        Main.tile[i, j].wall == WallID.GreenDungeon)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeon;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeonSlab ||
                        Main.tile[i, j].wall == WallID.GreenDungeonSlab)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonSlab;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeonSlabUnsafe ||
                        Main.tile[i, j].wall == WallID.GreenDungeonSlabUnsafe)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonSlabUnsafe;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeonTile ||
                        Main.tile[i, j].wall == WallID.GreenDungeonTile)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonTile;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeonTileUnsafe ||
                        Main.tile[i, j].wall == WallID.GreenDungeonTileUnsafe)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonTileUnsafe;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.PinkDungeonUnsafe ||
                        Main.tile[i, j].wall == WallID.GreenDungeonUnsafe)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonUnsafe;
                        replaceWall = false;
                    }
                    else if (Main.tile[i, j].wall == WallID.ObsidianBrick ||
                        Main.tile[i, j].wall == WallID.GreenDungeonUnsafe)
                    {
                        Main.tile[i, j].wall = WallID.BlueDungeonUnsafe;
                        replaceWall = false;
                    }

                    if (Main.tile[i, j].type == TileID.Stalactite)
                    {
                        Main.tile[i, j].frameX = (short)(Main.tile[i, j].frameX % 54);
                        replaceBlock = false;
                    }
                    else if (Main.tile[i, j].type == TileID.LihzahrdBrick ||
                        Main.tile[i, j].type == TileID.Copper ||
                        Main.tile[i, j].type == TileID.Tin ||
                        Main.tile[i, j].type == TileID.Iron ||
                        Main.tile[i, j].type == TileID.Lead ||
                        Main.tile[i, j].type == TileID.Silver ||
                        Main.tile[i, j].type == TileID.Tungsten ||
                        Main.tile[i, j].type == TileID.Gold ||
                        Main.tile[i, j].type == TileID.Platinum ||
                        Main.tile[i, j].type == TileID.Spikes ||
                        Main.tile[i, j].type == TileID.WoodenSpikes ||
                        Main.tile[i, j].type == TileID.Demonite ||
                        Main.tile[i, j].type == TileID.Crimtane ||
                        Main.tileContainer[Main.tile[i, j].type] ||
                        (Main.tileFrameImportant[Main.tile[i, j].type] && Main.tile[i, j].type != TileID.Trees))
                    {
                        Main.tile[i, j].color(Constants.Paint.DeepBlue);
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].type == TileID.Plants ||
                        Main.tile[i, j].type == TileID.Plants2 ||
                        Main.tile[i, j].type == TileID.CorruptPlants ||
                        Main.tile[i, j].type == TileID.HallowedPlants ||
                        Main.tile[i, j].type == TileID.HallowedPlants2 ||
                        Main.tile[i, j].type == TileID.JunglePlants ||
                        Main.tile[i, j].type == TileID.JunglePlants2 ||
                        Main.tile[i, j].type == TileID.MushroomPlants)
                    {
                        Main.tile[i, j].color(Constants.Paint.White);
                        replaceBlock = false;
                    }
                    if (Main.tile[i, j].type == TileID.Pots)
                    {
                        Main.tile[i, j].frameX = (short)(Main.tile[i, j].frameY % 108 + 144);
                        Main.tile[i, j].color(Constants.Paint.Blue);
                        replaceBlock = false;
                    }
                    if (j < Main.maxTilesY - 1)
                    {
                        if (Main.tile[i, j].type == TileID.LongMoss)
                        {
                            if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
                            {
                                Main.tile[i, j].type = TileID.Presents;
                                Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(8) * 18);
                                Main.tile[i, j].frameY = 0;
                                replaceBlock = false;
                            }
                            else
                            {
                                Main.tile[i, j].active(false);
                            }
                        }
                    }
                    if (Main.tile[i, j].wall == WallID.LihzahrdBrick ||
                        Main.tile[i, j].wall == WallID.LihzahrdBrickUnsafe ||
                        Main.tile[i, j].wall == WallID.SpiderUnsafe)
                    {
                        Main.tile[i, j].wallColor(Constants.Paint.DeepBlue);
                        replaceWall = false;
                    }

                    if (replaceBlock && Main.tile[i, j].active() && !Main.tileFrameImportant[Main.tile[i, j].type])
                    {
                        Main.tile[i, j].type = TileID.SnowBlock;
                    }
                    if (replaceWall && Main.tile[i, j].wall > WallID.None)
                    {
                        Main.tile[i, j].wall = WallID.IceUnsafe;
                    }
                }
            }
        });

        private static void ReplaceVine(int x, int y, int replaceWithTile, bool actuated = true)
        {
            int tileID = Main.tile[x, y].type;
            for (int y2 = y; y2 < Main.maxTilesY; y2++)
            {
                if (Main.tile[x, y2].active() && Main.tile[x, y2].type == tileID)
                {
                    Main.tile[x, y2].type = (ushort)replaceWithTile;
                    Main.tile[x, y2].actuator(actuated);
                }
                else
                {
                    break;
                }
            }
        }

        public override void Initialize()
        {
            XmasWorld = false;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["ChristmasWorld"] = XmasWorld,
            };
        }

        public override void Load(TagCompound tag)
        {
            XmasWorld = tag.GetBool("ChristmasWorld");
        }

        public override void PreUpdate()
        {
            if (XmasWorld)
            {
                Main.xMas = true;
            }
        }
    }
}