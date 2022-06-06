using Aequus.Tiles;
using Aequus.Tiles.Crab;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Generation
{
    public struct CrabCreviceGen
    {
        private const int Size = 160;
        private static int PirateChestCount = 0;

        public Point shipHole;
        public bool[,] holes;
        public Point[] chests;

        private static bool CanOverwriteTile(Tile tile)
        {
            return !Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType];
        }

        private bool IsValidCircleForGeneratingCave(int x, int y, int radius)
        {
            return IsValidCircleForGeneratingCave(new Circle(x, y, radius));
        }

        private static bool IsValidCircleForGeneratingCave(Circle circle)
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
                                if ((!Main.tile[x + k, y + l].HasTile || !Main.tile[x + k, y + l].Solid()) && CanOverwriteTile(Main.tile[x + k, y + l]))
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

        public void CreateSandAreaForCrevice(int x, int y)
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
                            if (!Main.tile[x2 + m, y2 + n].HasTile && !Main.tile[x2 + m, y2 + n].Solid() && Main.tile[x2 + m, y2 + n].LiquidAmount > 0)
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

        private bool HasUnOverwriteableTiles(Circle circle)
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
                            if (minWater > 100 && Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].Solid())
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

        private void AddVines()
        {
            for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    if (Main.tile[i, j].TileType == ModContent.TileType<SedimentaryRockTile>()/* || Main.tile[i, j].TileType == ModContent.TileType<PetrifiedWood>()*/)
                    {
                        if (Main.tile[i, j + 1].HasTile)
                        {
                            continue;
                        }
                        for (int k = 0; k < 30; k++)
                            TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
                    }
                }
            }
        }

        private void AddChests(int genX, int genY)
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
                if (Main.tile[chestX, chestY + 1].HasTile && Main.tile[chestX, chestY + 1].Solid()
                    && Main.tile[chestX + 1, chestY + 1].HasTile && Main.tile[chestX + 1, chestY + 1].Solid() && Main.tile[chestX, chestY].WallType == ModContent.WallType<SedimentaryRockWallWall>())
                {
                    bool validSpot = true;
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (Main.tile[chestX + k, chestY + l - 1].HasTile &&
                                !Main.tileCut[Main.tile[chestX + k, chestY + l - 1].TileType]
                                /*&& Main.tile[chestX + k, chestY + l - 1].TileType != ModContent.TileType<ExoticCoralNew>()*/)
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
                                Main.tile[chestX + k, chestY + l - 1].Active(value: false);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    int chest = WorldGen.PlaceChest(chestX, chestY, TileID.Containers, style: ChestTypes.Palm);
                    if (chest != -1 && Main.tile[chestX, chestY].TileType == TileID.Containers)
                    {
                        FillPalmChest(Chest.FindChest(chestX, chestY - 1), count);
                        count++;
                        i += 3500;
                    }
                }
            }
        }

        private void FillPalmChest(int chest, int count)
        {
            if (chest == -1 || Main.chest[chest] == null)
                return;
            var c = Main.chest[chest];
            int i = 0;
            //switch (count % 4)
            //{
            //    default:
            //        {
            //            c.item[i].SetDefaults(ModContent.ItemType<StarPhish>());
            //        }
            //        break;
            //    case 1:
            //        {
            //            c.item[i].SetDefaults(ModContent.ItemType<VineSword>());
            //        }
            //        break;
            //    case 2:
            //        {
            //            c.item[i].SetDefaults(ModContent.ItemType<CrabBarb>());
            //        }
            //        break;
            //    case 3:
            //        {
            //            c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
            //        }
            //        break;
            //}
            i++;
            //if (WorldGen.genRand.NextBool())
            //{
            //    switch (WorldGen.genRand.Next(3))
            //    {
            //        case 0:
            //            {
            //                c.item[i].SetDefaults(ItemID.DivingHelmet);
            //                i++;
            //            }
            //            break;

            //        case 1:
            //            {
            //                c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
            //                i++;
            //            }
            //            break;

            //        case 2:
            //            {
            //                c.item[i].SetDefaults(ModContent.ItemType<YuckyOrb>());
            //                i++;
            //            }
            //            break;
            //    }
            //}
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

        private void FillPirateChest(int chest, int count)
        {
            if (chest == -1 || Main.chest[chest] == null)
                return;
            var c = Main.chest[chest];
            int i = 0;
            //switch (count % 4)
            //{
            //    default:
            //        {
            //            c.item[i].SetDefaults(ModContent.ItemType<PearlAmulet>());
            //        }
            //        break;
            //}
            i++;
            //if (WorldGen.genRand.NextBool())
            //{
            //    switch (WorldGen.genRand.Next(3))
            //    {
            //        case 0:
            //            {
            //                c.item[i].SetDefaults(ItemID.DivingHelmet);
            //                i++;
            //            }
            //            break;

            //        case 1:
            //            {
            //                c.item[i].SetDefaults(ModContent.ItemType<CrabRod>());
            //                i++;
            //            }
            //            break;

            //        case 2:
            //            {
            //                c.item[i].SetDefaults(ModContent.ItemType<YuckyOrb>());
            //                i++;
            //            }
            //            break;
            //    }
            //}
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

        public void GenPirateShip(int x, int y)
        {
            GenPirateShip(x, y, WorldGen.genRand.NextBool() ? 1 : -1);
        }

        public void GenPirateShip(int x, int y, int direction)
        {
            GenPirateShip(x, y, direction, WorldGen.genRand.Next(35, 51));
        }

        public void GenPirateShip(int x, int y, int direction, int length)
        {
            //if (x < 40)
            //{
            //    x = 40;
            //}
            //else if (x > Main.maxTilesX - 40)
            //{
            //    x = Main.maxTilesX - 40;
            //}
            //if (direction == -1)
            //{
            //    if (x - length < 20)
            //    {
            //        x = length + 20;
            //    }
            //}
            //else
            //{
            //    if (x + length > Main.maxTilesX - 20)
            //    {
            //        x = Main.maxTilesX - 20 - length;
            //    }
            //}
            //for (int k = 0; k < length; k++)
            //{
            //    Main.tile[x + k * direction, y].Active(value: true);
            //    Main.tile[x + k * direction, y].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + k * direction, y].slope(slope: 0);
            //    Main.tile[x + k * direction, y].halfBrick(halfBrick: false);
            //    if (k >= 2 && k < length - 8)
            //    {
            //        if (k > 3 && k < 9)
            //        {
            //            Main.tile[x + k * direction, y].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //            Main.tile[x + k * direction, y].frameX = 0;
            //            Main.tile[x + k * direction, y].frameX = 0;
            //        }
            //        Main.tile[x + k * direction, y + 8].Active(value: true);
            //        Main.tile[x + k * direction, y + 8].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //        Main.tile[x + k * direction, y + 8].slope(slope: 0);
            //        Main.tile[x + k * direction, y + 8].halfBrick(halfBrick: false);
            //        for (int l = 0; l < 9; l++)
            //        {
            //            Main.tile[x + k * direction, y + l].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
            //        }
            //        for (int l = 1; l < 8; l++)
            //        {
            //            Main.tile[x + k * direction, y + l].Active(value: false);
            //        }
            //    }
            //    if (k >= 8 && k < length - 12)
            //    {
            //        if (k < 13)
            //        {
            //            Main.tile[x + k * direction, y + 8].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //            Main.tile[x + k * direction, y + 8].frameX = 0;
            //            Main.tile[x + k * direction, y + 8].frameX = 0;
            //        }
            //        Main.tile[x + k * direction, y + 13].Active(value: true);
            //        Main.tile[x + k * direction, y + 13].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //        Main.tile[x + k * direction, y + 13].slope(slope: 0);
            //        Main.tile[x + k * direction, y + 13].halfBrick(halfBrick: false);
            //        for (int l = 0; l < 5; l++)
            //        {
            //            Main.tile[x + k * direction, y + 9 + l].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
            //        }
            //        for (int l = 1; l < 5; l++)
            //        {
            //            Main.tile[x + k * direction, y + 8 + l].Active(value: false);
            //        }
            //    }
            //}
            //for (int k = 0; k < 10; k++)
            //{
            //    Main.tile[x + (k + length - 5) * direction, y - 1].Active(value: true);
            //    Main.tile[x + (k + length - 5) * direction, y - 1].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + (k + length - 5) * direction, y - 1].slope(slope: 0);
            //    Main.tile[x + (k + length - 5) * direction, y - 1].halfBrick(halfBrick: false);
            //}
            //Main.tile[x + (length - 6) * direction, y - 1].Active(value: true);
            //Main.tile[x + (length - 6) * direction, y - 1].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //Main.tile[x + (length - 6) * direction, y - 1].frameX = 0;
            //Main.tile[x + (length - 6) * direction, y - 1].frameY = 0;
            //Main.tile[x + (length - 6) * direction, y - 1].halfBrick(halfBrick: false);
            //if (direction == -1)
            //{
            //    Main.tile[x + (length - 6) * direction, y - 1].slope(slope: 1);
            //}
            //else
            //{
            //    Main.tile[x + (length - 6) * direction, y - 1].slope(slope: 2);
            //}

            //// platform at tip
            //Main.tile[x + (length + 5) * direction, y - 1].Active(value: true);
            //Main.tile[x + (length + 5) * direction, y - 1].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //Main.tile[x + (length + 5) * direction, y - 1].frameX = 0;
            //Main.tile[x + (length + 5) * direction, y - 1].frameY = 0;
            //Main.tile[x + (length + 5) * direction, y - 1].slope(slope: 0);
            //Main.tile[x + (length + 5) * direction, y - 1].halfBrick(halfBrick: false);
            //// back wall
            //for (int k = 0; k < 9; k++)
            //{
            //    if (k < 6)
            //    {
            //        Main.tile[x + 1 * direction, y + k].Active(value: true);
            //        Main.tile[x + 1 * direction, y + k].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //        Main.tile[x + 1 * direction, y + k].slope(slope: 0);
            //        Main.tile[x + 1 * direction, y + k].halfBrick(halfBrick: false);
            //        if (k < 3)
            //        {
            //            Main.tile[x, y + k].Active(value: true);
            //            Main.tile[x, y + k].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //            Main.tile[x, y + k].slope(slope: 0);
            //            Main.tile[x, y + k].halfBrick(halfBrick: false);
            //        }
            //    }
            //    Main.tile[x + 2 * direction, y + k].Active(value: true);
            //    Main.tile[x + 2 * direction, y + k].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + 2 * direction, y + k].slope(slope: 0);
            //    Main.tile[x + 2 * direction, y + k].halfBrick(halfBrick: false);
            //    Main.tile[x + 3 * direction, y + k].Active(value: true);
            //    Main.tile[x + 3 * direction, y + k].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + 3 * direction, y + k].slope(slope: 0);
            //    Main.tile[x + 3 * direction, y + k].halfBrick(halfBrick: false);
            //}

            //for (int k = 0; k < 5; k++)
            //{
            //    for (int l = 0; l <= k; l++)
            //    {
            //        Main.tile[x + (k + 3) * direction, y + l + 9].Active(value: true);
            //        Main.tile[x + (k + 3) * direction, y + l + 9].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //        Main.tile[x + (k + 3) * direction, y + l + 9].slope(slope: 0);
            //        Main.tile[x + (k + 3) * direction, y + l + 9].halfBrick(halfBrick: false);
            //    }
            //}

            //// wall front
            //for (int k = 0; k < 13; k++)
            //{
            //    Main.tile[x + (length - k - 1) * direction, y + k].Active(value: true);
            //    Main.tile[x + (length - k - 1) * direction, y + k].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + (length - k - 1) * direction, y + k].slope(slope: 0);
            //    Main.tile[x + (length - k - 1) * direction, y + k].halfBrick(halfBrick: false);

            //    Main.tile[x + (length - k - 1) * direction, y + k + 1].Active(value: true);
            //    Main.tile[x + (length - k - 1) * direction, y + k + 1].TileType = (ushort)ModContent.TileType<PetrifiedWood>();
            //    Main.tile[x + (length - k - 1) * direction, y + k + 1].slope(slope: 0);
            //    Main.tile[x + (length - k - 1) * direction, y + k + 1].halfBrick(halfBrick: false);
            //    for (int l = 0; l <= k; l++)
            //    {
            //        Main.tile[x + (length - k - 1) * direction, y + l + 1].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
            //    }
            //}

            //int sailHeight = WorldGen.genRand.Next(18, 30);
            //int sailX = x + length / 2 * direction;

            //for (int k = 1; k < sailHeight; k++)
            //{
            //    Main.tile[sailX, y - k].Active(value: true);
            //    Main.tile[sailX, y - k].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //    Main.tile[sailX, y - k].wall = (ushort)ModContent.WallType<PetrifiedWoodWall>();
            //    Main.tile[sailX, y - k].slope(slope: 0);
            //    Main.tile[sailX, y - k].halfBrick(halfBrick: false);
            //    if (Main.tile[sailX, y - k - 3].HasTile && Main.tile[sailX, y - k - 3].Solid())
            //    {
            //        sailHeight = k + 1;
            //        break;
            //    }
            //    if (k % 9 == 0)
            //    {
            //        int sailWidth = WorldGen.genRand.Next(12, 18);
            //        int down = 0;
            //        int pdown = 0;
            //        for (int m = -sailWidth; m < sailWidth; m++)
            //        {
            //            int a = m * direction;
            //            for (int i = -1; i < 2; i++)
            //            {
            //                for (int j = -1; j < 2; j++)
            //                {
            //                    if (Main.tile[sailX + a + i, y - k + down + j].HasTile &&
            //                        Main.tile[sailX + a + i, y - k + down + j].TileType != ModContent.TileType<PetrifiedWood>() &&
            //                        Main.tile[sailX + a + i, y - k + down + j].TileType != ModContent.TileType<PetrifiedWoodPlatform>() &&
            //                        Main.tile[sailX + a + i, y - k + down + j].Solid())
            //                    {
            //                        if (m < 0)
            //                            goto Next;
            //                        else
            //                            goto End;
            //                    }
            //                }
            //            }

            //            Main.tile[sailX + a, y - k + down].Active(value: true);
            //            Main.tile[sailX + a, y - k + down].TileType = (ushort)ModContent.TileType<PetrifiedWoodPlatform>();
            //            Main.tile[sailX + a, y - k + down].halfBrick(halfBrick: false);
            //            if (pdown == down && m > -sailWidth + 3 && m.Abs() > 2 && m < sailWidth - 3 && WorldGen.genRand.NextBool(8))
            //            {
            //                if (direction == -1)
            //                {
            //                    Main.tile[sailX + a, y - k + down].slope(slope: 2);
            //                    platformGenList.Add(new Vector3(sailX + a, y - k + down, 2));
            //                }
            //                else
            //                {
            //                    platformGenList.Add(new Vector3(sailX + a, y - k + down, 1));
            //                    Main.tile[sailX + a, y - k + down].slope(slope: 1);
            //                }
            //                down++;
            //            }
            //            else
            //            {
            //                pdown = down;
            //                Main.tile[sailX + a, y - k + down].slope(slope: 0);
            //            }

            //        Next:
            //            continue;
            //        End:
            //            break;
            //        }
            //    }
            //}

            //int tableX = WorldGen.genRand.Next(8, length - 10) * direction;
            //WorldGen.PlaceTile(x + tableX, y + 7, ModContent.TileType<AQTables>(), mute: true, forced: true, style: AQTables.PetrifiedWood);
            //if (WorldGen.genRand.NextBool())
            //{
            //    WorldGen.PlaceTile(x + tableX - 2, y + 7, ModContent.TileType<AQChairs>(), mute: true, forced: true, style: AQChairs.PetrifiedWood);
            //}
            //else
            //{
            //    WorldGen.PlaceTile(x + tableX + 2, y + 7, ModContent.TileType<AQChairs>(), mute: true, forced: true, style: AQChairs.PetrifiedWood);
            //}
            ////WorldGen.PlaceTile(x + tableX + 3, y + 6, TileID.Dirt, mute: true, forced: true, style: AQChairs.PetrifiedWood);

            //int chest = WorldGen.PlaceChest(x + 7 * direction, y + 7, TileID.Containers, style: ChestStyles.Gold);
            //if (chest != -1)
            //{
            //    FillPirateChest(chest, PirateChestCount);
            //    PirateChestCount++;
            //}

            //for (int k = 0; k < length + 10; k++)
            //{
            //    for (int l = -sailHeight; l < 20; l++)
            //    {
            //        WorldGen.SquareTileFrame(x + k * direction, y + l, resetFrame: true);
            //        WorldGen.SquareWallFrame(x + k * direction, y + l, resetFrame: true);
            //    }
            //}
        }

        public void GenerateCrabCrevice(out Point crabCreviceLocation)
        {
            PirateChestCount = 0;
            int crabCreviceLocationX = 0;
            int crabCreviceLocationY = 0;
            int reccomendedDir = 0;
            //if (AQMod.calamityMod.IsActive)
            //{
            //    reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1;
            //}
            //else if (AQMod.thoriumMod.IsActive)
            //{
            //    reccomendedDir = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
            //}
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    x = Main.maxTilesX - x;
                for (int y = 200; y < Main.worldSurface; y++)
                {
                    if (ProperCrabCreviceAnchor(x, y))
                    {
                        if (reccomendedDir == 0 || crabCreviceLocationX == 0)
                        {
                            crabCreviceLocationX = x;
                            crabCreviceLocationY = y;
                        }
                        else if (reccomendedDir == -1)
                        {
                            if (x * 2 < Main.maxTilesX)
                            {
                                crabCreviceLocationX = x;
                                crabCreviceLocationY = y;
                            }
                        }
                        else
                        {
                            if (x * 2 > Main.maxTilesX)
                            {
                                crabCreviceLocationX = x;
                                crabCreviceLocationY = y;
                            }
                        }
                        i += 1000;
                        break;
                    }
                }
            }

            crabCreviceLocation = new Point(crabCreviceLocationX, crabCreviceLocationY);

            CreateSandAreaForCrevice(crabCreviceLocationX, crabCreviceLocationY + 40);

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
                            if (Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].Solid())
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


            AddChests(crabCreviceLocationX, crabCreviceLocationY);
            AddVines();
        }

        public bool ProperCrabCreviceAnchor(int x, int y)
        {
            return !Framing.GetTileSafely(x, y).HasTile && Main.tile[x, y].LiquidAmount > 0 && Framing.GetTileSafely(x, y + 1).HasTile && Main.tileSand[Main.tile[x, y + 1].TileType];
        }
    }
}