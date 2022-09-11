using Aequus.Content.CrossMod;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration
{
    public class CrabCreviceGenerator
    {
        private int size;
        public Point location;

        public void Reset()
        {
            location = new Point();
            size = Main.maxTilesX / 20;
        }

        public bool ProperCrabCreviceAnchor(int x, int y)
        {
            return !Main.tile[x, y].HasTile && Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y + 1].HasTile && Main.tileSand[Main.tile[x, y + 1].TileType];
        }

        private bool CanOverwriteTile(Tile tile)
        {
            return !Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType];
        }

        public void CreateSandAreaForCrevice(int x, int y)
        {
            if (x - size < 10)
            {
                x = size + 10;
            }
            else if (x + size > Main.maxTilesX - 10)
            {
                x = Main.maxTilesX - 10 - size;
            }
            if (y - size < 10)
            {
                y = size + 10;
            }
            else if (y + size > Main.maxTilesY - 10)
            {
                y = Main.maxTilesY - 10 - size;
            }
            List<Point> placeTiles = new List<Point>();
            for (int i = 0; i < size * 2; i++)
            {
                for (int j = 0; j < size * 3; j++) // A bit overkill of an extra check, but whatever
                {
                    int x2 = x + i - size;
                    int y2 = y + j - size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.6f) <= size)
                    {
                        if (CanOverwriteTile(Main.tile[x2, y2]))
                        {
                            if (Main.tile[x2, y2].HasTile)
                                placeTiles.Add(new Point(x2, y2));
                            if (y2 > (int)Main.worldSurface)
                                Main.tile[x2, y2].WallType = (ushort)ModContent.WallType<SedimentaryRockWallWall>();
                        }
                    }
                }
            }
            for (int i = 0; i < placeTiles.Count; i++)
            {
                int x2 = placeTiles[i].X;
                int y2 = placeTiles[i].Y;
                if (y2 > (int)Main.worldSurface)
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            Main.tile[x2 + m, y2 + n].Active(value: true);
                            Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                        }
                    }
                }
                else
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (!Main.tile[x2 + m, y2 + n].HasTile && !Main.tile[x2 + m, y2 + n].SolidType() && Main.tile[x2 + m, y2 + n].LiquidAmount > 0)
                            {
                                continue;
                            }
                            Main.tile[x2 + m, y2 + n].Active(value: true);
                            Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                        }
                    }
                }
            }
        }

        public bool HasUnOverwriteableTiles(Circle circle)
        {
            for (int i = 0; i < circle.Radius * 2; i++)
            {
                for (int j = 0; j < circle.Radius * 2; j++)
                {
                    int x2 = circle.X + i - circle.Radius;
                    int y2 = circle.Y + j - circle.Radius;
                    if (circle.Inside(x2, y2) && !CanOverwriteTile(Main.tile[x2, y2]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsValidCircleForGeneratingCave(int x, int y, int radius)
        {
            return IsValidCircleForGeneratingCave(new Circle(x, y, radius));
        }

        private bool IsValidCircleForGeneratingCave(Circle circle)
        {
            const int wallSize = 5;
            for (int i = 0; i < circle.Radius * 2; i++)
            {
                for (int j = 0; j < circle.Radius * 2; j++)
                {
                    int x = circle.X + i - circle.Radius;
                    int y = circle.Y + j - circle.Radius;
                    if (circle.Inside(x, y))
                    {
                        for (int k = -wallSize; k <= wallSize; k++)
                        {
                            for (int l = -wallSize; l <= wallSize; l++)
                            {
                                if ((!Main.tile[x + k, y + l].HasTile || !Main.tile[x + k, y + l].SolidType()) && CanOverwriteTile(Main.tile[x + k, y + l]))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool GenerateCreviceCave(int x, int y, int minScale, int maxScale, int steps)
        {
            List<Circle> validCircles = new List<Circle>();
            for (int i = maxScale; i > minScale; i--)
            {
                var c = Circle.FixedCircle(x, y, i);
                if (IsValidCircleForGeneratingCave(c))
                {
                    validCircles.Add(c);
                    break;
                }
            }
            if (validCircles.Count == 0)
            {
                return false;
            }
            validCircles.Add(validCircles[0]
                .GetRandomCircleInsideCircle(validCircles[0].Radius / 3, minScale, maxScale, WorldGen.genRand, IsValidCircleForGeneratingCave));
            if (validCircles[1].IsInvalid || HasUnOverwriteableTiles(validCircles[1]))
            {
                return false;
            }
            for (int i = 0; i < steps; i++)
            {
                int chosenCircle = WorldGen.genRand.Next(validCircles.Count);
                validCircles.Add(validCircles[chosenCircle]
                    .GetRandomCircleInsideCircle(validCircles[chosenCircle].Radius / 4, minScale, maxScale, WorldGen.genRand, IsValidCircleForGeneratingCave));
                if (validCircles[^1].IsInvalid || HasUnOverwriteableTiles(validCircles[^1]))
                {
                    //Main.NewText("c" + (i + 2) + " was considered invalid!");
                    return false;
                }
            }

            for (int k = 0; k < validCircles.Count; k++)
            {
                for (int i = 0; i < validCircles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < validCircles[k].Radius * 2; j++)
                    {
                        int x2 = validCircles[k].X + i - validCircles[k].Radius;
                        int y2 = validCircles[k].Y + j - validCircles[k].Radius;
                        if (validCircles[k].Inside(x2, y2))
                        {
                            for (int m = -2; m <= 2; m++)
                            {
                                for (int n = -2; n <= 2; n++)
                                {
                                    Main.tile[x2 + m, y2 + n].Active(value: true);
                                    Main.tile[x2 + m, y2 + n].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                                    Main.tile[x2 + m, y2 + n].WallType = (ushort)ModContent.WallType<SedimentaryRockWallWall>();
                                }
                            }
                        }
                    }
                }
            }

            byte minWater = Math.Min((byte)(255 - validCircles[0].Radius / 24 + validCircles[0].Y / 10), (byte)253);
            byte maxWater = 255;
            if (WorldGen.genRand.NextBool(4))
            {
                minWater /= 6;
                maxWater = (byte)(minWater + 2);
            }
            else if (WorldGen.genRand.NextBool())
            {
                minWater *= 4;
                if (minWater > 253)
                {
                    minWater = 253;
                }
                maxWater = 255;
            }
            else if (minWater < 100)
            {
                maxWater = 125;
            }

            for (int k = 0; k < validCircles.Count; k++)
            {
                for (int i = 0; i < validCircles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < validCircles[k].Radius * 2; j++)
                    {
                        int x2 = validCircles[k].X + i - validCircles[k].Radius;
                        int y2 = validCircles[k].Y + j - validCircles[k].Radius;
                        if (validCircles[k].Inside(x2, y2))
                        {
                            Main.tile[x2, y2].Active(value: false);
                            if (minWater > 100 && Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].SolidType())
                            {
                                Main.tile[x2, y2].LiquidAmount = 255;
                            }
                            else
                            {
                                Main.tile[x2, y2].LiquidAmount = (byte)WorldGen.genRand.Next(minWater, maxWater);
                            }
                        }
                    }
                }
            }

            if (WorldGen.genRand.NextBool(3))
            {
                var caverPoint = WorldGen.genRand.Next(validCircles);
                WorldGen.Caverer(caverPoint.X, caverPoint.Y);
            }
            return true;
        }

        public void Generate(GenerationProgress progress)
        {
            location = Point.Zero;
            size = 160;

            if (progress == null)
                progress = new GenerationProgress();

            progress.Message = AequusText.GetText("WorldGeneration.CrabCrevice");

            int reccomendedDir = 0;
            if (CalamityModSupport.CalamityMod != null)
            {
                reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1;
            }
            //else if (AQMod.thoriumMod.IsActive)
            //{
            //    reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
            //}

            for (int i = 0; i < 5000; i++)
            {
                int checkX = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    checkX = Main.maxTilesX - checkX;
                for (int checkY = 200; checkY < Main.worldSurface; checkY++)
                {
                    if (ProperCrabCreviceAnchor(checkX, checkY))
                    {
                        if (reccomendedDir == 0 || location.X == 0)
                        {
                            location.X = checkX;
                            location.Y = checkY;
                        }
                        else if (reccomendedDir == -1)
                        {
                            if (checkX * 2 < Main.maxTilesX)
                            {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        else
                        {
                            if (checkX * 2 > Main.maxTilesX)
                            {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        i += 1000;
                        break;
                    }
                }
            }

            int x = location.X;
            int y = location.Y;
            location = new Point(x, y);

            CreateSandAreaForCrevice(x, y + 40);

            int finalCaveStart = -50;
            int finalCaveX;
            if (x < Main.maxTilesX / 2)
            {
                finalCaveX = x + WorldGen.genRand.Next(60);
            }
            else
            {
                finalCaveX = x + WorldGen.genRand.Next(-60, 0);
            }
            if (finalCaveX + finalCaveStart < 30)
            {
                finalCaveStart = 30 - finalCaveX;
            }
            int finalCaveEnd = 50;
            if (finalCaveX + finalCaveEnd > Main.maxTilesX - 30)
            {
                finalCaveEnd = Main.maxTilesX - 30 - finalCaveX;
            }
            List<Circle> finalCaveCircles = new List<Circle>();
            for (int k = finalCaveStart; k < finalCaveEnd; k++)
            {
                float finalCaveProgress = 1f / (finalCaveStart.Abs() + finalCaveEnd.Abs()) * k.Abs();
                var circle = new Circle(finalCaveX + k, y + 180, WorldGen.genRand.Next(2, 14) + ((int)(Math.Sin((finalCaveProgress.Abs() - 0.5f) * MathHelper.Pi) * 9.0)).Abs());
                if (!HasUnOverwriteableTiles(circle))
                {
                    finalCaveCircles.Add(circle);
                }
            }

            var caverPoint = WorldGen.genRand.Next(finalCaveCircles);
            WorldGen.Caverer(caverPoint.X, caverPoint.Y);

            for (int k = 0; k < finalCaveCircles.Count; k++)
            {
                for (int i = 0; i < finalCaveCircles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < finalCaveCircles[k].Radius * 2; j++)
                    {
                        int x2 = finalCaveCircles[k].X + i - finalCaveCircles[k].Radius;
                        int y2 = finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius;
                        if (finalCaveCircles[k].Inside(x2, y2))
                        {
                            for (int m = -2; m <= 2; m++)
                            {
                                for (int n = -2; n <= 2; n++)
                                {
                                    Main.tile[x2 + m, y2 + n].Active(value: true);
                                    Main.tile[x2 + m, y2 + n].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                                    Main.tile[x2 + m, y2 + n].WallType = (ushort)ModContent.WallType<SedimentaryRockWallWall>();
                                }
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < finalCaveCircles.Count; k++)
            {
                for (int i = 0; i < finalCaveCircles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < finalCaveCircles[k].Radius * 2; j++)
                    {
                        int x2 = finalCaveCircles[k].X + i - finalCaveCircles[k].Radius;
                        int y2 = finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius;
                        if (finalCaveCircles[k].Inside(x2, y2))
                        {
                            Main.tile[x2, y2].Active(value: false);
                            if (Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].SolidType())
                            {
                                Main.tile[x2, y2].LiquidAmount = 255;
                            }
                            else
                            {
                                Main.tile[x2, y2].LiquidAmount = (byte)WorldGen.genRand.Next(10, 100);
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < 20000; k++)
            {
                int caveX = x + WorldGen.genRand.Next(-156, 156);
                int caveY = y + WorldGen.genRand.Next(-10, 220);
                int minScale = WorldGen.genRand.Next(4, 8);
                if (GenerateCreviceCave(caveX, caveY, minScale, minScale + WorldGen.genRand.Next(4, 18), WorldGen.genRand.Next(80, 250)))
                {
                    k += 200;
                }
            }
        }
    }
}