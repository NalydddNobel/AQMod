using Aequus.Content.CrossMod;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Ranged;
using Aequus.Tiles;
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
        public int nextChestLoot;
        public Point location;

        public int LeftX(int sizeX)
        {
            return AequusWorld.Structures.GetLocation("CrabCrevice").GetValueOrDefault(new Point(0, 0)).X < Main.maxTilesX / 2 ? 5 : Main.maxTilesX - sizeX - 5;
        }

        public void Reset()
        {
            nextChestLoot = 0;
            location = new Point();
            size = Main.maxTilesX / 28;
            if (size > 200)
            {
                size -= 200;
                size /= 2;
                size += 200;
            }
        }

        public bool ProperCrabCreviceAnchor(int x, int y)
        {
            return !Main.tile[x, y].HasTile && Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y + 1].HasTile && (Main.tileSand[Main.tile[x, y + 1].TileType] || Main.tile[x, y + 1].TileType == TileID.ShellPile);
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
                for (int j = 0; j < size * 5; j++) // A bit overkill of an extra check, but whatever
                {
                    int x2 = x + i - size;
                    int y2 = y + j - size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.175f) <= size)
                    {
                        if (CanOverwriteTile(Main.tile[x2, y2]))
                        {
                            if (Main.tile[x2, y2].HasTile)
                            {
                                placeTiles.Add(new Point(x2, y2));
                            }
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
                            if (y2 < Main.worldSurface + 25)
                            {
                                if (!WorldGen.genRand.NextBool(25 + ((int)Main.worldSurface - y2) + 2))
                                {
                                    Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                                    continue;
                                }
                            }
                            Main.tile[x2 + m, y2 + n].TileType = TileID.HardenedSand;
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
                                    if (y2 < Main.worldSurface)
                                        Main.tile[x2 + m, y2 + n].WallType = (ushort)ModContent.WallType<SedimentaryRockWallWall>();
                                    else if (y2 < Main.worldSurface - 25 && !WorldGen.genRand.NextBool(25))
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

            if (WorldGen.genRand.NextBool())
            {
                var caverPoint = WorldGen.genRand.Next(validCircles);
                if (caverPoint.Y > location.Y + 80)
                    WorldGen.Caverer(caverPoint.X, caverPoint.Y);
            }
            return true;
        }

        public void GrowWalls(int x, int y)
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
            var placeTiles = new List<Point>();
            for (int i = 0; i < size * 2; i++)
            {
                for (int j = 0; j < size * 6; j++) // A bit overkill of an extra check, but whatever
                {
                    int x2 = x + i - size;
                    int y2 = y + j - size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 * 0.8f + y3 * y3 * 0.125f) <= size)
                    {
                        if (CanOverwriteTile(Main.tile[x2, y2]))
                        {
                            if (Main.tile[x2, y2].HasTile && y2 > (int)Main.worldSurface && WorldGen.genRand.NextBool(16))
                            {
                                if (WorldGen.InWorld(x2, y2, 5))
                                {
                                    bool allowedToCreatePillar = false;
                                    for (int k = -1; k <= 1; k++)
                                    {
                                        for (int l = -1; l <= 1; l++)
                                        {
                                            if (!Main.tile[x2 + k, y2 + l].HasTile)
                                            {
                                                allowedToCreatePillar = true;
                                            }
                                        }
                                    }
                                    if (allowedToCreatePillar)
                                        GrowWormyWall(x2, y2);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GrowWormyWall(int x, int y)
        {
            var velo = new Vector2(WorldGen.genRand.NextFloat(-1f, 1f), WorldGen.genRand.NextFloat(-0.2f, 1f));

            var loc = new Vector2(x, y);
            int size = WorldGen.genRand.Next(2, 5);
            for (int i = 0; i < 1000; i++)
            {
                loc += velo;
                if (velo.Y < 0f)
                    velo.Y *= 0.95f;
                if (velo.Length() < 1f)
                    velo = Vector2.Normalize(velo);
                var p = loc.ToPoint();
                if (!WorldGen.InWorld(p.X, p.Y, 10))
                {
                    if (i > 15)
                        break;
                    continue;
                }
                size = Math.Clamp(size + WorldGen.genRand.Next(-1, 2), 1, 5);
                size *= 2;
                for (int k = -size; k <= size; k++)
                {
                    for (int l = -size; l <= size; l++)
                    {
                        if (new Vector2(k, l).Length() <= size / 2f)
                        {
                            Main.tile[p + new Point(k - size / 2, l - size / 2)].WallType = (ushort)ModContent.WallType<SedimentaryRockWallWall>();
                        }
                    }
                }
                if (Main.tile[p].IsFullySolid())
                {
                    if (i > 15)
                        break;
                }
                size /= 2;
                velo = velo.RotatedBy(WorldGen.genRand.NextFloat(WorldGen.genRand.NextFloat(-0.3f, 0.01f), WorldGen.genRand.NextFloat(0.01f, 0.3f)));
            }
        }

        public void PlaceChests(int leftX, int sizeX)
        {
            Reset();
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY / 128; i++)
            {
                int randX = leftX + WorldGen.genRand.Next(sizeX);
                int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);

                if (Main.tile[randX, randY].TileType == ModContent.TileType<SedimentaryRockTile>())
                {
                    randY--;
                    int chestID = WorldGen.PlaceChest(randX, randY, notNearOtherChests: true, style: ChestType.Palm);
                    if (chestID != -1)
                    {
                        var c = Main.chest[chestID];
                        FillChest(c.item, randY);
                    }
                }
            }
        }

        public void FillChest(Item[] arr, int y)
        {
            int index = 0;
            var rand = WorldGen.genRand;
            switch (nextChestLoot % 4)
            {
                case 0:
                    arr[index++].SetDefaults<StarPhish>();
                    break;
                case 1:
                    arr[index++].SetDefaults<DavyJonesAnchor>();
                    break;
                case 2:
                    arr[index++].SetDefaults<ArmFloaties>();
                    break;
                case 3:
                    arr[index++].SetDefaults<LiquidGun>();
                    break;
            }
            nextChestLoot++;

            switch (rand.Next(8))
            {
                case 0:
                    arr[index++].SetDefaults(ItemID.Trident);
                    break;

                case 1:
                    arr[index++].SetDefaults(ItemID.FloatingTube);
                    break;

                case 2:
                    arr[index++].SetDefaults(ItemID.Flipper);
                    break;

                case 3:
                    if (y > Main.worldSurface)
                    {
                        arr[index++].SetDefaults(ItemID.WaterWalkingBoots);
                    }
                    break;

                case 4:
                    arr[index++].SetDefaults(ItemID.BreathingReed);
                    break;

                case 5:
                    if (y > Main.worldSurface)
                    {
                        arr[index++].SetDefaults<FoolsGoldRing>();
                    }
                    break;
            }

            switch (rand.Next(3))
            {
                case 0:
                    arr[index++].SetDefaults(ItemID.CanOfWorms);
                    break;
                case 1:
                    arr[index].SetDefaults(ItemID.ApprenticeBait);
                    arr[index++].stack = rand.Next(5) + 2;
                    break;
                case 2:
                    if (y > Main.worldSurface)
                    {
                        arr[index].SetDefaults(ItemID.JourneymanBait);
                        arr[index++].stack = rand.Next(3) + 1;
                    }
                    break;
            }

            switch (rand.Next(8))
            {
                case 0:
                    arr[index].SetDefaults(ItemID.HealingPotion);
                    arr[index++].stack = rand.Next(5) + 1;
                    break;
                case 1:
                    arr[index].SetDefaults(ItemID.RecallPotion);
                    arr[index++].stack = rand.Next(5) + 1;
                    break;
                case 2:
                    arr[index].SetDefaults(ItemID.GillsPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                    break;
                case 3:
                    if (y > Main.worldSurface)
                    {
                        arr[index].SetDefaults(ItemID.MiningPotion);
                        arr[index++].stack = rand.Next(3) + 1;
                    }
                    break;
                case 4:
                    arr[index].SetDefaults(ItemID.BuilderPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                    break;
                case 5:
                    arr[index].SetDefaults(ItemID.HunterPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                    break;
                case 6:
                    if (y > Main.worldSurface)
                    {
                        arr[index].SetDefaults(ItemID.TrapsightPotion);
                        arr[index++].stack = rand.Next(3) + 1;
                    }
                    break;
                case 7:
                    arr[index].SetDefaults(ItemID.WaterWalkingPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                    break;
            }

            switch (rand.Next(3))
            {
                case 0:
                    arr[index].SetDefaults(ItemID.CoralTorch);
                    arr[index++].stack = rand.Next(15, 101);
                    break;
                case 1:
                    arr[index].SetDefaults(ItemID.Glowstick);
                    arr[index++].stack = rand.Next(15, 101);
                    break;
                case 2:
                    arr[index].SetDefaults(ItemID.BottledWater);
                    arr[index++].stack = rand.Next(8, 34);
                    break;
            }

            if (y > Main.worldSurface)
            {
                switch (WorldGen.genRand.Next(6))
                {
                    case 0:
                        arr[index++].SetDefaults(ItemID.DivingHelmet);
                        break;

                    case 1:
                        arr[index++].SetDefaults(ItemID.BeachBall);
                        break;

                    case 2:
                        arr[index++].SetDefaults(ItemID.JellyfishNecklace);
                        break;
                }
            }
            else
            {
                arr[index].SetDefaults(ItemID.PalmWood);
                arr[index++].stack = rand.Next(15, 251);
            }

            arr[index].SetDefaults(ItemID.SilverCoin);
            arr[index++].stack = rand.Next(20, 91);
        }

        public void FixSand(int leftX, int sizeX)
        {
            for (int i = leftX; i < leftX + sizeX; i++)
            {
                for (int j = 5; j < Main.maxTilesY - 5; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Sand || Main.tile[i, j].TileType == TileID.HardenedSand) && !Main.tile[i, j + 1].IsSolid())
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                        Main.tile[i, j - 1].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                    }
                    if (Main.tile[i, j].WallType == ModContent.WallType<SedimentaryRockWallWall>())
                    {
                        Main.tile[i, j].LiquidAmount = (byte)Math.Max(Main.tile[i, j].LiquidAmount, WorldGen.genRand.Next(byte.MaxValue) + 1);
                    }
                }
            }
        }

        public void Generate(GenerationProgress progress)
        {
            Reset();

            int reccomendedDir = 0;
            if (CalamityMod.Instance != null)
            {
                reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1;
            }
            else if (ThoriumMod.Instance != null)
            {
                reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
            }

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

            CreateSandAreaForCrevice(x + reccomendedDir * -20, y + 40);

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

            for (int k = 0; k < size * 128; k++)
            {
                int caveX = x + (int)WorldGen.genRand.NextFloat(-size * 1.33f, size * 1.33f);
                int caveY = y + WorldGen.genRand.Next(-10, (int)(size * 3f));
                int minScale = WorldGen.genRand.Next(4, 8);
                if (WorldGen.InWorld(x, y, 30) && GenerateCreviceCave(caveX, caveY, minScale, minScale + WorldGen.genRand.Next(4, 18), WorldGen.genRand.Next(80, 250)))
                {
                    k += 375;
                }
            }

            GrowWalls(location.X, location.Y);

            WorldGen.structures.AddProtectedStructure(new Rectangle(location.X - size, location.Y - size, size * 2, size * 2).Fluffize(5));
            AequusWorld.Structures.Add("CrabCrevice", location);
            int sizeX = size * 2;
            var leftX = LeftX(sizeX);
            PlaceChests(leftX, sizeX);
            FixSand(leftX, sizeX);
        }

        public void GrowPlants()
        {
            Reset();
            int sizeX = size * 2;
            var leftX = LeftX(sizeX);
            for (int i = 0; i < (Main.maxTilesX * Main.maxTilesY / 20); i++)
            {
                int randX = leftX + WorldGen.genRand.Next(sizeX);
                int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);

                if (Main.tile[randX, randY].HasTile && Main.tile[randX, randY].WallType == ModContent.WallType<SedimentaryRockWallWall>())
                {
                    try
                    {
                        for (int k = 0; k < 500; k++)
                        {
                            AequusWorld.RandomUpdateTile(randX, randY, checkNPCSpawns: false);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY / 8; i++)
            {
                int randX = leftX + WorldGen.genRand.Next(sizeX);
                int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);

                if (Main.tile[randX, randY].HasTile && Main.tile[randX, randY].TileType == ModContent.TileType<PearlsTile>())
                {
                    Main.tile[randX, randY].TileFrameX = 18 * 4;
                    break;
                }
            }
        }

        public void TransformPots()
        {
            Reset();
            int sizeX = size * 2;
            var leftX = LeftX(sizeX);
            for (int i = leftX; i < leftX + sizeX; i++)
            {
                for (int j = 0; j < Main.UnderworldLayer - 200; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Pots)
                    {
                        Main.tile[i, j].TileType = (ushort)ModContent.TileType<CrabCrevicePot>();
                        Main.tile[i, j].TileFrameY %= 72;
                    }
                }
            }
        }
    }
}