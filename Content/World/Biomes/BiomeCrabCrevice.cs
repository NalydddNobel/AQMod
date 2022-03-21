using AQMod.Common.ID;
using AQMod.Content.Players;
using AQMod.Dusts.Splashes;
using AQMod.Gores;
using AQMod.Gores.Droplets;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.HookUpgrades;
using AQMod.Items.Misc;
using AQMod.Items.Tools.Fishing;
using AQMod.Items.Weapons.Melee.Clicker;
using AQMod.Items.Weapons.Ranged;
using AQMod.Tiles;
using AQMod.Tiles.CrabCrevice;
using AQMod.Tiles.Furniture;
using AQMod.Tiles.PetrifiedFurn;
using AQMod.Tiles.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace AQMod.Content.World.Biomes
{
    public sealed class BiomeCrabCrevice : ModWorld
    {
        public sealed class CrabCreviceSurfaceWater : ModWaterStyle
        {
            public override bool ChooseWaterStyle()
            {
                var biomes = Main.player[Main.myPlayer].Biomes();
                return biomes.zoneCrabSeason || (biomes.zoneCrabCrevice && Main.player[Main.myPlayer].position.Y < Main.worldSurface * 16f);
            }

            public override int ChooseWaterfallStyle()
                => ModContent.GetInstance<CrabCreviceSurfaceWaterfall>().Type;

            public override int GetSplashDust()
                => ModContent.DustType<CrabSeasonSplash>();

            public override int GetDropletGore()
                => AQGore.GetID<CrabSeasonDroplet>();

            public override void LightColorMultiplier(ref float r, ref float g, ref float b)
            {
                r = 1f;
                g = 1f;
                b = 1f;
            }

            public override Color BiomeHairColor()
                => Color.SandyBrown;
        }
        public sealed class CrabCreviceSurfaceWaterfall : ModWaterfallStyle
        {
        }
        public sealed class CrabCreviceUndergroundWater : ModWaterStyle
        {
            public override bool ChooseWaterStyle()
            {
                return Main.player[Main.myPlayer].GetModPlayer<PlayerBiomes>().zoneCrabCrevice && Main.player[Main.myPlayer].position.Y >= Main.worldSurface * 16f;
            }

            public override int ChooseWaterfallStyle()
                => ModContent.GetInstance<CrabCreviceUndergroundWaterfall>().Type;

            public override int GetSplashDust()
                => ModContent.DustType<CrabCreviceSplash>();

            public override int GetDropletGore()
                => AQGore.GetID<CrabCreviceDroplet>();

            public override void LightColorMultiplier(ref float r, ref float g, ref float b)
            {
                r = 1f;
                g = 1f;
                b = 1f;
            }

            public override Color BiomeHairColor()
                => Color.SandyBrown;
        }
        public sealed class CrabCreviceUndergroundWaterfall : ModWaterfallStyle
        {
        }

        private const int Size = 160;
        internal static List<Vector3> platformGenList;
        private static int PirateChestCount = 0;

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
            public Circle GetRandomCircleInsideCircleNoAirCheck(int minDistanceFromEdge, int minScale, int maxScale, UnifiedRandom rand)
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
                int chosenPoint = rand.Next(testPoints.Count);
                int size = rand.Next(minScale, maxScale);
                return FixedCircle(testPoints[chosenPoint].X, testPoints[chosenPoint].Y, size);
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            platformGenList = new List<Vector3>();
            int i = tasks.FindIndex((t) => t.Name.Equals("Beaches"));
            if (i != -1)
            {
                tasks.Insert(i + 1, new PassLegacy("AQMod: Crab Crevice", GenerateCrabCrevice));
            }
        }

        public override void PostWorldGen()
        {
            ReplacePlatformsViaGenList();
        }

        private static bool CanOverwriteTile(Tile tile)
        {
            return !Main.tileDungeon[tile.type] && !Main.wallDungeon[tile.wall];
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
                                if ((!Main.tile[x + k, y + l].active() || !Main.tile[x + k, y + l].Solid()) && CanOverwriteTile(Main.tile[x + k, y + l]))
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
                        if (CanOverwriteTile(Main.tile[x2, y2]))
                        {
                            if (Main.tile[x2, y2].active())
                                placeTiles.Add(new Point(x2, y2));
                            if (y2 > (int)Main.worldSurface)
                                Main.tile[x2, y2].wall = (ushort)ModContent.WallType<OceanRavineWall>();
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

        private static bool HasUnOverwriteableTiles(Circle circle)
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
        private static bool CanGenCrustaciumVein(Circle circle)
        {
            bool anySand = false;
            for (int i = 0; i < circle.Radius * 2; i++)
            {
                for (int j = 0; j < circle.Radius * 2; j++)
                {
                    int x2 = circle.X + i - circle.Radius;
                    int y2 = circle.Y + j - circle.Radius;
                    if (circle.Inside(x2, y2))
                    {
                        if (Main.tile[x2, y2].type == TileID.Sand)
                        {
                            anySand = true;
                        }
                        else if (Main.tile[x2, y2].type == ModContent.TileType<CrustaciumFlesh>() || Main.tile[x2, y2].type == ModContent.TileType<CrustaciumShell>() || !CanOverwriteTile(Main.tile[x2, y2]))
                        {
                            return false;
                        }
                    }
                }
            }
            return anySand;
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
            if (validCircles[1].IsInvalid || HasUnOverwriteableTiles(validCircles[1]))
            {
                return false;
            }
            for (int i = 0; i < steps; i++)
            {
                int chosenCircle = WorldGen.genRand.Next(validCircles.Count);
                validCircles.Add(validCircles[chosenCircle].GetRandomCircleInsideCircle(validCircles[chosenCircle].Radius / 4, minScale, maxScale, WorldGen.genRand));
                if (validCircles[validCircles.Count - 1].IsInvalid || HasUnOverwriteableTiles(validCircles[validCircles.Count - 1]))
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
                            if (minWater > 100 && Main.tile[x2, y2 + 1].active() && Main.tile[x2, y2 + 1].Solid())
                            {
                                Main.tile[x2, y2].liquid = 255;
                            }
                            else
                            {
                                Main.tile[x2, y2].liquid = (byte)WorldGen.genRand.Next(minWater, maxWater);
                            }
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
                    if (Main.tile[i, j].type == ModContent.TileType<SedimentSand>() || Main.tile[i, j].type == ModContent.TileType<PetrifiedWood>())
                    {
                        if (Main.tile[i, j + 1] == null)
                        {
                            Main.tile[i, j + 1] = new Tile();
                        }
                        else if (Main.tile[i, j + 1].active())
                        {
                            continue;
                        }
                        for (int k = 0; k < 30; k++)
                            TileLoader.RandomUpdate(i, j, Main.tile[i, j].type);
                    }
                }
            }
        }

        private static bool AddCrustacium(int x, int y, int power, int steps)
        {
            List<Circle> circles = new List<Circle>();
            for (int i = power * 2; i > power; i--)
            {
                var c = FixedCircle(x, y, i);
                if (IsValidCircleForGeneratingCave(c))
                {
                    circles.Add(c);
                    break;
                }
            }
            if (circles.Count == 0)
            {
                return false;
            }
            for (int i = 1; i < steps; i++)
            {
                var c = FixedCircle(x + WorldGen.genRand.Next(-i, i), y + WorldGen.genRand.Next(-i, i), WorldGen.genRand.Next(power, power * 2));
                if (!CanGenCrustaciumVein(c))
                {
                    return false;
                }
                circles.Add(c);
            }
            for (int k = 0; k < circles.Count; k++)
            {
                for (int i = 0; i < circles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < circles[k].Radius * 2; j++)
                    {
                        int x2 = circles[k].X + i - circles[k].Radius;
                        int y2 = circles[k].Y + j - circles[k].Radius;
                        if (circles[k].Inside(x2, y2) && Main.tile[x2, y2].active() && Main.tile[x2, y2].Solid())
                        {
                            Main.tile[x2, y2].active(active: true);
                            Main.tile[x2, y2].type = (ushort)ModContent.TileType<CrustaciumFlesh>();
                        }
                    }
                }
            }

            for (int k = 0; k < circles.Count; k++)
            {
                for (int i = 0; i < circles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < circles[k].Radius * 2; j++)
                    {
                        int x2 = circles[k].X + i - circles[k].Radius;
                        int y2 = circles[k].Y + j - circles[k].Radius;
                        if (circles[k].Inside(x2, y2) && Main.tile[x2, y2].active() &&
                            Main.tile[x2, y2].type == ModContent.TileType<CrustaciumFlesh>())
                        {
                            if (CrustaciumFlesh.ShouldConvertToShell(x2, y2))
                            {
                                Main.tile[x2, y2].type = (ushort)ModContent.TileType<CrustaciumShell>();
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < circles.Count; k++)
            {
                for (int i = 0; i < circles[k].Radius * 2; i++)
                {
                    for (int j = 0; j < circles[k].Radius * 2; j++)
                    {
                        WorldGen.SquareWallFrame(circles[k].X + i - circles[k].Radius, circles[k].Y + j - circles[k].Radius, true);
                        WorldGen.SquareTileFrame(circles[k].X + i - circles[k].Radius, circles[k].Y + j - circles[k].Radius, true);
                    }
                }
            }
            return true;
        }

        private static void AddChests(int genX, int genY)
        {
            int count = 0;
            for (int i = 0; i < 50000; i++)
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
                if (Main.tile[chestX, chestY] == null)
                {
                    Main.tile[chestX, chestY] = new Tile();
                    continue;
                }
                if (Main.tile[chestX, chestY + 1] == null)
                {
                    Main.tile[chestX, chestY + 1] = new Tile();
                    continue;
                }
                if (Main.tile[chestX + 1, chestY] == null)
                {
                    Main.tile[chestX + 1, chestY] = new Tile();
                    continue;
                }
                if (Main.tile[chestX + 1, chestY + 1] == null)
                {
                    Main.tile[chestX + 1, chestY + 1] = new Tile();
                    continue;
                }
                if (Main.tile[chestX, chestY - 1] == null)
                {
                    Main.tile[chestX, chestY - 1] = new Tile();
                    continue;
                }
                if (Main.tile[chestX + 1, chestY - 1] == null)
                {
                    Main.tile[chestX + 1, chestY - 1] = new Tile();
                    continue;
                }
                if (Main.tile[chestX, chestY + 1].active() && Main.tile[chestX, chestY + 1].Solid()
                    && Main.tile[chestX + 1, chestY + 1].active() && Main.tile[chestX + 1, chestY + 1].Solid() && Main.tile[chestX, chestY].wall == ModContent.WallType<OceanRavineWall>())
                {
                    bool validSpot = true;
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (Main.tile[chestX + k, chestY + l - 1].active() &&
                                !Main.tileCut[Main.tile[chestX + k, chestY + l - 1].type] &&
                                Main.tile[chestX + k, chestY + l - 1].type != ModContent.TileType<ExoticCoralNew>())
                            {
                                validSpot = false;
                                k = 2;
                                break;
                            }
                        }
                    }
                    if (validSpot)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                Main.tile[chestX + k, chestY + l - 1].active(active: false);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    int chest = WorldGen.PlaceChest(chestX, chestY, TileID.Containers, style: ChestStyles.Palm);
                    if (chest != -1 && Main.tile[chestX, chestY].type == TileID.Containers)
                    {
                        FillPalmChest(Chest.FindChest(chestX, chestY - 1), count);
                        count++;
                        i += 3500;
                    }
                }
            }
        }

        private static void FillPalmChest(int chest, int count)
        {
            if (chest == -1 || Main.chest[chest] == null)
                return;
            var c = Main.chest[chest];
            int i = 0;
            switch (count % 4)
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
                case 3:
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
                    }
                    break;
            }
            i++;
            if (WorldGen.genRand.NextBool())
            {
                switch (WorldGen.genRand.Next(3))
                {
                    case 0:
                        {
                            c.item[i].SetDefaults(ItemID.DivingHelmet);
                            i++;
                        }
                        break;

                    case 1:
                        {
                            c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
                            i++;
                        }
                        break;

                    case 2:
                        {
                            c.item[i].SetDefaults(ModContent.ItemType<YuckyOrb>());
                            i++;
                        }
                        break;
                }
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.GillsPotion);
                c.item[i].stack = WorldGen.genRand.Next(1, 4);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.WaterWalkingPotion);
                c.item[i].stack = WorldGen.genRand.Next(1, 4);
                i++;
            }
            if (WorldGen.genRand.NextBool(4))
            {
                c.item[i].SetDefaults(ItemID.IronCrate);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.HealingPotion);
                c.item[i].stack = 2 + WorldGen.genRand.Next(5);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.WoodenCrate);
                c.item[i].stack = 1 + WorldGen.genRand.Next(2);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.PinkJellyfish);
                c.item[i].stack = 1 + WorldGen.genRand.Next(2);
                i++;
            }
            c.item[i].SetDefaults(ItemID.Glowstick);
            c.item[i].stack = WorldGen.genRand.Next(80, 200);
            i++;
        }

        private static void FillPirateChest(int chest, int count)
        {
            if (chest == -1 || Main.chest[chest] == null)
                return;
            var c = Main.chest[chest];
            int i = 0;
            switch (count % 4)
            {
                default:
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<PearlAmulet>());
                    }
                    break;
            }
            i++;
            if (WorldGen.genRand.NextBool())
            {
                switch (WorldGen.genRand.Next(3))
                {
                    case 0:
                        {
                            c.item[i].SetDefaults(ItemID.DivingHelmet);
                            i++;
                        }
                        break;

                    case 1:
                        {
                            c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
                            i++;
                        }
                        break;

                    case 2:
                        {
                            c.item[i].SetDefaults(ModContent.ItemType<YuckyOrb>());
                            i++;
                        }
                        break;
                }
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.GillsPotion);
                c.item[i].stack = WorldGen.genRand.Next(1, 4);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.WaterWalkingPotion);
                c.item[i].stack = WorldGen.genRand.Next(1, 4);
                i++;
            }
            if (WorldGen.genRand.NextBool(4))
            {
                c.item[i].SetDefaults(ItemID.IronCrate);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.HealingPotion);
                c.item[i].stack = 2 + WorldGen.genRand.Next(5);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.WoodenCrate);
                c.item[i].stack = 1 + WorldGen.genRand.Next(2);
                i++;
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[i].SetDefaults(ItemID.PinkJellyfish);
                c.item[i].stack = 1 + WorldGen.genRand.Next(2);
                i++;
            }
            c.item[i].SetDefaults(ItemID.Glowstick);
            c.item[i].stack = WorldGen.genRand.Next(80, 200);
            i++;
        }

        public static void GenPirateShip(int x, int y)
        {
            GenPirateShip(x, y, WorldGen.genRand.NextBool() ? 1 : -1);
        }

        public static void GenPirateShip(int x, int y, int direction)
        {
            GenPirateShip(x, y, direction, WorldGen.genRand.Next(35, 51));
        }

        public static void GenPirateShip(int x, int y, int direction, int length)
        {
            if (platformGenList == null)
            {
                platformGenList = new List<Vector3>();
            }
            if (x < 40)
            {
                x = 40;
            }
            else if (x > Main.maxTilesX - 40)
            {
                x = Main.maxTilesX - 40;
            }
            if (direction == -1)
            {
                if (x - length < 20)
                {
                    x = length + 20;
                }
            }
            else
            {
                if (x + length > Main.maxTilesX - 20)
                {
                    x = Main.maxTilesX - 20 - length;
                }
            }
            for (int k = 0; k < length; k++)
            {
                Main.tile[x + k * direction, y].active(active: true);
                Main.tile[x + k * direction, y].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + k * direction, y].slope(slope: 0);
                Main.tile[x + k * direction, y].halfBrick(halfBrick: false);
                if (k >= 2 && k < length - 8)
                {
                    if (k > 3 && k < 9)
                    {
                        Main.tile[x + k * direction, y].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
                        Main.tile[x + k * direction, y].frameX = 0;
                        Main.tile[x + k * direction, y].frameX = 0;
                    }
                    Main.tile[x + k * direction, y + 8].active(active: true);
                    Main.tile[x + k * direction, y + 8].type = (ushort)ModContent.TileType<PetrifiedWood>();
                    Main.tile[x + k * direction, y + 8].slope(slope: 0);
                    Main.tile[x + k * direction, y + 8].halfBrick(halfBrick: false);
                    for (int l = 0; l < 9; l++)
                    {
                        Main.tile[x + k * direction, y + l].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
                    }
                    for (int l = 1; l < 8; l++)
                    {
                        Main.tile[x + k * direction, y + l].active(active: false);
                    }
                }
                if (k >= 8 && k < length - 12)
                {
                    if (k < 13)
                    {
                        Main.tile[x + k * direction, y + 8].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
                        Main.tile[x + k * direction, y + 8].frameX = 0;
                        Main.tile[x + k * direction, y + 8].frameX = 0;
                    }
                    Main.tile[x + k * direction, y + 13].active(active: true);
                    Main.tile[x + k * direction, y + 13].type = (ushort)ModContent.TileType<PetrifiedWood>();
                    Main.tile[x + k * direction, y + 13].slope(slope: 0);
                    Main.tile[x + k * direction, y + 13].halfBrick(halfBrick: false);
                    for (int l = 0; l < 5; l++)
                    {
                        Main.tile[x + k * direction, y + 9 + l].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
                    }
                    for (int l = 1; l < 5; l++)
                    {
                        Main.tile[x + k * direction, y + 8 + l].active(active: false);
                    }
                }
            }
            for (int k = 0; k < 10; k++)
            {
                Main.tile[x + (k + length - 5) * direction, y - 1].active(active: true);
                Main.tile[x + (k + length - 5) * direction, y - 1].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + (k + length - 5) * direction, y - 1].slope(slope: 0);
                Main.tile[x + (k + length - 5) * direction, y - 1].halfBrick(halfBrick: false);
            }
            Main.tile[x + (length - 6) * direction, y - 1].active(active: true);
            Main.tile[x + (length - 6) * direction, y - 1].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            Main.tile[x + (length - 6) * direction, y - 1].frameX = 0;
            Main.tile[x + (length - 6) * direction, y - 1].frameY = 0;
            Main.tile[x + (length - 6) * direction, y - 1].halfBrick(halfBrick: false);
            if (direction == -1)
            {
                Main.tile[x + (length - 6) * direction, y - 1].slope(slope: 1);
            }
            else
            {
                Main.tile[x + (length - 6) * direction, y - 1].slope(slope: 2);
            }

            // platform at tip
            Main.tile[x + (length + 5) * direction, y - 1].active(active: true);
            Main.tile[x + (length + 5) * direction, y - 1].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            Main.tile[x + (length + 5) * direction, y - 1].frameX = 0;
            Main.tile[x + (length + 5) * direction, y - 1].frameY = 0;
            Main.tile[x + (length + 5) * direction, y - 1].slope(slope: 0);
            Main.tile[x + (length + 5) * direction, y - 1].halfBrick(halfBrick: false);
            // back wall
            for (int k = 0; k < 9; k++)
            {
                if (k < 6)
                {
                    Main.tile[x + 1 * direction, y + k].active(active: true);
                    Main.tile[x + 1 * direction, y + k].type = (ushort)ModContent.TileType<PetrifiedWood>();
                    Main.tile[x + 1 * direction, y + k].slope(slope: 0);
                    Main.tile[x + 1 * direction, y + k].halfBrick(halfBrick: false);
                    if (k < 3)
                    {
                        Main.tile[x, y + k].active(active: true);
                        Main.tile[x, y + k].type = (ushort)ModContent.TileType<PetrifiedWood>();
                        Main.tile[x, y + k].slope(slope: 0);
                        Main.tile[x, y + k].halfBrick(halfBrick: false);
                    }
                }
                Main.tile[x + 2 * direction, y + k].active(active: true);
                Main.tile[x + 2 * direction, y + k].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + 2 * direction, y + k].slope(slope: 0);
                Main.tile[x + 2 * direction, y + k].halfBrick(halfBrick: false);
                Main.tile[x + 3 * direction, y + k].active(active: true);
                Main.tile[x + 3 * direction, y + k].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + 3 * direction, y + k].slope(slope: 0);
                Main.tile[x + 3 * direction, y + k].halfBrick(halfBrick: false);
            }

            for (int k = 0; k < 5; k++)
            {
                for (int l = 0; l <= k; l++)
                {
                    Main.tile[x + (k + 3) * direction, y + l + 9].active(active: true);
                    Main.tile[x + (k + 3) * direction, y + l + 9].type = (ushort)ModContent.TileType<PetrifiedWood>();
                    Main.tile[x + (k + 3) * direction, y + l + 9].slope(slope: 0);
                    Main.tile[x + (k + 3) * direction, y + l + 9].halfBrick(halfBrick: false);
                }
            }

            // wall front
            for (int k = 0; k < 13; k++)
            {
                Main.tile[x + (length - k - 1) * direction, y + k].active(active: true);
                Main.tile[x + (length - k - 1) * direction, y + k].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + (length - k - 1) * direction, y + k].slope(slope: 0);
                Main.tile[x + (length - k - 1) * direction, y + k].halfBrick(halfBrick: false);

                Main.tile[x + (length - k - 1) * direction, y + k + 1].active(active: true);
                Main.tile[x + (length - k - 1) * direction, y + k + 1].type = (ushort)ModContent.TileType<PetrifiedWood>();
                Main.tile[x + (length - k - 1) * direction, y + k + 1].slope(slope: 0);
                Main.tile[x + (length - k - 1) * direction, y + k + 1].halfBrick(halfBrick: false);
                for (int l = 0; l <= k; l++)
                {
                    Main.tile[x + (length - k - 1) * direction, y + l + 1].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
                }
            }

            int sailHeight = WorldGen.genRand.Next(18, 30);
            int sailX = x + length / 2 * direction;

            for (int k = 1; k < sailHeight; k++)
            {
                Main.tile[sailX, y - k].active(active: true);
                Main.tile[sailX, y - k].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
                Main.tile[sailX, y - k].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
                Main.tile[sailX, y - k].slope(slope: 0);
                Main.tile[sailX, y - k].halfBrick(halfBrick: false);
                if (Main.tile[sailX, y - k - 3].active() && Main.tile[sailX, y - k - 3].Solid())
                {
                    sailHeight = k + 1;
                    break;
                }
                if (k % 9 == 0)
                {
                    int sailWidth = WorldGen.genRand.Next(12, 18);
                    int down = 0;
                    int pdown = 0;
                    for (int m = -sailWidth; m < sailWidth; m++)
                    {
                        int a = m * direction;
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                if (Main.tile[sailX + a + i, y - k + down + j].active() &&
                                    Main.tile[sailX + a + i, y - k + down + j].type != ModContent.TileType<PetrifiedWood>() &&
                                    Main.tile[sailX + a + i, y - k + down + j].type != ModContent.TileType<PetrifiedWoodPlatform>() &&
                                    Main.tile[sailX + a + i, y - k + down + j].Solid())
                                {
                                    if (m < 0)
                                        goto Next;
                                    else
                                        goto End;
                                }
                            }
                        }

                        Main.tile[sailX + a, y - k + down].active(active: true);
                        Main.tile[sailX + a, y - k + down].type = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
                        Main.tile[sailX + a, y - k + down].halfBrick(halfBrick: false);
                        if (pdown == down && m > -sailWidth + 3 && m.Abs() > 2 && m < sailWidth - 3 && WorldGen.genRand.NextBool(8))
                        {
                            if (direction == -1)
                            {
                                Main.tile[sailX + a, y - k + down].slope(slope: 2);
                                platformGenList.Add(new Vector3(sailX + a, y - k + down, 2));
                            }
                            else
                            {
                                platformGenList.Add(new Vector3(sailX + a, y - k + down, 1));
                                Main.tile[sailX + a, y - k + down].slope(slope: 1);
                            }
                            down++;
                        }
                        else
                        {
                            pdown = down;
                            Main.tile[sailX + a, y - k + down].slope(slope: 0);
                        }

                    Next:
                        continue;
                    End:
                        break;
                    }
                }
            }

            int tableX = WorldGen.genRand.Next(8, length - 10) * direction;
            WorldGen.PlaceTile(x + tableX, y + 7, ModContent.TileType<AQTables>(), mute: true, forced: true, style: AQTables.PetrifiedWood);
            if (WorldGen.genRand.NextBool())
            {
                WorldGen.PlaceTile(x + tableX - 2, y + 7, ModContent.TileType<AQChairs>(), mute: true, forced: true, style: AQChairs.PetrifiedWood);
            }
            else
            {
                WorldGen.PlaceTile(x + tableX + 2, y + 7, ModContent.TileType<AQChairs>(), mute: true, forced: true, style: AQChairs.PetrifiedWood);
            }
            //WorldGen.PlaceTile(x + tableX + 3, y + 6, TileID.Dirt, mute: true, forced: true, style: AQChairs.PetrifiedWood);

            int chest = WorldGen.PlaceChest(x + 7 * direction, y + 7, TileID.Containers, style: ChestStyles.Gold);
            if (chest != -1)
            {
                FillPirateChest(chest, PirateChestCount);
                PirateChestCount++;
            }

            for (int k = 0; k < length + 10; k++)
            {
                for (int l = -sailHeight; l < 20; l++)
                {
                    WorldGen.SquareTileFrame(x + k * direction, y + l, resetFrame: true);
                    WorldGen.SquareWallFrame(x + k * direction, y + l, resetFrame: true);
                }
            }
        }

        internal static void ReplacePlatformsViaGenList()
        {
            if (platformGenList == null)
                return;
            foreach (var v in platformGenList)
            {
                var p = new Point((int)v.X, (int)v.Y);
                WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<PetrifiedWoodPlatform>(), mute: true, forced: true);
                Main.tile[p.X, p.Y].halfBrick(halfBrick: false);
                Main.tile[p.X, p.Y].slope(slope: (byte)(int)v.Z);
                Main.tile[p.X, p.Y].frameX = (short)TileUtils.FrameForPlatformSloping(Main.tile[p.X, p.Y].slope());
                Main.tile[p.X, p.Y].frameY = 0;
                WorldGen.SquareTileFrame(p.X, p.Y, resetFrame: true);
            }
            platformGenList = null;
        }

        public static void GenerateCrabCrevice(GenerationProgress progress)
        {
            PirateChestCount = 0;
            if (!AQConfigServer.Instance.generateOceanRavines)
            {
                return;
            }
            if (platformGenList == null)
                platformGenList = new List<Vector3>();
            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice");
            int crabCreviceLocationX = 0;
            int crabCreviceLocationY = 0;
            int reccomendedDir = 0;
            if (AQMod.calamityMod.IsActive)
            {
                reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1;
            }
            else if (AQMod.thoriumMod.IsActive)
            {
                reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
            }
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    x = Main.maxTilesX - x;
                for (int j = 200; j < Main.worldSurface; j++)
                {
                    if (CanPlaceLegacyOceanRavine(x, j))
                    {
                        if (reccomendedDir == 0 || crabCreviceLocationX == 0)
                        {
                            crabCreviceLocationX = x;
                            crabCreviceLocationY = j;
                        }
                        else if (reccomendedDir == -1)
                        {
                            if (x * 2 < Main.maxTilesX)
                            {
                                crabCreviceLocationX = x;
                                crabCreviceLocationY = j;
                            }
                        }
                        else
                        {
                            if (x * 2 > Main.maxTilesX)
                            {
                                crabCreviceLocationX = x;
                                crabCreviceLocationY = j;
                            }
                        }
                        int style = WorldGen.genRand.Next(3);
                        PlaceLegacyOceanRavine(x, j, style);
                        i += 1000;
                        break;
                    }
                }
            }

            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.Sand");
            CreateSandAreaForCrevice(crabCreviceLocationX, crabCreviceLocationY + 40);

            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.FinalCave");
            int finalCaveStart = -50;
            int finalCaveX;
            if (crabCreviceLocationX < Main.maxTilesX / 2)
            {
                finalCaveX = crabCreviceLocationX + WorldGen.genRand.Next(60);
            }
            else
            {
                finalCaveX = crabCreviceLocationX + WorldGen.genRand.Next(-60, 0);
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
                var circle = new Circle(finalCaveX + k, crabCreviceLocationY + 180, WorldGen.genRand.Next(2, 14) + ((int)(Math.Sin((finalCaveProgress.Abs() - 0.5f) * MathHelper.Pi) * 9.0)).Abs());
                if (!HasUnOverwriteableTiles(circle))
                {
                    finalCaveCircles.Add(circle);
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
                            Main.tile[x2, y2].active(active: false);
                            if (Main.tile[x2, y2 + 1].active() && Main.tile[x2, y2 + 1].Solid())
                            {
                                Main.tile[x2, y2].liquid = 255;
                            }
                            else
                            {
                                Main.tile[x2, y2].liquid = (byte)WorldGen.genRand.Next(10, 100);
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
                        WorldGen.SquareWallFrame(finalCaveCircles[k].X + i - finalCaveCircles[k].Radius, finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius, true);
                        WorldGen.SquareTileFrame(finalCaveCircles[k].X + i - finalCaveCircles[k].Radius, finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius, true);
                    }
                }
            }

            GenPirateShip(finalCaveX - 50, crabCreviceLocationY + 190 + WorldGen.genRand.Next(-8, 5), 1);
            GenPirateShip(finalCaveX + 50, crabCreviceLocationY + 192 + WorldGen.genRand.Next(-5, 8), -1);

            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.Caves");
            for (int k = 0; k < 20000; k++)
            {
                int caveX = crabCreviceLocationX + WorldGen.genRand.Next(-156, 156);
                int caveY = crabCreviceLocationY + WorldGen.genRand.Next(-10, 220);
                int minScale = WorldGen.genRand.Next(4, 8);
                if (GenerateCreviceCave(caveX, caveY, minScale, minScale + WorldGen.genRand.Next(4, 18), WorldGen.genRand.Next(80, 250)))
                {
                    k += 200;
                }
            }

            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.Ores");
            for (int k = 0; k < 4250000; k++)
            {
                int caveX = crabCreviceLocationX + WorldGen.genRand.Next(-200, 200);
                int caveY = crabCreviceLocationY + WorldGen.genRand.Next(40, 260);
                if (AddCrustacium(caveX, caveY, WorldGen.genRand.Next(4, 12), WorldGen.genRand.Next(3, 8)))
                {
                    k += 400000;
                }
            }

            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.Chests");
            AddChests(crabCreviceLocationX, crabCreviceLocationY);
            progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.CrabCrevice.Vines");
            AddVines();
        }

        public static void GenerateLegacyRavines(GenerationProgress progress)
        {
            if (!AQConfigServer.Instance.generateOceanRavines)
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
                wallType = ModContent.WallType<OceanRavineWall>();
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
                int mossTile = ModContent.TileType<ArgonMossSand>();
                if (mossStyle == 1)
                {
                    mossTile = ModContent.TileType<KryptonMossSand>();
                }
                else if (mossStyle == 2)
                {
                    mossTile = ModContent.TileType<XenonMossSand>();
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
                wallType = ModContent.WallType<OceanRavineWall>();
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