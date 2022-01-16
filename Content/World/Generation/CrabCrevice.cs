using AQMod.Common.Configuration;
using AQMod.Common.ID;
using AQMod.Items.Accessories.Barbs;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Tiles;
using AQMod.Tiles.Furniture;
using AQMod.Tiles.Nature.CrabCrevice;
using AQMod.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public static class CrabCrevice
    {
        private const int Size = 160;

        private struct Circle
        {
            public int X;
            public int Y;
            public int Radius;

            public bool IsInvalid => X == -1;

            public Circle(int x, int y, int radius)
            {
                X = x;
                Y = y;
                Radius = radius;
            }

            public bool Inside(int x, int y)
            {
                int x2 = x - X;
                int y2 = y - Y;
                return Math.Sqrt(x2 * x2 + y2 * y2) <= Radius;
            }
            public double Distance(int x, int y)
            {
                int x2 = x - X;
                int y2 = y - Y;
                return Math.Sqrt(x2 * x2 + y2 * y2);
            }
            public Circle GetRandomCircleInsideCircle(int minDistanceFromEdge, int minScale, int maxScale, UnifiedRandom rand)
            {
                List<Point> testPoints = new List<Point>();
                for (int i = 0; i < Radius * 2; i++)
                {
                    for (int j = 0; j < Radius * 2; j++)
                    {
                        int x = X + i - Radius;
                        int y = Y + j - Radius;
                        if (Distance(x, y) < Radius - minDistanceFromEdge)
                        {
                            if (Main.tile[x, y] == null)
                            {
                                Main.tile[x, y] = new Tile();
                                continue;
                            }
                            if (!Main.tile[x, y].active() || !Main.tile[x, y].Solid())
                            {
                                continue;
                            }
                            testPoints.Add(new Point(x, y));
                        }
                    }
                }
                for (int i = 0; i < testPoints.Count; i++)
                {
                    int chosenPoint = rand.Next(testPoints.Count);
                    int size = rand.Next(minScale, maxScale);
                    for (int j = size; j >= minScale; j--)
                    {
                        var c = FixedCircle(testPoints[chosenPoint].X, testPoints[chosenPoint].Y, j);
                        if (IsValidCircleForGeneratingCave(c))
                        {
                            return c;
                        }
                    }
                    testPoints.RemoveAt(i);
                }
                return new Circle(-1, -1, -1);
            }
        }

        private static Circle FixedCircle(int x, int y, int radius)
        {
            if (x - radius < 10)
            {
                x = radius + 10;
            }
            else if (x + radius > Main.maxTilesX - 10)
            {
                x = Main.maxTilesX - 10 - radius;
            }
            if (y - radius < 10)
            {
                y = radius + 10;
            }
            else if (y + radius > Main.maxTilesY - 10)
            {
                y = Main.maxTilesY - 10 - radius;
            }
            return new Circle(x, y, radius);
        }

        private static bool IsValidCircleForGeneratingCave(int x, int y, int radius)
        {
            return IsValidCircleForGeneratingCave(new Circle(x, y, radius));
        }

        private static bool IsValidCircleForGeneratingCave(Circle circle)
        {
            for (int i = 0; i < circle.Radius * 2; i++)
            {
                for (int j = 0; j < circle.Radius * 2; j++)
                {
                    int x = circle.X + i - circle.Radius;
                    int y = circle.Y + j - circle.Radius;
                    if (circle.Inside(x, y))
                    {
                        for (int k = -3; k <= 3; k++)
                        {
                            for (int l = -3; l <= 3; l++)
                            {
                                if (Main.tile[x + k, y + l] == null)
                                {
                                    Main.tile[x + k, y + l] = new Tile();
                                    return false;
                                }
                                if (!Main.tile[x + k, y + l].active() || !Main.tile[x + k, y + l].Solid())
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

        public static void CreateSandAreaForCrevice(int x, int y)
        {
            if (x - Size < 10)
            {
                x = Size + 10;
            }
            else if (x + Size > Main.maxTilesX - 10)
            {
                x = Main.maxTilesX - 10 - Size;
            }
            if (y - Size < 10)
            {
                y = Size + 10;
            }
            else if (y + Size > Main.maxTilesY - 10)
            {
                y = Main.maxTilesY - 10 - Size;
            }
            List<Point> placeTiles = new List<Point>();
            for (int i = 0; i < Size * 2; i++)
            {
                for (int j = 0; j < Size * 3; j++) // A bit overkill of an extra check, but whatever
                {
                    int x2 = x + i - Size;
                    int y2 = y + j - Size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.6f) <= Size)
                    {
                        if (Main.tile[x2, y2] == null)
                        {
                            Main.tile[x2, y2] = new Tile();
                            continue;
                        }
                        if (Main.tile[x2, y2].active())
                        {
                            placeTiles.Add(new Point(x2, y2));
                        }
                        if (y2 > (int)Main.worldSurface)
                            Main.tile[x2, y2].wall = (ushort)ModContent.WallType<OceanRavineWall>();
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
                            Main.tile[x2 + m, y2 + n].active(active: true);
                            Main.tile[x2 + m, y2 + n].type = TileID.Sand;
                        }
                    }
                }
                else
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (!Main.tile[x2 + m, y2 + n].active() && !Main.tile[x2 + m, y2 + n].Solid() && Main.tile[x2 + m, y2 + n].liquid > 0)
                            {
                                continue;
                            }
                            Main.tile[x2 + m, y2 + n].active(active: true);
                            Main.tile[x2 + m, y2 + n].type = TileID.Sand;
                        }
                    }
                }
            }
        }

        public static bool GenerateCreviceCave(int x, int y, int minScale, int maxScale, int steps)
        {
            List<Circle> validCircles = new List<Circle>();
            for (int i = maxScale; i > minScale; i--)
            {
                var c = FixedCircle(x, y, i);
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
            validCircles.Add(validCircles[0].GetRandomCircleInsideCircle(validCircles[0].Radius / 3, minScale, maxScale, WorldGen.genRand));
            if (validCircles[1].IsInvalid)
            {
                return false;
            }
            for (int i = 0; i < steps; i++)
            {
                int chosenCircle = WorldGen.genRand.Next(validCircles.Count);
                validCircles.Add(validCircles[chosenCircle].GetRandomCircleInsideCircle(validCircles[chosenCircle].Radius / 4, minScale, maxScale, WorldGen.genRand));
                if (validCircles[validCircles.Count - 1].IsInvalid)
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
                                    Main.tile[x2 + m, y2 + n].active(active: true);
                                    Main.tile[x2 + m, y2 + n].type = (ushort)ModContent.TileType<SedimentSand>();
                                    Main.tile[x2 + m, y2 + n].wall = (ushort)ModContent.WallType<OceanRavineWall>();
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
                            Main.tile[x2, y2].active(active: false);
                            Main.tile[x2, y2].liquid = (byte)WorldGen.genRand.Next(minWater, maxWater);
                        }
                    }
                }
            }

            for (int k = 0; k < validCircles.Count; k++)
            {
                for (int i = 0; i < validCircles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < validCircles[k].Radius * 2; j++)
                    {
                        WorldGen.SquareWallFrame(validCircles[k].X + i - validCircles[k].Radius, validCircles[k].Y + j - validCircles[k].Radius, true);
                        WorldGen.SquareTileFrame(validCircles[k].X + i - validCircles[k].Radius, validCircles[k].Y + j - validCircles[k].Radius, true);
                    }
                }
            }
            return true;
        }

        private static void AddVines()
        {
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (Main.tile[i, j].type == ModContent.TileType<SedimentSand>())
                    {
                        if (Main.tile[i, j + 1] == null)
                        {
                            Main.tile[i, j + 1] = new Tile();
                        }
                        else if (Main.tile[i, j + 1].active())
                        {
                            continue;
                        }
                        for (int k = 0; k < 30 && Main.tile[i, j].type == ModContent.TileType<SedimentSand>(); k++)
                            TileLoader.RandomUpdate(i, j, Main.tile[i, j].type);
                    }
                }
            }
        }

        private static void AddExoticCoral(int x, int y)
        {
            if (x - Size < 10)
            {
                x = Size + 10;
            }
            else if (x + Size > Main.maxTilesX - 10)
            {
                x = Main.maxTilesX - 10 - Size;
            }
            if (y - Size < 10)
            {
                y = Size + 10;
            }
            else if (y + Size > Main.maxTilesY - 10)
            {
                y = Main.maxTilesY - 10 - Size;
            }
            for (int i = 0; i < Size * 2; i++)
            {
                for (int j = 0; j < Size * 3; j++)
                {
                    int x2 = x + i - Size;
                    int y2 = y + j - Size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.6f) <= Size)
                    {
                        if (Main.tile[x2, y2] == null)
                        {
                            Main.tile[x2, y2] = new Tile();
                            continue;
                        }
                        if (Main.tile[x2, y2 + 1] == null)
                        {
                            Main.tile[x2, y2 + 1] = new Tile();
                            continue;
                        }
                        if (!Main.tile[x2, y2].active() && Main.tile[x2, y2].liquid == 255 &&
                            Main.tile[x2, y2 + 1].active() && ExoticCoralNew.CanBePlacedOnType(Main.tile[x2, y2 + 1].type))
                        {
                            WorldGen.PlaceTile(x2, y2, ModContent.TileType<ExoticCoralNew>(), mute: true, forced: false, style: ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                        }
                    }
                }
            }
        }

        private static void AddChests(int genX, int genY)
        {
            int count = 0;
            for (int i = 0; i < 25000; i++)
            {
                int chestX = genX + WorldGen.genRand.Next(-120, 120);
                if (chestX < 30 || chestX > Main.maxTilesX - 30)
                {
                    continue;
                }
                int chestY = genY + WorldGen.genRand.Next(-10, 200);
                if (chestY < 30 || chestY > Main.maxTilesY - 30)
                {
                    continue;
                }
                WorldGen.PlaceTile(chestX, chestY, TileID.Containers, mute: true, forced: false, ChestStyles.Palm);
                if (Main.tile[chestX, chestY].type == TileID.Containers)
                {
                    FillWithLoot(Chest.FindChest(chestX, chestY - 1), count);
                    count++;
                    i += 2500;
                }
            }
        }

        private static void FillWithLoot(int chest, int count)
        {
            if (chest == -1 || Main.chest[chest] == null)
                return;
            var c = Main.chest[chest];
            int i = 0;
            switch (count % 3)
            {
                default:
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<StarPhish>());
                    }
                    break;
                case 1:
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<VineSword>());
                    }
                    break;
                case 2:
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<CrabBarb>());
                    }
                    break;
            }
            i++;
        }

        public static void GenerateCrabCrevice(GenerationProgress progress)
        {
            if (!ModContent.GetInstance<WorldGenOptions>().generateOceanRavines)
            {
                return;
            }
            progress.Message = Language.GetTextValue("Mods.AQMod.Common.CrabCrevice");
            int crabCreviceLocationX = 0;
            int crabCreviceLocationY = 0;
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    x = Main.maxTilesX - x;
                for (int j = 200; j < Main.worldSurface; j++)
                {
                    if (CanPlaceLegacyOceanRavine(x, j))
                    {
                        crabCreviceLocationX = x;
                        crabCreviceLocationY = j;
                        int style = WorldGen.genRand.Next(3);
                        PlaceLegacyOceanRavine(x, j, style);
                        i += 1000;
                        break;
                    }
                }
            }
            CreateSandAreaForCrevice(crabCreviceLocationX, crabCreviceLocationY + 40);
            for (int k = 0; k < 20000; k++)
            {
                int caveX = crabCreviceLocationX + WorldGen.genRand.Next(-120, 120);
                int caveY = crabCreviceLocationY + WorldGen.genRand.Next(-10, 200);
                int minScale = WorldGen.genRand.Next(4, 8);
                if (GenerateCreviceCave(caveX, caveY, minScale, minScale + WorldGen.genRand.Next(4, 18), WorldGen.genRand.Next(80, 250)))
                {
                    k += 650;
                }
            }
            AddChests(crabCreviceLocationX, crabCreviceLocationY);
            AddExoticCoral(crabCreviceLocationX, crabCreviceLocationY);
            AddVines();
        }

        public static void GenerateLegacyRavines(GenerationProgress progress)
        {
            if (!ModContent.GetInstance<WorldGenOptions>().generateOceanRavines)
            {
                return;
            }
            progress.Message = Language.GetTextValue("Mods.AQMod.Common.OceanRavines");
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    x = Main.maxTilesX - x;
                for (int j = 200; j < Main.worldSurface; j++)
                {
                    if (CanPlaceLegacyOceanRavine(x, j))
                    {
                        int style = WorldGen.genRand.Next(3);
                        PlaceLegacyOceanRavine(x, j, style);
                        i += 1000;
                        break;
                    }
                }
            }
        }

        public static bool CanPlaceLegacyOceanRavine(int x, int y)
        {
            return !Framing.GetTileSafely(x, y).active() && Main.tile[x, y].liquid > 0 && Framing.GetTileSafely(x, y + 1).active() && Main.tileSand[Main.tile[x, y + 1].type];
        }

        public static void PlaceLegacyOceanRavine(int x, int y, int mossStyle = -1, int tileType = TileID.HardenedSand, int tileType2 = TileID.Sand, int wallType = 0)
        {
            if (wallType == 0)
                wallType = ModContent.WallType<Walls.OceanRavineWall>();
            int height = WorldGen.genRand.Next(40, 120);
            int digDir = x < Main.maxTilesX / 2 ? 1 : -1;
            int[] xAdds = new int[height];
            int x3 = x;
            int x2 = x;
            for (int i = 0; i < height; i++)
            {
                if (WorldGen.genRand.NextBool())
                {
                    xAdds[i] = digDir;
                    x += xAdds[i];
                }
                for (int j = 0; j < 20; j++)
                {
                    WorldGen.PlaceTile(x - 10 + j, y + i, tileType, true, true);
                    Framing.GetTileSafely(x - 10 + j, y + i).wall = (ushort)wallType;
                }
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(15, 25), 5, tileType, true);
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(20, 28), 5, tileType2, true);
            }
            for (int i = 0; i < 10; i++)
            {
                WorldGen.digTunnel(x2, y - i, digDir, 1f, 2, 6, true);
                for (int j = 0; j < 20; j++)
                {
                    Framing.GetTileSafely(x - 10 + j, y + height + i).wall = (ushort)wallType;
                }
            }
            for (int i = 0; i < height; i++)
            {
                x2 += xAdds[i];
                WorldGen.digTunnel(x2, y + i, digDir, 1f, 2, 6, true);
            }
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 28), 6, tileType, true);
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 33), 6, tileType2, true);
            WorldGen.digTunnel(x - digDir, y + height - 6, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 5, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 4, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 3, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 2, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 1, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height, digDir, 1f, 5, 6, true);
            if (mossStyle != -1)
            {
                int mossTile = ModContent.TileType<Tiles.Nature.CrabCrevice.ArgonMossSand>();
                if (mossStyle == 1)
                {
                    mossTile = ModContent.TileType<Tiles.Nature.CrabCrevice.KryptonMossSand>();
                }
                else if (mossStyle == 2)
                {
                    mossTile = ModContent.TileType<Tiles.Nature.CrabCrevice.XenonMossSand>();
                }
                for (int n = 0; n < height; n++)
                {
                    x3 += xAdds[n];
                    for (int m = -10; m < 20; m++)
                    {
                        GrassTileType.SpreadGrassToSurroundings(x3 + m, y + n, TileID.Sand, mossTile);
                        GrassTileType.SpreadGrassToSurroundings(x3 + m, y + n, TileID.HardenedSand, mossTile);
                        WorldGen.SquareTileFrame(x3 + m, y + n, true);
                    }
                    if (WorldGen.genRand.NextBool(5))
                    {
                        for (int m = 0; m < 15; m++)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                if (!Framing.GetTileSafely(x3 + m, y + n).active() && Main.tile[x3 + m, y + n].liquid > 0)
                                {
                                    WorldGen.PlaceTile(x3 + m, y + n, ModContent.TileType<Torches>(), true, false, -1, mossStyle + 3);
                                    if (Framing.GetTileSafely(x3 + m, y + n).active() && Main.tile[x3 + m, y + n].type == ModContent.TileType<Torches>())
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void placeLegacyOceanRavine(int x, int y, int torchStyle = -1, int tileType = TileID.Sandstone, int tileType2 = TileID.HardenedSand, int wallType = 0)
        {
            if (wallType == 0)
                wallType = ModContent.WallType<Walls.OceanRavineWall>();
            int height = WorldGen.genRand.Next(40, 120);
            int digDir = x < Main.maxTilesX / 2 ? 1 : -1;
            int[] xAdds = new int[height];
            int x3 = x;
            int x2 = x;
            for (int i = 0; i < height; i++)
            {
                if (WorldGen.genRand.NextBool())
                {
                    xAdds[i] = digDir;
                    x += xAdds[i];
                }
                for (int j = 0; j < 20; j++)
                {
                    WorldGen.PlaceTile(x - 10 + j, y + i, tileType, true, true);
                    Framing.GetTileSafely(x - 10 + j, y + i).wall = (ushort)wallType;
                }
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(15, 25), 5, tileType, true);
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(20, 28), 5, tileType2, true);
            }
            for (int i = 0; i < 10; i++)
            {
                WorldGen.digTunnel(x2, y - i, digDir, 1f, 2, 6, true);
                for (int j = 0; j < 20; j++)
                {
                    Framing.GetTileSafely(x - 10 + j, y + height + i).wall = (ushort)wallType;
                }
            }
            for (int i = 0; i < height; i++)
            {
                x2 += xAdds[i];
                WorldGen.digTunnel(x2, y + i, digDir, 1f, 2, 6, true);
            }
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 28), 6, tileType, true);
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 33), 6, tileType2, true);
            WorldGen.digTunnel(x - digDir, y + height - 6, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 5, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 4, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 3, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 2, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 1, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height, digDir, 1f, 5, 6, true);
            if (torchStyle != -1)
            {
                for (int i = 0; i < height; i++)
                {
                    x3 += xAdds[i];
                    if (WorldGen.genRand.NextBool(5))
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                if (!Framing.GetTileSafely(x3 + j, y + i).active() && Main.tile[x3 + j, y + i].liquid > 0)
                                {
                                    WorldGen.PlaceTile(x3 + j, y + i, ModContent.TileType<Torches>(), true, false, -1, torchStyle);
                                    if (Framing.GetTileSafely(x3 + j, y + i).active() && Main.tile[x3 + j, y + i].type == ModContent.TileType<Torches>())
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}