using AQMod.Tiles;
using AQMod.Tiles.Nature.CrabCrevice;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public sealed class MiscFoliage : ModWorld
    {
        public static void GenerateExoticBlotches(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.ExoticBlotches");
            for (int i = 0; i < 6000; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow + 80, Main.maxTilesY - 240);

                int style = WorldGen.genRand.Next(3);
                int size = WorldGen.genRand.Next(50, 150);
                if (ExoticCoralNew.TryPlaceExoticBlotch(x, y, style, size))
                    i += 500;
            }
        }

        public static void GenerateCrabCreviceGrass(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.MiscFoliageCrabCrevice");
            for (int i = 50; i < Main.maxTilesX - 50; i++)
            {
                for (int j = 50; j < Main.maxTilesY - 50; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (Main.tile[i, j + 1] == null)
                    {
                        Main.tile[i, j + 1] = new Tile();
                        continue;
                    }
                    if (!Main.tile[i, j].active() && Main.tile[i, j].liquid > 100)
                    {
                        if (Main.tile[i, j + 1].active() && !Main.tile[i, j + 1].halfBrick() && Main.tile[i, j + 1].slope() == 0 && Main.tile[i, j + 1].type == ModContent.TileType<SedimentSand>())
                        {
                            Main.tile[i, j].active(active: true);
                            Main.tile[i, j].halfBrick(halfBrick: false);
                            Main.tile[i, j].slope(slope: 0);
                            if (WorldGen.genRand.NextBool(8))
                            {
                                Main.tile[i, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                                Main.tile[i, j].frameX = 0;
                            }
                            else
                            {
                                Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                                Main.tile[i, j].frameX = (short)(22 * ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                            }
                            Main.tile[i, j].frameY = 0;

                            WorldGen.SquareTileFrame(i, j, resetFrame: true);
                        }
                        if (WorldGen.genRand.NextBool(5))
                        {
                            if (Main.tile[i, j - 1] == null)
                            {
                                Main.tile[i, j - 1] = new Tile();
                            }
                            else if (!Main.tile[i, j].active() && Main.tile[i, j - 1].active() && Main.tile[i, j - 1].type == ModContent.TileType<SedimentSand>())
                            {
                                byte slope = Main.tile[i, j - 1].slope();
                                if (slope != AQTile.Slope_TopLeft && slope != AQTile.Slope_TopRight)
                                {
                                    Main.tile[i, j].active(active: true);
                                    Main.tile[i, j].halfBrick(halfBrick: false);
                                    Main.tile[i, j].slope(slope: 0);
                                    Main.tile[i, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                                    Main.tile[i, j].frameX = 0;
                                    Main.tile[i, j].frameY = 0;

                                    WorldGen.SquareTileFrame(i, j, resetFrame: true);
                                }
                            }
                            if (Main.tile[i - 1, j] == null)
                            {
                                Main.tile[i - 1, j] = new Tile();
                            }
                            else if (!Main.tile[i, j].active() && Main.tile[i - 1, j].active() && !Main.tile[i - 1, j].halfBrick() && Main.tile[i - 1, j].type == ModContent.TileType<SedimentSand>())
                            {
                                byte slope = Main.tile[i - 1, j].slope();
                                if (slope != AQTile.Slope_TopLeft && slope != AQTile.Slope_TopRight)
                                {
                                    Main.tile[i, j].active(active: true);
                                    Main.tile[i, j].halfBrick(halfBrick: false);
                                    Main.tile[i, j].slope(slope: 0);
                                    Main.tile[i, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                                    Main.tile[i, j].frameX = 0;
                                    Main.tile[i, j].frameY = 0;

                                    WorldGen.SquareTileFrame(i, j, resetFrame: true);
                                }
                            }
                            if (Main.tile[i + 1, j] == null)
                            {
                                Main.tile[i + 1, j] = new Tile();
                            }
                            else if (!Main.tile[i, j].active() && Main.tile[i + 1, j].active() && !Main.tile[i + 1, j].halfBrick() && Main.tile[i + 1, j].type == ModContent.TileType<SedimentSand>())
                            {
                                byte slope = Main.tile[i + 1, j].slope();
                                if (slope != AQTile.Slope_TopLeft && slope != AQTile.Slope_TopRight)
                                {
                                    Main.tile[i, j].active(active: true);
                                    Main.tile[i, j].halfBrick(halfBrick: false);
                                    Main.tile[i, j].slope(slope: 0);
                                    Main.tile[i, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                                    Main.tile[i, j].frameX = 0;
                                    Main.tile[i, j].frameY = 0;

                                    WorldGen.SquareTileFrame(i, j, resetFrame: true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals("Micro Biomes"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, new PassLegacy("AQMod: Exotic Coral", GenerateExoticBlotches));
                tasks.Insert(i, new PassLegacy("AQMod: Misc Foliage", GenerateCrabCreviceGrass));
            }
        }
    }
}