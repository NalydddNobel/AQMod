using AQMod.Content.World.Generation;
using AQMod.Items.Materials;
using AQMod.Localization;
using AQMod.Tiles;
using AQMod.Tiles.Nature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AQMod.Common.WorldGeneration
{
    public class AQWorldGen : ModWorld
    {
        private static GenPass getPass(string name, WorldGenLegacyMethod method)
        {
            return new PassLegacy(name, method);
        }

        internal static void GenerateGlimmeringStatues(GenerationProgress progress)
        {
            if (progress != null)
                progress.Message = Language.GetTextValue(AQText.Key + "Common.WorldGen_GlimmeringStatues");
            int glimmerCount = 0;
            int glimmerMax = Main.maxTilesX / 1500;
            var rectLeft = new Rectangle(80, 40, 600, (int)Main.worldSurface - 100);
            var rectRight = new Rectangle(Main.maxTilesX - rectLeft.Width - rectLeft.X, rectLeft.Y, rectLeft.Width, rectLeft.Height);
            var rectSpace = new Rectangle(80, 40, Main.maxTilesX - 160, 180);
            for (int i = 0; i < 10000; i++)
            {
                int r = WorldGen.genRand.Next(3);
                Rectangle rect;
                switch (r)
                {
                    default:
                    rect = rectSpace;
                    break;

                    case 1:
                    rect = rectLeft;
                    break;

                    case 2:
                    rect = rectRight;
                    break;
                }
                int x = WorldGen.genRand.Next(rect.X, rect.X + rect.Width);
                int y = WorldGen.genRand.Next(rect.Y, rect.Y + rect.Height);
                if (GlimmeringStatue.TryGenGlimmeringStatue(x, y))
                {
                    glimmerCount++;
                    if (glimmerCount >= glimmerMax)
                        break;
                }
            }
        }

        internal static void GenerateGlobeTemples(GenerationProgress progress)
        {
            if (progress != null)
                progress.Message = Language.GetTextValue(AQText.Key + "Common.WorldGen_Globes");
            int templeCount = 0;
            int templeMax = Main.maxTilesX / 400;
            for (int i = 0; i < 10000; i++)
            {
                int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int y = WorldGen.genRand.Next((int)Main.worldSurface + 50, Main.maxTilesY - 300);
                if (Globe.GenGlobeTemple(x, y))
                {
                    templeCount++;
                    if (templeCount >= templeMax)
                        break;
                }
            }
        }

        internal static void GenerateGoreNests(GenerationProgress progress)
        {
            if (progress != null)
                progress.Message = Language.GetTextValue(AQText.Key + "Common.WorldGen_GoreNests");
            int goreNestCount = 0;
            for (int i = 0; i < 10000; i++)
            {
                int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int y = WorldGen.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 80);
                if (GoreNest.GrowGoreNest(x, y, true, true))
                {
                    goreNestCount++;
                    if (goreNestCount > 4)
                        break;
                }
            }
        }

        internal static void GenerateTikiChests(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue(AQText.Key + "Common.RandomStructures_TikiChest");
            int halfX = Main.maxTilesX / 2;
            bool flag = WorldGen.genRand.NextBool();
            int tikiChests = Main.maxTilesX / 900;
            for (int i = 0; i < 100000; i++)
            {
                int xOffset = WorldGen.genRand.Next(80, 200);
                int x = flag ? Main.maxTilesX - xOffset : xOffset;
                int y = WorldGen.genRand.Next(200, 500);
                if (TryPlaceTikiChest(x, y, out int chest))
                {
                    Chest c = Main.chest[chest];
                    int index = 0;
                    c.item[index].SetDefaults(ModContent.ItemType<CrabShell>());
                    c.item[index].stack = WorldGen.genRand.Next(18, 25) + 1;
                    index++;
                    tikiChests--;
                    if (tikiChests <= 0)
                        break;
                }
            }
            for (int i = 0; i < 100000; i++)
            {
                int _ = WorldGen.genRand.Next(80, 200);
                int x = flag ? Main.maxTilesX - _ : _;
                int y = WorldGen.genRand.Next(200, 500);
                if (TryPlaceFakeTikiChest(x, y))
                    break;
            }
        }

        public static bool TryPlaceTikiChest(int x, int y, out int chest)
        {
            if (check2x2_Liquid(x, y) && check1x3_Liquid(x - 1, y) && check1x3_Liquid(x + 2, y))
            {
                chest = WorldGen.PlaceChest(x, y, 21, false, 0);
                WorldGen.PlaceTile(x - 1, y, TileID.Lamps);
                WorldGen.PlaceTile(x + 2, y, TileID.Lamps);
                return true;
            }
            chest = -1;
            return false;
        }

        public static bool TryPlaceFakeTikiChest(int x, int y)
        {
            if (check2x2_Liquid(x, y) && check1x3_Liquid(x - 1, y) && check1x3_Liquid(x + 2, y))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (ActiveAndUncuttable(x, y + 4 + i) && NotSloped(x, y + 4 + i))
                    {
                        WorldGen.KillTile(x, y + 3 + i);
                        WorldGen.PlaceTile(x, y + 3 + i, TileID.Explosives);
                        AQUtils.PointAtoPointB(x, y, x, y + 3 + i, delegate (int x2, int y2)
                        {
                            WorldGen.PlaceWire(x2, y2);
                            return true;
                        });
                        AQUtils.PointAtoPointB(x - 1, y, x + 2, y, delegate (int x2, int y2)
                        {
                            WorldGen.PlaceWire(x2, y2);
                            return true;
                        });
                        WorldGen.Place2x2(x + 1, y, TileID.FakeContainers, 0);
                        WorldGen.PlaceTile(x - 1, y, TileID.Lamps);
                        WorldGen.PlaceTile(x + 2, y, TileID.Lamps);
                        return true;
                    }
                }
            }
            return false;
        }

        internal static void GenerateNobleMushrooms(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue(AQText.Key + "Common.RandomStructures_NobleMushrooms");
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 240);
                int style = WorldGen.genRand.Next(3);
                int size = WorldGen.genRand.Next(30, 75);
                if (TryPlaceNobleGroup(x, y, style, size))
                {
                    TryPlaceNobleGroup(x, y, style, size + 10);
                    TryPlaceNobleGroup(x, y, style, size + 20);
                    i += 450;
                }
            }
        }

        public static bool TryPlaceNobleGroup(int x, int y, int style, int size)
        {
            var halfSize = size / 2;
            var area = new Rectangle(x - halfSize, y - halfSize, size, size);
            int requiredMushrooms = size / 15;
            var validSpots = new List<Point>();
            var invalidX = new List<int>();
            var tileType = ModContent.TileType<NobleMushrooms>();
            for (int j = area.Y; j < area.Y + size; j++)
            {
                for (int i = area.X; i < area.X + size; i++)
                {
                    var invalidXIndex = invalidX.FindIndex((x2) => x2.Equals(i));
                    if (invalidXIndex != -1)
                    {
                        i++;
                        continue;
                    }
                    if (check2x2(i, j))
                    {
                        var type = Main.tile[i, j + 1].type;
                        bool canPlace = false;
                        for (int k = 0; k < NobleMushrooms.generationTiles.Length; k++)
                        {
                            if (NobleMushrooms.generationTiles[k] == type)
                            {
                                canPlace = true;
                                break;
                            }
                        }
                        type = Main.tile[i + 1, j + 1].type;
                        bool canPlace2 = false;
                        for (int k = 0; k < NobleMushrooms.generationTiles.Length; k++)
                        {
                            if (NobleMushrooms.generationTiles[k] == type)
                            {
                                canPlace2 = true;
                                break;
                            }
                        }
                        if (canPlace && canPlace2)
                        {
                            validSpots.Add(new Point(i, j));
                            invalidX.Add(i);
                            i++;
                        }
                    }
                }
            }
            if (validSpots.Count < requiredMushrooms)
                return false;
            for (int i = 0; i < requiredMushrooms; i++)
            {
                int index = WorldGen.genRand.Next(validSpots.Count);
                WorldGen.Place2x2Horizontal(validSpots[index].X, validSpots[index].Y, (ushort)tileType, style);
                if (Main.tile[validSpots[index].X, validSpots[index].Y].type == tileType)
                {
                    int verticalStyle = WorldGen.genRand.Next(3);
                    if (verticalStyle != 0)
                    {
                        int mushroomX = validSpots[index].X - Main.tile[validSpots[index].X, validSpots[index].Y].frameX % 36 / 18;
                        int mushroomY = validSpots[index].Y - Main.tile[validSpots[index].X, validSpots[index].Y].frameY % 38 / 18;

                        Main.tile[mushroomX, mushroomY].frameY += (short)(38 * verticalStyle);
                        Main.tile[mushroomX + 1, mushroomY].frameY += (short)(38 * verticalStyle);
                        Main.tile[mushroomX, mushroomY + 1].frameY += (short)(38 * verticalStyle);
                        Main.tile[mushroomX + 1, mushroomY + 1].frameY += (short)(38 * verticalStyle);
                    }
                }
                validSpots.RemoveAt(index);
            }
            return true;
        }

        internal static void GenerateCandelabraTraps(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue(AQText.Key + "Common.RandomStructures_CandelabraTraps");
            int halfX = Main.maxTilesX / 2;
            int candelabraTrapCount = Main.expertMode ? 7 : 3;
            int candelabras = 0;
            for (int i = 0; i < 100000; i++)
            {
                int x = WorldGen.genRand.Next(halfX - 350, halfX + 350);
                int y = WorldGen.genRand.Next(500, Main.maxTilesY - 300);
                if (TryPlaceCandelabraTrap(x, y))
                {
                    candelabras++;
                    if (candelabras >= candelabraTrapCount)
                        break;
                }
            }
        }

        public static bool TryPlaceCandelabraTrap(int x, int y)
        {
            if (check2x2(x, y))
            {
                var candelabra = new Rectangle(x, y - 1, 2, 2);
                var pressurePlate = new Point();
                var search = new Rectangle(x - 20, y - 20, 40, 30);
                for (int j = 0; j < 150; j++)
                {
                    int x2 = WorldGen.genRand.Next(search.X, search.X + search.Width + 1);
                    int y2 = WorldGen.genRand.Next(search.Y, search.Y + search.Height + 1);
                    if (!candelabra.Contains(x2, y2) && pressurePlateCheck(x2, y2))
                        pressurePlate = new Point(x2, y2);
                }
                if (pressurePlate != new Point()) // I always want to have a pressure plate that toggles the candelabra 
                {
                    var pressurePlate2 = new Point();
                    var invalidRect = new Rectangle(pressurePlate.X - 2, pressurePlate.Y - 2, 4, 4);
                    for (int j = 0; j < 100; j++)
                    {
                        int x2 = WorldGen.genRand.Next(search.X, search.X + search.Width + 1);
                        int y2 = WorldGen.genRand.Next(search.Y, search.Y + search.Height + 1);
                        if (!candelabra.Contains(x2, y2) && !invalidRect.Contains(x2, y2) && pressurePlateCheck(x2, y2))
                            pressurePlate2 = new Point(x2, y2);
                    }
                    WorldGen.KillTile(pressurePlate.X, pressurePlate.Y);
                    Main.tile[pressurePlate.X, pressurePlate.Y + 1].active(active: true);
                    WorldGen.PlaceTile(pressurePlate.X, pressurePlate.Y, TileID.PressurePlates, true, true);
                    AQUtils.PointAtoPointB(pressurePlate.X, pressurePlate.Y, x, y, delegate (int x3, int y3)
                    {
                        WorldGen.PlaceWire(x3, y3);
                        return true;
                    });
                    AQUtils.RectangleMethod(candelabra, delegate (int x3, int y3)
                    {
                        if (Main.tileCut[Main.tile[x3, y3].type])
                            WorldGen.KillTile(x3, y3);
                        return true;
                    });
                    WorldGen.PlaceTile(x + 1, y, TileID.PlatinumCandelabra, true, true);
                    if (pressurePlate2 != new Point())
                    {
                        var dynamite = new Point();
                        for (int j = 0; j < 5; j++)
                        {
                            if (Framing.GetTileSafely(x, y + 4 + j).active() &&
                                ActiveAndSolid(x, y + 5 + j) && NotSloped(x, y + 5 + j))
                            {
                                dynamite = new Point(x, y + 5 + j);
                                break;
                            }
                        }
                        if (dynamite != new Point())
                        {
                            WorldGen.PlaceTile(pressurePlate2.X, pressurePlate2.Y, TileID.PressurePlates, true, true);
                            AQUtils.PointAtoPointB(pressurePlate2.X, pressurePlate2.Y, dynamite.X, dynamite.Y, delegate (int x3, int y3)
                            {
                                WorldGen.PlaceWire2(x3, y3);
                                return true;
                            });
                            WorldGen.KillTile(dynamite.X, dynamite.Y);
                            WorldGen.PlaceTile(dynamite.X, dynamite.Y, TileID.Explosives, true, true);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void GenerateExoticBlotches(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue(AQText.Key + "Common.RandomStructures_ExoticCoralBlotches");
            for (int i = 0; i < 15000; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY - 240);
                int style = WorldGen.genRand.Next(3);
                int size = WorldGen.genRand.Next(50, 150);
                if (ExoticCoral.TryPlaceExoticBlotch(x, y, style, size))
                    i += 150;
            }
        }

        public static bool PlaceRobsterQuestTile(int style = 6)
        {
            int x = WorldGen.genRand.Next(90, 200);
            if (WorldGen.genRand.NextBool())
                x = Main.maxTilesX - x;
            for (int j = 200; j < Main.worldSurface; j++)
            {
                if (!Framing.GetTileSafely(x, j).active() && Main.tile[x, j].liquid > 0 && Framing.GetTileSafely(x, j + 1).active())
                {
                    WorldGen.PlaceTile(x, j, ModContent.TileType<ExoticCoral>(), true, false, -1, style);
                    if (Framing.GetTileSafely(x, j).active() && Main.tile[x, j].type == ModContent.TileType<ExoticCoral>())
                        return true;
                }
            }
            return false;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int i;
            i = tasks.FindIndex((t) => t.Name.Equals("Beaches"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, getPass("AQMod: Ocean Ravines", CrabCrevice.GenerateLegacyRavines));
            }
            i = tasks.FindIndex((t) => t.Name.Equals("Hellforge"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, getPass("AQMod: Gore Nests", GenerateGoreNests));
                tasks.Insert(i, getPass("AQMod: Globe Temples", GenerateGlobeTemples));
                tasks.Insert(i, getPass("AQMod: Glimmering Statues", GenerateGlimmeringStatues));
            }
            i = tasks.FindIndex((t) => t.Name.Equals("Settle Liquids"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, getPass("AQMod: Fix Baby Pools", WaterCleaner.PassFix1TileHighWater));
            }
            i = tasks.FindIndex((t) => t.Name.Equals("Micro Biomes"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, getPass("AQMod: Noble Mushrooms", GenerateNobleMushrooms));
                tasks.Insert(i, getPass("AQMod: Tiki Chests", GenerateTikiChests));
                tasks.Insert(i, getPass("AQMod: Candelabra Traps", GenerateCandelabraTraps));
                tasks.Insert(i, getPass("AQMod: Exotic Coral", GenerateExoticBlotches));
                tasks.Insert(i, getPass("AQMod: Buried Chests", ChestLoot.Buried.GenerateDirtChests));
            }
        }

        internal static void KillRectangle(Rectangle rect)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    WorldGen.KillTile(i, j);
                }
            }
        }

        internal static void KillCuttable(Rectangle rect)
        {
            for (int i = rect.X; i <= rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j <= rect.Y + rect.Height; j++)
                {
                    if (Framing.GetTileSafely(i, j).active() && Main.tileCut[Main.tile[i, j].type])
                        WorldGen.KillTile(i, j);
                }
            }
        }

        internal static bool pressurePlateCheck(int x, int y)
        {
            return !ActiveAndUncuttable(x, y) &&
            !ActiveAndUncuttable(x + 1, y) &&
            !ActiveAndUncuttable(x, y - 1) &&
            !ActiveAndUncuttable(x + 1, y - 1) &&
            !ActiveAndUncuttable(x, y - 2) &&
            !ActiveAndUncuttable(x + 1, y - 2) &&
            ActiveAndSolid(x, y + 1) && NotSloped(x, y + 1);
        }

        internal static bool NotSloped(int x, int y)
        {
            return Main.tile[x, y].slope() == 0 && !Main.tile[x, y].halfBrick();
        }

        internal static bool ActiveAndSolid(int x, int y)
        {
            return Framing.GetTileSafely(x, y).active() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileCut[Main.tile[x, y].type];
        }

        internal static bool ActiveAndFullySolid(int x, int y)
        {
            return ActiveAndFullySolid(Framing.GetTileSafely(x, y));
        }

        internal static bool ActiveAndFullySolid(Tile tile)
        {
            return tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type] && !Main.tileCut[tile.type];
        }

        internal static bool ActiveAndUncuttable(int x, int y)
        {
            return Framing.GetTileSafely(x, y).active() && !Main.tileCut[Main.tile[x, y].type];
        }

        internal static bool check1x3(int x, int y)
        {
            return !ActiveAndUncuttable(x, y) &&
                   !ActiveAndUncuttable(x, y - 1) &&
                   !ActiveAndUncuttable(x, y - 2) &&
                   ActiveAndUncuttable(x, y + 1) && NotSloped(x, y + 1);
        }

        internal static bool check1x3_Liquid(int x, int y)
        {
            return !ActiveAndUncuttable(x, y) && Main.tile[x, y].liquid == 0 &&
                   !ActiveAndUncuttable(x, y - 1) && Main.tile[x, y - 1].liquid == 0 &&
                   !ActiveAndUncuttable(x, y - 2) && Main.tile[x, y - 2].liquid == 0 &&
                   ActiveAndUncuttable(x, y + 1) && NotSloped(x, y + 1);
        }

        internal static bool check2x2(int x, int y)
        {
            return !ActiveAndUncuttable(x, y) &&
                   !ActiveAndUncuttable(x + 1, y) &&
                   !ActiveAndUncuttable(x, y - 1) &&
                   !ActiveAndUncuttable(x + 1, y - 1) &&
                   ActiveAndSolid(x, y + 1) && NotSloped(x, y + 1) &&
                   ActiveAndSolid(x + 1, y + 1) && NotSloped(x + 1, y + 1);
        }

        internal static bool check2x2_Liquid(int x, int y)
        {
            return !ActiveAndUncuttable(x, y) && Main.tile[x, y].liquid == 0 &&
                   !ActiveAndUncuttable(x + 1, y) && Main.tile[x + 1, y].liquid == 0 &&
                   !ActiveAndUncuttable(x, y - 1) && Main.tile[x, y - 1].liquid == 0 &&
                   !ActiveAndUncuttable(x + 1, y - 1) && Main.tile[x + 1, y - 1].liquid == 0 &&
                   ActiveAndSolid(x, y + 1) && NotSloped(x, y + 1) &&
                   ActiveAndSolid(x + 1, y + 1) && NotSloped(x + 1, y + 1);
        }

        private static int CountNearBlocksTypes(int i, int j, int radius, int cap = 0, params int[] tiletypes)
        {
            if (tiletypes.Length == 0)
            {
                return 0;
            }
            int value5 = i - radius;
            int value2 = i + radius;
            int value3 = j - radius;
            int value4 = j + radius;
            int num3 = Utils.Clamp(value5, 0, Main.maxTilesX - 1);
            value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
            value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
            value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
            int num2 = 0;
            for (int k = num3; k < value2; k++)
            {
                for (int l = value3; l < value4; l++)
                {
                    if (!Main.tile[k, l].active())
                    {
                        continue;
                    }
                    for (int m = 0; m < tiletypes.Length; m++)
                    {
                        if (tiletypes[m] == Main.tile[k, l].type)
                        {
                            num2++;
                            if (cap <= 0 || num2 < cap)
                            {
                                break;
                            }
                            return num2;
                        }
                    }
                }
            }
            return num2;
        }

        public static void RandomUpdateTile_Spreading(int x, int y, int maxValue = -1)
        {
            if (maxValue == -1)
                maxValue = (int)MathHelper.Lerp(151, (float)151 * 2.8f, MathHelper.Clamp((float)Main.maxTilesX / 4200f - 1f, 0f, 1f));
            int num = 20;
            if (Main.rand.Next(100) == 0 && Main.rand.Next(maxValue) == 0)
            {
                WorldGen.PlantAlch();
            }
            int xMinus1 = x - 1;
            int xPlus2 = x + 2;
            int yMinus1 = y - 1;
            int yPlus2 = y + 2;
            if (xMinus1 < 10)
            {
                xMinus1 = 10;
            }
            if (xPlus2 > Main.maxTilesX - 10)
            {
                xPlus2 = Main.maxTilesX - 10;
            }
            if (yMinus1 < 10)
            {
                yMinus1 = 10;
            }
            if (yPlus2 > Main.maxTilesY - 10)
            {
                yPlus2 = Main.maxTilesY - 10;
            }
            if (Main.tile[x, y] == null)
            {
                return;
            }
            if (Main.tileAlch[Main.tile[x, y].type])
            {
                WorldGen.GrowAlch(x, y);
            }
            if (Main.tile[x, y].liquid > 32)
            {
                if (Main.tile[x, y].active() && (Main.tile[x, y].type == 3 || Main.tile[x, y].type == 20 || Main.tile[x, y].type == 24 || Main.tile[x, y].type == 27 || Main.tile[x, y].type == 73 || Main.tile[x, y].type == 201))
                {
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(17, -1, -1, null, 0, x, y);
                    }
                }
            }
            else if (Main.tile[x, y].nactive())
            {
                WorldGen.hardUpdateWorld(x, y);
                if (Main.rand.Next(3000) == 0)
                {
                    WorldGen.plantDye(x, y);
                }
                if (Main.rand.Next(9001) == 0)
                {
                    WorldGen.plantDye(x, y, exoticPlant: true);
                }
                if (Main.tile[x, y].type == 80)
                {
                    if (WorldGen.genRand.Next(15) == 0)
                    {
                        WorldGen.GrowCactus(x, y);
                    }
                }
                else if (TileID.Sets.Conversion.Sand[Main.tile[x, y].type])
                {
                    if (!Main.tile[x, yMinus1].active())
                    {
                        if (x < 250 || x > Main.maxTilesX - 250)
                        {
                            if (WorldGen.genRand.Next(500) == 0)
                            {
                                int num22 = 7;
                                int num32 = 6;
                                int num43 = 0;
                                for (int k = x - num22; k <= x + num22; k++)
                                {
                                    for (int l = yMinus1 - num22; l <= yMinus1 + num22; l++)
                                    {
                                        if (Main.tile[k, l].active() && Main.tile[k, l].type == 81)
                                        {
                                            num43++;
                                        }
                                    }
                                }
                                if (num43 < num32 && Main.tile[x, yMinus1].liquid == byte.MaxValue && Main.tile[x, yMinus1 - 1].liquid == byte.MaxValue && Main.tile[x, yMinus1 - 2].liquid == byte.MaxValue && Main.tile[x, yMinus1 - 3].liquid == byte.MaxValue && Main.tile[x, yMinus1 - 4].liquid == byte.MaxValue)
                                {
                                    WorldGen.PlaceTile(x, yMinus1, 81, mute: true);
                                    if (Main.netMode == 2 && Main.tile[x, yMinus1].active())
                                    {
                                        NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                                    }
                                }
                            }
                        }
                        else if (x > 400 && x < Main.maxTilesX - 400 && WorldGen.genRand.Next(300) == 0)
                        {
                            WorldGen.GrowCactus(x, y);
                        }
                    }
                }
                else if (Main.tile[x, y].type == 116 || Main.tile[x, y].type == 112 || Main.tile[x, y].type == 234)
                {
                    if (!Main.tile[x, yMinus1].active() && x > 400 && x < Main.maxTilesX - 400 && WorldGen.genRand.Next(300) == 0)
                    {
                        WorldGen.GrowCactus(x, y);
                    }
                }
                else if (Main.tile[x, y].type == 147 || Main.tile[x, y].type == 161 || Main.tile[x, y].type == 163 || Main.tile[x, y].type == 164 || Main.tile[x, y].type == 200)
                {
                    if (Main.rand.Next(10) == 0 && !Main.tile[x, y + 1].active() && !Main.tile[x, y + 2].active())
                    {
                        int num139 = x - 3;
                        int num49 = x + 4;
                        int num50 = 0;
                        for (int m = num139; m < num49; m++)
                        {
                            if (Main.tile[m, y].type == 165 && Main.tile[m, y].active())
                            {
                                num50++;
                            }
                            if (Main.tile[m, y + 1].type == 165 && Main.tile[m, y + 1].active())
                            {
                                num50++;
                            }
                            if (Main.tile[m, y + 2].type == 165 && Main.tile[m, y + 2].active())
                            {
                                num50++;
                            }
                            if (Main.tile[m, y + 3].type == 165 && Main.tile[m, y + 3].active())
                            {
                                num50++;
                            }
                        }
                        if (num50 < 2)
                        {
                            WorldGen.PlaceTight(x, y + 1, 165);
                            WorldGen.SquareTileFrame(x, y + 1);
                            if (Main.netMode == 2 && Main.tile[x, y + 1].active())
                            {
                                NetMessage.SendTileSquare(-1, x, y + 1, 3);
                            }
                        }
                    }
                }
                else if (Main.tile[x, y].type == 254)
                {
                    if (Main.rand.Next((Main.tile[x, y].frameX + 10) / 10) == 0)
                    {
                        WorldGen.GrowPumpkin(x, y, 254);
                    }
                }
                else if (Main.tile[x, y].type == 78 || Main.tile[x, y].type == 380)
                {
                    if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(2) == 0)
                    {
                        WorldGen.PlaceTile(x, yMinus1, 3, mute: true);
                        if (Main.netMode == 2 && Main.tile[x, yMinus1].active())
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                        }
                    }
                }
                else if (Main.tile[x, y].type == 2 || Main.tile[x, y].type == 23 || Main.tile[x, y].type == 32 || Main.tile[x, y].type == 109 || Main.tile[x, y].type == 199 || Main.tile[x, y].type == 352)
                {
                    int num51 = Main.tile[x, y].type;
                    if (Main.halloween && WorldGen.genRand.Next(75) == 0 && (num51 == 2 || num51 == 109))
                    {
                        int num52 = 100;
                        int num54 = 0;
                        for (int n = x - num52; n < x + num52; n += 2)
                        {
                            for (int num55 = y - num52; num55 < y + num52; num55 += 2)
                            {
                                if (n > 1 && n < Main.maxTilesX - 2 && num55 > 1 && num55 < Main.maxTilesY - 2 && Main.tile[n, num55].active() && Main.tile[n, num55].type == 254)
                                {
                                    num54++;
                                }
                            }
                        }
                        if (num54 < 6)
                        {
                            WorldGen.PlacePumpkin(x, yMinus1);
                            if (Main.netMode == 2 && Main.tile[x, yMinus1].type == 254)
                            {
                                NetMessage.SendTileSquare(-1, x, yMinus1, 4);
                            }
                        }
                    }
                    if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(12) == 0 && num51 == 2 && WorldGen.PlaceTile(x, yMinus1, 3, mute: true))
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                        }
                    }
                    if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(10) == 0 && num51 == 23 && WorldGen.PlaceTile(x, yMinus1, 24, mute: true))
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                        }
                    }
                    if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(10) == 0 && num51 == 109 && WorldGen.PlaceTile(x, yMinus1, 110, mute: true))
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                        }
                    }
                    if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(10) == 0 && num51 == 199 && WorldGen.PlaceTile(x, yMinus1, 201, mute: true))
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                        }
                    }
                    bool flag12 = false;
                    for (int num56 = xMinus1; num56 < xPlus2; num56++)
                    {
                        for (int num57 = yMinus1; num57 < yPlus2; num57++)
                        {
                            if ((x == num56 && y == num57) || !Main.tile[num56, num57].active())
                            {
                                continue;
                            }
                            if (num51 == 32)
                            {
                                num51 = 23;
                            }
                            if (num51 == 352)
                            {
                                num51 = 199;
                            }
                            if (Main.tile[num56, num57].type == 0 || (num51 == 23 && Main.tile[num56, num57].type == 2) || (num51 == 199 && Main.tile[num56, num57].type == 2) || (num51 == 23 && Main.tile[num56, num57].type == 109))
                            {
                                WorldGen.SpreadGrass(num56, num57, 0, num51, repeat: false, Main.tile[x, y].color());
                                if (num51 == 23)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 2, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (num51 == 23)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 109, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (num51 == 199)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 2, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (num51 == 199)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 109, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (Main.tile[num56, num57].type == num51)
                                {
                                    WorldGen.SquareTileFrame(num56, num57);
                                    flag12 = true;
                                }
                            }
                            if (Main.tile[num56, num57].type == 0 || (num51 == 109 && Main.tile[num56, num57].type == 2) || (num51 == 109 && Main.tile[num56, num57].type == 23) || (num51 == 109 && Main.tile[num56, num57].type == 199))
                            {
                                WorldGen.SpreadGrass(num56, num57, 0, num51, repeat: false, Main.tile[x, y].color());
                                if (num51 == 109)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 2, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (num51 == 109)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 23, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (num51 == 109)
                                {
                                    WorldGen.SpreadGrass(num56, num57, 199, num51, repeat: false, Main.tile[x, y].color());
                                }
                                if (Main.tile[num56, num57].type == num51)
                                {
                                    WorldGen.SquareTileFrame(num56, num57);
                                    flag12 = true;
                                }
                            }
                        }
                    }
                    if (Main.netMode == 2 && flag12)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 3);
                    }
                }
                else if (Main.tile[x, y].type == 20 && WorldGen.genRand.Next(20) == 0)
                {
                    bool flag23 = WorldGen.PlayerLOS(x, y);
                    if (((Main.tile[x, y].frameX < 324 || Main.tile[x, y].frameX >= 540) ? WorldGen.GrowTree(x, y) : WorldGen.GrowPalmTree(x, y)) && flag23)
                    {
                        WorldGen.TreeGrowFXCheck(x, y);
                    }
                }
                if (Main.tile[x, y].type == 3 && WorldGen.genRand.Next(20) == 0 && Main.tile[x, y].frameX != 144)
                {
                    if ((Main.tile[x, y].frameX < 144 && Main.rand.Next(10) == 0) || ((Main.tile[x, y + 1].type == 78 || Main.tile[x, y + 1].type == 380) && Main.rand.Next(2) == 0))
                    {
                        Main.tile[x, y].frameX = (short)(198 + WorldGen.genRand.Next(10) * 18);
                    }
                    Main.tile[x, y].type = 73;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 3);
                    }
                }
                if (Main.tile[x, y].type == 110 && WorldGen.genRand.Next(20) == 0 && Main.tile[x, y].frameX < 144)
                {
                    Main.tile[x, y].type = 113;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 3);
                    }
                }
                if (Main.tile[x, y].type == 32 && WorldGen.genRand.Next(3) == 0)
                {
                    int num58 = x;
                    int num59 = y;
                    int num60 = 0;
                    if (Main.tile[num58 + 1, num59].active() && Main.tile[num58 + 1, num59].type == 32)
                    {
                        num60++;
                    }
                    if (Main.tile[num58 - 1, num59].active() && Main.tile[num58 - 1, num59].type == 32)
                    {
                        num60++;
                    }
                    if (Main.tile[num58, num59 + 1].active() && Main.tile[num58, num59 + 1].type == 32)
                    {
                        num60++;
                    }
                    if (Main.tile[num58, num59 - 1].active() && Main.tile[num58, num59 - 1].type == 32)
                    {
                        num60++;
                    }
                    if (num60 < 3 || Main.tile[x, y].type == 23)
                    {
                        switch (WorldGen.genRand.Next(4))
                        {
                            case 0:
                            num59--;
                            break;
                            case 1:
                            num59++;
                            break;
                            case 2:
                            num58--;
                            break;
                            case 3:
                            num58++;
                            break;
                        }
                        if (!Main.tile[num58, num59].active())
                        {
                            num60 = 0;
                            if (Main.tile[num58 + 1, num59].active() && Main.tile[num58 + 1, num59].type == 32)
                            {
                                num60++;
                            }
                            if (Main.tile[num58 - 1, num59].active() && Main.tile[num58 - 1, num59].type == 32)
                            {
                                num60++;
                            }
                            if (Main.tile[num58, num59 + 1].active() && Main.tile[num58, num59 + 1].type == 32)
                            {
                                num60++;
                            }
                            if (Main.tile[num58, num59 - 1].active() && Main.tile[num58, num59 - 1].type == 32)
                            {
                                num60++;
                            }
                            if (num60 < 2)
                            {
                                int num61 = 7;
                                int num140 = num58 - num61;
                                int num62 = num58 + num61;
                                int num64 = num59 - num61;
                                int num65 = num59 + num61;
                                bool flag24 = false;
                                for (int num66 = num140; num66 < num62; num66++)
                                {
                                    for (int num67 = num64; num67 < num65; num67++)
                                    {
                                        if (Math.Abs(num66 - num58) * 2 + Math.Abs(num67 - num59) < 9 && Main.tile[num66, num67].active() && Main.tile[num66, num67].type == 23 && Main.tile[num66, num67 - 1].active() && Main.tile[num66, num67 - 1].type == 32 && Main.tile[num66, num67 - 1].liquid == 0)
                                        {
                                            flag24 = true;
                                            break;
                                        }
                                    }
                                }
                                if (flag24)
                                {
                                    Main.tile[num58, num59].type = 32;
                                    Main.tile[num58, num59].active(active: true);
                                    WorldGen.SquareTileFrame(num58, num59);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, num58, num59, 3);
                                    }
                                }
                            }
                        }
                    }
                }
                if (Main.tile[x, y].type == 352 && WorldGen.genRand.Next(3) == 0)
                {
                    WorldGen.GrowSpike(x, y, 352, 199);
                }
            }
            if (Main.tile[x, y].wall == 81 || Main.tile[x, y].wall == 83 || (Main.tile[x, y].type == 199 && Main.tile[x, y].active()))
            {
                int num68 = x + WorldGen.genRand.Next(-2, 3);
                int num69 = y + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num68, num69].wall >= 63 && Main.tile[num68, num69].wall <= 68)
                {
                    bool flag25 = false;
                    for (int num70 = x - num; num70 < x + num; num70++)
                    {
                        for (int num71 = y - num; num71 < y + num; num71++)
                        {
                            if (Main.tile[x, y].active())
                            {
                                int type = Main.tile[x, y].type;
                                if (type == 199 || type == 200 || type == 201 || type == 203 || type == 205 || type == 234 || type == 352)
                                {
                                    flag25 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag25)
                    {
                        Main.tile[num68, num69].wall = 81;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num68, num69, 3);
                        }
                    }
                }
            }
            if (Main.tile[x, y].wall == 69 || Main.tile[x, y].wall == 3 || (Main.tile[x, y].type == 23 && Main.tile[x, y].active()))
            {
                int num72 = x + WorldGen.genRand.Next(-2, 3);
                int num73 = y + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num72, num73].wall >= 63 && Main.tile[num72, num73].wall <= 68)
                {
                    bool flag26 = false;
                    for (int num75 = x - num; num75 < x + num; num75++)
                    {
                        for (int num76 = y - num; num76 < y + num; num76++)
                        {
                            if (Main.tile[num75, num76].active())
                            {
                                int type5 = Main.tile[num75, num76].type;
                                if (type5 == 22 || type5 == 23 || type5 == 24 || type5 == 25 || type5 == 32 || type5 == 112 || type5 == 163)
                                {
                                    flag26 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag26)
                    {
                        Main.tile[num72, num73].wall = 69;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num72, num73, 3);
                        }
                    }
                }
            }
            if (Main.tile[x, y].wall == 70 || (Main.tile[x, y].type == 109 && Main.tile[x, y].active()))
            {
                int num77 = x + WorldGen.genRand.Next(-2, 3);
                int num78 = y + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num77, num78].wall == 63 || Main.tile[num77, num78].wall == 65 || Main.tile[num77, num78].wall == 66 || Main.tile[num77, num78].wall == 68)
                {
                    bool flag27 = false;
                    for (int num79 = x - num; num79 < x + num; num79++)
                    {
                        for (int num80 = y - num; num80 < y + num; num80++)
                        {
                            if (Main.tile[num79, num80].active())
                            {
                                int type6 = Main.tile[num79, num80].type;
                                if (type6 == 109 || type6 == 110 || type6 == 113 || type6 == 115 || type6 == 116 || type6 == 117 || type6 == 164)
                                {
                                    flag27 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag27)
                    {
                        Main.tile[num77, num78].wall = 70;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num77, num78, 3);
                        }
                    }
                }
            }
            WorldGen.SpreadDesertWalls(num, x, y);
            if (!Main.tile[x, y].active())
            {
                return;
            }
            if ((Main.tile[x, y].type == 2 || Main.tile[x, y].type == 52 || (Main.tile[x, y].type == 192 && WorldGen.genRand.Next(10) == 0)) && WorldGen.genRand.Next(40) == 0 && !Main.tile[x, y + 1].active() && !Main.tile[x, y + 1].lava())
            {
                bool flag28 = false;
                for (int num81 = y; num81 > y - 10; num81--)
                {
                    if (Main.tile[x, num81].bottomSlope())
                    {
                        flag28 = false;
                        break;
                    }
                    if (Main.tile[x, num81].active() && Main.tile[x, num81].type == 2 && !Main.tile[x, num81].bottomSlope())
                    {
                        flag28 = true;
                        break;
                    }
                }
                if (flag28)
                {
                    int num82 = x;
                    int num83 = y + 1;
                    Main.tile[num82, num83].type = 52;
                    Main.tile[num82, num83].active(active: true);
                    Main.tile[num82, num83].color(Main.tile[x, y].color());
                    WorldGen.SquareTileFrame(num82, num83);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, num82, num83, 3);
                    }
                }
            }
            if (Main.tile[x, y].type == 70)
            {
                int type7 = Main.tile[x, y].type;
                if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(10) == 0)
                {
                    WorldGen.PlaceTile(x, yMinus1, 71, mute: true);
                    if (Main.tile[x, yMinus1].active())
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                    }
                    if (Main.netMode == 2 && Main.tile[x, yMinus1].active())
                    {
                        NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                    }
                }
                if (WorldGen.genRand.Next(100) == 0)
                {
                    bool flag2 = WorldGen.PlayerLOS(x, y);
                    if (WorldGen.GrowTree(x, y) && flag2)
                    {
                        WorldGen.TreeGrowFXCheck(x, y - 1);
                    }
                }
                bool flag3 = false;
                for (int num84 = xMinus1; num84 < xPlus2; num84++)
                {
                    for (int num86 = yMinus1; num86 < yPlus2; num86++)
                    {
                        if ((x != num84 || y != num86) && Main.tile[num84, num86].active() && Main.tile[num84, num86].type == 59)
                        {
                            WorldGen.SpreadGrass(num84, num86, 59, type7, repeat: false, Main.tile[x, y].color());
                            if (Main.tile[num84, num86].type == type7)
                            {
                                WorldGen.SquareTileFrame(num84, num86);
                                flag3 = true;
                            }
                        }
                    }
                }
                if (Main.netMode == 2 && flag3)
                {
                    NetMessage.SendTileSquare(-1, x, y, 3);
                }
            }
            if (Main.tile[x, y].type == 60)
            {
                int type8 = Main.tile[x, y].type;
                if (!Main.tile[x, yMinus1].active() && WorldGen.genRand.Next(7) == 0)
                {
                    WorldGen.PlaceTile(x, yMinus1, 61, mute: true);
                    if (Main.tile[x, yMinus1].active())
                    {
                        Main.tile[x, yMinus1].color(Main.tile[x, y].color());
                    }
                    if (Main.netMode == 2 && Main.tile[x, yMinus1].active())
                    {
                        NetMessage.SendTileSquare(-1, x, yMinus1, 1);
                    }
                }
                else if (WorldGen.genRand.Next(500) == 0 && (!Main.tile[x, yMinus1].active() || Main.tile[x, yMinus1].type == 61 || Main.tile[x, yMinus1].type == 74 || Main.tile[x, yMinus1].type == 69))
                {
                    if (WorldGen.GrowTree(x, y) && WorldGen.PlayerLOS(x, y))
                    {
                        WorldGen.TreeGrowFXCheck(x, y - 1);
                    }
                }
                else if (WorldGen.genRand.Next(25) == 0 && Main.tile[x, yMinus1].liquid == 0)
                {
                    WorldGen.PlaceJunglePlant(x, yMinus1, 233, WorldGen.genRand.Next(8), 0);
                    if (Main.tile[x, yMinus1].type == 233)
                    {
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, x, yMinus1, 4);
                        }
                        else
                        {
                            WorldGen.PlaceJunglePlant(x, yMinus1, 233, WorldGen.genRand.Next(12), 1);
                            if (Main.tile[x, yMinus1].type == 233 && Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, x, yMinus1, 3);
                            }
                        }
                    }
                }
                bool flag4 = false;
                for (int num87 = xMinus1; num87 < xPlus2; num87++)
                {
                    for (int num88 = yMinus1; num88 < yPlus2; num88++)
                    {
                        if ((x != num87 || y != num88) && Main.tile[num87, num88].active() && Main.tile[num87, num88].type == 59)
                        {
                            WorldGen.SpreadGrass(num87, num88, 59, type8, repeat: false, Main.tile[x, y].color());
                            if (Main.tile[num87, num88].type == type8)
                            {
                                WorldGen.SquareTileFrame(num87, num88);
                                flag4 = true;
                            }
                        }
                    }
                }
                if (Main.netMode == 2 && flag4)
                {
                    NetMessage.SendTileSquare(-1, x, y, 3);
                }
            }
            if (Main.tile[x, y].type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile[x, y].frameX < 144)
            {
                if (Main.rand.Next(4) == 0)
                {
                    Main.tile[x, y].frameX = (short)(162 + WorldGen.genRand.Next(8) * 18);
                }
                Main.tile[x, y].type = 74;
                if (Main.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, x, y, 3);
                }
            }
            if ((Main.tile[x, y].type == 60 || Main.tile[x, y].type == 62) && WorldGen.genRand.Next(15) == 0 && !Main.tile[x, y + 1].active() && !Main.tile[x, y + 1].lava())
            {
                bool flag5 = false;
                for (int num89 = y; num89 > y - 10; num89--)
                {
                    if (Main.tile[x, num89].bottomSlope())
                    {
                        flag5 = false;
                        break;
                    }
                    if (Main.tile[x, num89].active() && Main.tile[x, num89].type == 60 && !Main.tile[x, num89].bottomSlope())
                    {
                        flag5 = true;
                        break;
                    }
                }
                if (flag5)
                {
                    int num90 = x;
                    int num91 = y + 1;
                    Main.tile[num90, num91].type = 62;
                    Main.tile[num90, num91].active(active: true);
                    WorldGen.SquareTileFrame(num90, num91);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, num90, num91, 3);
                    }
                }
            }
            if ((Main.tile[x, y].type == 109 || Main.tile[x, y].type == 115) && WorldGen.genRand.Next(15) == 0 && !Main.tile[x, y + 1].active() && !Main.tile[x, y + 1].lava())
            {
                bool flag6 = false;
                for (int num92 = y; num92 > y - 10; num92--)
                {
                    if (Main.tile[x, num92].bottomSlope())
                    {
                        flag6 = false;
                        break;
                    }
                    if (Main.tile[x, num92].active() && Main.tile[x, num92].type == 109 && !Main.tile[x, num92].bottomSlope())
                    {
                        flag6 = true;
                        break;
                    }
                }
                if (flag6)
                {
                    int num93 = x;
                    int num94 = y + 1;
                    Main.tile[num93, num94].type = 115;
                    Main.tile[num93, num94].active(active: true);
                    WorldGen.SquareTileFrame(num93, num94);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, num93, num94, 3);
                    }
                }
            }
            TileLoader.RandomUpdate(x, y, Main.tile[x, y].type);
            WallLoader.RandomUpdate(x, y, Main.tile[x, y].wall);
            if ((Main.tile[x, y].type != 199 && Main.tile[x, y].type != 205) || WorldGen.genRand.Next(15) != 0 || Main.tile[x, y + 1].active() || Main.tile[x, y + 1].lava())
            {
                return;
            }
            bool flag7 = false;
            for (int num95 = y; num95 > y - 10; num95--)
            {
                if (Main.tile[x, num95].bottomSlope())
                {
                    flag7 = false;
                    break;
                }
                if (Main.tile[x, num95].active() && Main.tile[x, num95].type == 199 && !Main.tile[x, num95].bottomSlope())
                {
                    flag7 = true;
                    break;
                }
            }
            if (flag7)
            {
                int num97 = x;
                int num98 = y + 1;
                Main.tile[num97, num98].type = 205;
                Main.tile[num97, num98].active(active: true);
                WorldGen.SquareTileFrame(num97, num98);
                if (Main.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, num97, num98, 3);
                }
            }
        }

        public static void RandomUpdateTile_Misc(int x, int y, int maxValue = -1)
        {
            if (maxValue == -1)
            {
                maxValue = 40;
                if (Main.expertMode)
                {
                    maxValue = 30;
                }
            }
            int num = 20;
            int num100 = x;
            int num101 = y;
            int num102 = num100 - 1;
            int num103 = num100 + 2;
            int num104 = num101 - 1;
            int num105 = num101 + 2;
            if (num102 < 10)
            {
                num102 = 10;
            }
            if (num103 > Main.maxTilesX - 10)
            {
                num103 = Main.maxTilesX - 10;
            }
            if (num104 < 10)
            {
                num104 = 10;
            }
            if (num105 > Main.maxTilesY - 10)
            {
                num105 = Main.maxTilesY - 10;
            }
            if (Main.tile[num100, num101] == null)
            {
                return;
            }
            if (Main.tileAlch[Main.tile[num100, num101].type])
            {
                WorldGen.GrowAlch(num100, num101);
            }
            if (Main.tile[num100, num101].liquid <= 32)
            {
                if (Main.tile[num100, num101].nactive())
                {
                    WorldGen.hardUpdateWorld(num100, num101);
                    if (Main.rand.Next(3000) == 0)
                    {
                        WorldGen.plantDye(num100, num101);
                    }
                    if (Main.rand.Next(4500) == 0)
                    {
                        WorldGen.plantDye(num100, num101, exoticPlant: true);
                    }
                    if (Main.tile[num100, num101].type == 23 && !Main.tile[num100, num104].active() && WorldGen.genRand.Next(1) == 0)
                    {
                        WorldGen.PlaceTile(num100, num104, 24, mute: true);
                        if (Main.netMode == 2 && Main.tile[num100, num104].active())
                        {
                            NetMessage.SendTileSquare(-1, num100, num104, 1);
                        }
                    }
                    if (Main.tile[num100, num101].type == 32 && WorldGen.genRand.Next(3) == 0)
                    {
                        int num106 = num100;
                        int num108 = num101;
                        int num109 = 0;
                        if (Main.tile[num106 + 1, num108].active() && Main.tile[num106 + 1, num108].type == 32)
                        {
                            num109++;
                        }
                        if (Main.tile[num106 - 1, num108].active() && Main.tile[num106 - 1, num108].type == 32)
                        {
                            num109++;
                        }
                        if (Main.tile[num106, num108 + 1].active() && Main.tile[num106, num108 + 1].type == 32)
                        {
                            num109++;
                        }
                        if (Main.tile[num106, num108 - 1].active() && Main.tile[num106, num108 - 1].type == 32)
                        {
                            num109++;
                        }
                        if (num109 < 3 || Main.tile[num100, num101].type == 23)
                        {
                            switch (WorldGen.genRand.Next(4))
                            {
                                case 0:
                                num108--;
                                break;
                                case 1:
                                num108++;
                                break;
                                case 2:
                                num106--;
                                break;
                                case 3:
                                num106++;
                                break;
                            }
                            if (!Main.tile[num106, num108].active())
                            {
                                num109 = 0;
                                if (Main.tile[num106 + 1, num108].active() && Main.tile[num106 + 1, num108].type == 32)
                                {
                                    num109++;
                                }
                                if (Main.tile[num106 - 1, num108].active() && Main.tile[num106 - 1, num108].type == 32)
                                {
                                    num109++;
                                }
                                if (Main.tile[num106, num108 + 1].active() && Main.tile[num106, num108 + 1].type == 32)
                                {
                                    num109++;
                                }
                                if (Main.tile[num106, num108 - 1].active() && Main.tile[num106, num108 - 1].type == 32)
                                {
                                    num109++;
                                }
                                if (num109 < 2)
                                {
                                    int num110 = 7;
                                    int num141 = num106 - num110;
                                    int num111 = num106 + num110;
                                    int num112 = num108 - num110;
                                    int num113 = num108 + num110;
                                    bool flag8 = false;
                                    for (int num114 = num141; num114 < num111; num114++)
                                    {
                                        for (int num115 = num112; num115 < num113; num115++)
                                        {
                                            if (Math.Abs(num114 - num106) * 2 + Math.Abs(num115 - num108) < 9 && Main.tile[num114, num115].active() && Main.tile[num114, num115].type == 23 && Main.tile[num114, num115 - 1].active() && Main.tile[num114, num115 - 1].type == 32 && Main.tile[num114, num115 - 1].liquid == 0)
                                            {
                                                flag8 = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (flag8)
                                    {
                                        Main.tile[num106, num108].type = 32;
                                        Main.tile[num106, num108].active(active: true);
                                        WorldGen.SquareTileFrame(num106, num108);
                                        if (Main.netMode == 2)
                                        {
                                            NetMessage.SendTileSquare(-1, num106, num108, 3);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Main.tile[num100, num101].type == 352 && WorldGen.genRand.Next(3) == 0)
                    {
                        WorldGen.GrowSpike(num100, num101, 352, 199);
                    }
                    if (Main.tile[num100, num101].type == 199)
                    {
                        int type9 = Main.tile[num100, num101].type;
                        bool flag9 = false;
                        for (int num116 = num102; num116 < num103; num116++)
                        {
                            for (int num118 = num104; num118 < num105; num118++)
                            {
                                if ((num100 != num116 || num101 != num118) && Main.tile[num116, num118].active() && Main.tile[num116, num118].type == 0)
                                {
                                    WorldGen.SpreadGrass(num116, num118, 0, type9, repeat: false, Main.tile[num100, num101].color());
                                    if (Main.tile[num116, num118].type == type9)
                                    {
                                        WorldGen.SquareTileFrame(num116, num118);
                                        flag9 = true;
                                    }
                                }
                            }
                        }
                        if (Main.netMode == 2 && flag9)
                        {
                            NetMessage.SendTileSquare(-1, num100, num101, 3);
                        }
                    }
                    if (Main.tile[num100, num101].type == 60)
                    {
                        int type10 = Main.tile[num100, num101].type;
                        if (!Main.tile[num100, num104].active() && WorldGen.genRand.Next(10) == 0)
                        {
                            WorldGen.PlaceTile(num100, num104, 61, mute: true);
                            if (Main.netMode == 2 && Main.tile[num100, num104].active())
                            {
                                NetMessage.SendTileSquare(-1, num100, num104, 1);
                            }
                        }
                        else if (WorldGen.genRand.Next(25) == 0 && Main.tile[num100, num104].liquid == 0)
                        {
                            if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && WorldGen.genRand.Next(60) == 0)
                            {
                                bool flag10 = true;
                                int num119 = 150;
                                for (int num120 = num100 - num119; num120 < num100 + num119; num120 += 2)
                                {
                                    for (int num121 = num101 - num119; num121 < num101 + num119; num121 += 2)
                                    {
                                        if (num120 > 1 && num120 < Main.maxTilesX - 2 && num121 > 1 && num121 < Main.maxTilesY - 2 && Main.tile[num120, num121].active() && Main.tile[num120, num121].type == 238)
                                        {
                                            flag10 = false;
                                            break;
                                        }
                                    }
                                }
                                if (flag10)
                                {
                                    WorldGen.PlaceJunglePlant(num100, num104, 238, 0, 0);
                                    WorldGen.SquareTileFrame(num100, num104);
                                    WorldGen.SquareTileFrame(num100 + 1, num104 + 1);
                                    if (Main.tile[num100, num104].type == 238 && Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, num100, num104, 4);
                                    }
                                }
                            }
                            if (Main.hardMode && NPC.downedMechBossAny && WorldGen.genRand.Next(maxValue) == 0)
                            {
                                bool flag11 = true;
                                int num122 = 60;
                                if (Main.expertMode)
                                {
                                    num122 -= 10;
                                }
                                for (int num123 = num100 - num122; num123 < num100 + num122; num123 += 2)
                                {
                                    for (int num124 = num101 - num122; num124 < num101 + num122; num124 += 2)
                                    {
                                        if (num123 > 1 && num123 < Main.maxTilesX - 2 && num124 > 1 && num124 < Main.maxTilesY - 2 && Main.tile[num123, num124].active() && Main.tile[num123, num124].type == 236)
                                        {
                                            flag11 = false;
                                            break;
                                        }
                                    }
                                }
                                if (flag11)
                                {
                                    WorldGen.PlaceJunglePlant(num100, num104, TileID.LifeFruit, WorldGen.genRand.Next(3), 0);
                                    WorldGen.SquareTileFrame(num100, num104);
                                    WorldGen.SquareTileFrame(num100 + 1, num104 + 1);
                                    if (Main.tile[num100, num104].type == 236 && Main.netMode == 2)
                                    {
                                        NetMessage.SendTileSquare(-1, num100, num104, 4);
                                    }
                                }
                            }
                            else
                            {
                                WorldGen.PlaceJunglePlant(num100, num104, 233, WorldGen.genRand.Next(8), 0);
                                if (Main.tile[num100, num104].type == 233)
                                {
                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendTileSquare(-1, num100, num104, 4);
                                    }
                                    else
                                    {
                                        WorldGen.PlaceJunglePlant(num100, num104, 233, WorldGen.genRand.Next(12), 1);
                                        if (Main.tile[num100, num104].type == 233 && Main.netMode == 2)
                                        {
                                            NetMessage.SendTileSquare(-1, num100, num104, 3);
                                        }
                                    }
                                }
                            }
                        }
                        bool flag13 = false;
                        for (int num125 = num102; num125 < num103; num125++)
                        {
                            for (int num126 = num104; num126 < num105; num126++)
                            {
                                if ((num100 != num125 || num101 != num126) && Main.tile[num125, num126].active() && Main.tile[num125, num126].type == 59)
                                {
                                    WorldGen.SpreadGrass(num125, num126, 59, type10, repeat: false, Main.tile[num100, num101].color());
                                    if (Main.tile[num125, num126].type == type10)
                                    {
                                        WorldGen.SquareTileFrame(num125, num126);
                                        flag13 = true;
                                    }
                                }
                            }
                        }
                        if (Main.netMode == 2 && flag13)
                        {
                            NetMessage.SendTileSquare(-1, num100, num101, 3);
                        }
                    }
                    if (Main.tile[num100, num101].type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile[num100, num101].frameX < 144)
                    {
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.tile[num100, num101].frameX = (short)(162 + WorldGen.genRand.Next(8) * 18);
                        }
                        Main.tile[num100, num101].type = 74;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num100, num101, 3);
                        }
                    }
                    if ((Main.tile[num100, num101].type == 60 || Main.tile[num100, num101].type == 62) && WorldGen.genRand.Next(5) == 0 && !Main.tile[num100, num101 + 1].active() && !Main.tile[num100, num101 + 1].lava())
                    {
                        bool flag14 = false;
                        for (int num127 = num101; num127 > num101 - 10; num127--)
                        {
                            if (Main.tile[num100, num127].bottomSlope())
                            {
                                flag14 = false;
                                break;
                            }
                            if (Main.tile[num100, num127].active() && Main.tile[num100, num127].type == 60 && !Main.tile[num100, num127].bottomSlope())
                            {
                                flag14 = true;
                                break;
                            }
                        }
                        if (flag14)
                        {
                            int num129 = num100;
                            int num130 = num101 + 1;
                            Main.tile[num129, num130].type = 62;
                            Main.tile[num129, num130].active(active: true);
                            WorldGen.SquareTileFrame(num129, num130);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num129, num130, 3);
                            }
                        }
                    }
                    if ((Main.tile[num100, num101].type == 60 || Main.tile[num100, num101].type == 62) && WorldGen.genRand.Next(80) == 0 && !WorldGen.PlayerLOS(num100, num101))
                    {
                        bool flag15 = true;
                        int num131 = num101;
                        if (Main.tile[num100, num101].type == 60)
                        {
                            num131++;
                        }
                        for (int num132 = num100; num132 < num100 + 2; num132++)
                        {
                            int num133 = num131 - 1;
                            if (!WorldGen.AnchorValid(Framing.GetTileSafely(num132, num133), AnchorType.SolidTile) || Main.tile[num132, num133].bottomSlope())
                            {
                                flag15 = false;
                            }
                            if (Main.tile[num132, num133].liquid > 0 || Main.wallHouse[Main.tile[num132, num133].wall])
                            {
                                flag15 = false;
                            }
                            if (!flag15)
                            {
                                break;
                            }
                            for (int num134 = num131; num134 < num131 + 2; num134++)
                            {
                                if ((Main.tile[num132, num134].active() && (!Main.tileCut[Main.tile[num132, num134].type] || Main.tile[num132, num134].type == 444)) || Main.tile[num132, num134].lava())
                                {
                                    flag15 = false;
                                }
                                if (!flag15)
                                {
                                    break;
                                }
                            }
                            if (!flag15)
                            {
                                break;
                            }
                        }
                        if (flag15 && CountNearBlocksTypes(num100, num101, 20, 1, 444) > 0)
                        {
                            flag15 = false;
                        }
                        if (flag15)
                        {
                            for (int num135 = num100; num135 < num100 + 2; num135++)
                            {
                                Main.tile[num135, num131 - 1].slope(0);
                                Main.tile[num135, num131 - 1].halfBrick(halfBrick: false);
                                for (int num136 = num131; num136 < num131 + 2; num136++)
                                {
                                    if (Main.tile[num135, num136].active())
                                    {
                                        WorldGen.KillTile(num135, num136);
                                    }
                                }
                            }
                            for (int num137 = num100; num137 < num100 + 2; num137++)
                            {
                                for (int num138 = num131; num138 < num131 + 2; num138++)
                                {
                                    Main.tile[num137, num138].active(active: true);
                                    Main.tile[num137, num138].type = 444;
                                    Main.tile[num137, num138].frameX = (short)((num137 - num100) * 18);
                                    Main.tile[num137, num138].frameY = (short)((num138 - num131) * 18);
                                }
                            }
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num100, num131, 3);
                            }
                        }
                    }
                    if (Main.tile[num100, num101].type == 69 && WorldGen.genRand.Next(3) == 0)
                    {
                        int num3 = num100;
                        int num4 = num101;
                        int num5 = 0;
                        if (Main.tile[num3 + 1, num4].active() && Main.tile[num3 + 1, num4].type == 69)
                        {
                            num5++;
                        }
                        if (Main.tile[num3 - 1, num4].active() && Main.tile[num3 - 1, num4].type == 69)
                        {
                            num5++;
                        }
                        if (Main.tile[num3, num4 + 1].active() && Main.tile[num3, num4 + 1].type == 69)
                        {
                            num5++;
                        }
                        if (Main.tile[num3, num4 - 1].active() && Main.tile[num3, num4 - 1].type == 69)
                        {
                            num5++;
                        }
                        if (num5 < 3 || Main.tile[num100, num101].type == 60)
                        {
                            switch (WorldGen.genRand.Next(4))
                            {
                                case 0:
                                num4--;
                                break;
                                case 1:
                                num4++;
                                break;
                                case 2:
                                num3--;
                                break;
                                case 3:
                                num3++;
                                break;
                            }
                            if (!Main.tile[num3, num4].active())
                            {
                                num5 = 0;
                                if (Main.tile[num3 + 1, num4].active() && Main.tile[num3 + 1, num4].type == 69)
                                {
                                    num5++;
                                }
                                if (Main.tile[num3 - 1, num4].active() && Main.tile[num3 - 1, num4].type == 69)
                                {
                                    num5++;
                                }
                                if (Main.tile[num3, num4 + 1].active() && Main.tile[num3, num4 + 1].type == 69)
                                {
                                    num5++;
                                }
                                if (Main.tile[num3, num4 - 1].active() && Main.tile[num3, num4 - 1].type == 69)
                                {
                                    num5++;
                                }
                                if (num5 < 2)
                                {
                                    int num6 = 7;
                                    int num142 = num3 - num6;
                                    int num7 = num3 + num6;
                                    int num8 = num4 - num6;
                                    int num9 = num4 + num6;
                                    bool flag16 = false;
                                    for (int num10 = num142; num10 < num7; num10++)
                                    {
                                        for (int num11 = num8; num11 < num9; num11++)
                                        {
                                            if (Math.Abs(num10 - num3) * 2 + Math.Abs(num11 - num4) < 9 && Main.tile[num10, num11].active() && Main.tile[num10, num11].type == 60 && Main.tile[num10, num11 - 1].active() && Main.tile[num10, num11 - 1].type == 69 && Main.tile[num10, num11 - 1].liquid == 0)
                                            {
                                                flag16 = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (flag16)
                                    {
                                        Main.tile[num3, num4].type = 69;
                                        Main.tile[num3, num4].active(active: true);
                                        WorldGen.SquareTileFrame(num3, num4);
                                        if (Main.netMode == 2)
                                        {
                                            NetMessage.SendTileSquare(-1, num3, num4, 3);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Main.tile[num100, num101].type == 147 || Main.tile[num100, num101].type == 161 || Main.tile[num100, num101].type == 163 || Main.tile[num100, num101].type == 164 || Main.tile[num100, num101].type == 200)
                    {
                        if (Main.rand.Next(10) == 0 && !Main.tile[num100, num101 + 1].active() && !Main.tile[num100, num101 + 2].active())
                        {
                            int num143 = num100 - 3;
                            int num13 = num100 + 4;
                            int num14 = 0;
                            for (int num15 = num143; num15 < num13; num15++)
                            {
                                if (Main.tile[num15, num101].type == 165 && Main.tile[num15, num101].active())
                                {
                                    num14++;
                                }
                                if (Main.tile[num15, num101 + 1].type == 165 && Main.tile[num15, num101 + 1].active())
                                {
                                    num14++;
                                }
                                if (Main.tile[num15, num101 + 2].type == 165 && Main.tile[num15, num101 + 2].active())
                                {
                                    num14++;
                                }
                                if (Main.tile[num15, num101 + 3].type == 165 && Main.tile[num15, num101 + 3].active())
                                {
                                    num14++;
                                }
                            }
                            if (num14 < 2)
                            {
                                WorldGen.PlaceTight(num100, num101 + 1, 165);
                                WorldGen.SquareTileFrame(num100, num101 + 1);
                                if (Main.netMode == 2 && Main.tile[num100, num101 + 1].active())
                                {
                                    NetMessage.SendTileSquare(-1, num100, num101 + 1, 3);
                                }
                            }
                        }
                    }
                    else if (Main.tileMoss[Main.tile[num100, num101].type])
                    {
                        int type11 = Main.tile[num100, num101].type;
                        bool flag17 = false;
                        for (int num16 = num102; num16 < num103; num16++)
                        {
                            for (int num17 = num104; num17 < num105; num17++)
                            {
                                if ((num100 != num16 || num101 != num17) && Main.tile[num16, num17].active() && Main.tile[num16, num17].type == 1)
                                {
                                    WorldGen.SpreadGrass(num16, num17, 1, type11, repeat: false, Main.tile[num100, num101].color());
                                    if (Main.tile[num16, num17].type == type11)
                                    {
                                        WorldGen.SquareTileFrame(num16, num17);
                                        flag17 = true;
                                    }
                                }
                            }
                        }
                        if (Main.netMode == 2 && flag17)
                        {
                            NetMessage.SendTileSquare(-1, num100, num101, 3);
                        }
                        if (WorldGen.genRand.Next(6) == 0)
                        {
                            int num18 = num100;
                            int num19 = num101;
                            switch (WorldGen.genRand.Next(4))
                            {
                                case 0:
                                num18--;
                                break;
                                case 1:
                                num18++;
                                break;
                                case 2:
                                num19--;
                                break;
                                default:
                                num19++;
                                break;
                            }
                            if (!Main.tile[num18, num19].active())
                            {
                                WorldGen.PlaceTile(num18, num19, 184, mute: true);
                                if (Main.netMode == 2 && Main.tile[num18, num19].active())
                                {
                                    NetMessage.SendTileSquare(-1, num18, num19, 1);
                                }
                            }
                        }
                    }
                    if (Main.tile[num100, num101].type == 70)
                    {
                        int type12 = Main.tile[num100, num101].type;
                        if (!Main.tile[num100, num104].active() && WorldGen.genRand.Next(10) == 0)
                        {
                            WorldGen.PlaceTile(num100, num104, 71, mute: true);
                            if (Main.netMode == 2 && Main.tile[num100, num104].active())
                            {
                                NetMessage.SendTileSquare(-1, num100, num104, 1);
                            }
                        }
                        if (WorldGen.genRand.Next(200) == 0 && WorldGen.GrowShroom(num100, num101) && WorldGen.PlayerLOS(num100, num101))
                        {
                            WorldGen.TreeGrowFXCheck(num100, num101 - 1);
                        }
                        bool flag18 = false;
                        for (int num20 = num102; num20 < num103; num20++)
                        {
                            for (int num21 = num104; num21 < num105; num21++)
                            {
                                if ((num100 != num20 || num101 != num21) && Main.tile[num20, num21].active() && Main.tile[num20, num21].type == 59)
                                {
                                    WorldGen.SpreadGrass(num20, num21, 59, type12, repeat: false, Main.tile[num100, num101].color());
                                    if (Main.tile[num20, num21].type == type12)
                                    {
                                        WorldGen.SquareTileFrame(num20, num21);
                                        flag18 = true;
                                    }
                                }
                            }
                        }
                        if (Main.netMode == 2 && flag18)
                        {
                            NetMessage.SendTileSquare(-1, num100, num101, 3);
                        }
                    }
                }
                else
                {
                    if (Main.tile[num100, num101].wall == 62 && Main.tile[num100, num101].liquid == 0 && WorldGen.genRand.Next(10) == 0)
                    {
                        int num23 = WorldGen.genRand.Next(2, 4);
                        int num144 = num100 - num23;
                        int num24 = num100 + num23;
                        int num25 = num101 - num23;
                        int num26 = num101 + num23;
                        bool flag19 = false;
                        for (int num27 = num144; num27 <= num24; num27++)
                        {
                            for (int num28 = num25; num28 <= num26; num28++)
                            {
                                if (WorldGen.SolidTile(num27, num28))
                                {
                                    flag19 = true;
                                    break;
                                }
                            }
                        }
                        if (flag19 && !Main.tile[num100, num101].active())
                        {
                            WorldGen.PlaceTile(num100, num101, 51, mute: true);
                            WorldGen.TileFrame(num100, num101, resetFrame: true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num100, num101, 3);
                            }
                        }
                    }
                }
            }
            if (Main.tile[num100, num101].wall == 81 || Main.tile[num100, num101].wall == 83 || (Main.tile[num100, num101].type == 199 && Main.tile[num100, num101].active()))
            {
                int num29 = num100 + WorldGen.genRand.Next(-2, 3);
                int num30 = num101 + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num29, num30].wall >= 63 && Main.tile[num29, num30].wall <= 68)
                {
                    bool flag20 = false;
                    for (int num31 = num100 - num; num31 < num100 + num; num31++)
                    {
                        for (int num33 = num101 - num; num33 < num101 + num; num33++)
                        {
                            if (Main.tile[num100, num101].active())
                            {
                                int type2 = Main.tile[num100, num101].type;
                                if (type2 == 199 || type2 == 200 || type2 == 201 || type2 == 203 || type2 == 205 || type2 == 234 || type2 == 352)
                                {
                                    flag20 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag20)
                    {
                        Main.tile[num29, num30].wall = 81;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num29, num30, 3);
                        }
                    }
                }
            }
            if (Main.tile[num100, num101].wall == 69 || Main.tile[num100, num101].wall == 3 || (Main.tile[num100, num101].type == 23 && Main.tile[num100, num101].active()))
            {
                int num34 = num100 + WorldGen.genRand.Next(-2, 3);
                int num35 = num101 + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num34, num35].wall >= 63 && Main.tile[num34, num35].wall <= 68)
                {
                    bool flag21 = false;
                    for (int num36 = num100 - num; num36 < num100 + num; num36++)
                    {
                        for (int num37 = num101 - num; num37 < num101 + num; num37++)
                        {
                            if (Main.tile[num100, num101].active())
                            {
                                int type3 = Main.tile[num100, num101].type;
                                if (type3 == 22 || type3 == 23 || type3 == 24 || type3 == 25 || type3 == 32 || type3 == 112 || type3 == 163)
                                {
                                    flag21 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag21)
                    {
                        Main.tile[num34, num35].wall = 69;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num34, num35, 3);
                        }
                    }
                }
            }
            if (Main.tile[num100, num101].wall == 70 || (Main.tile[num100, num101].type == 109 && Main.tile[num100, num101].active()))
            {
                int num38 = num100 + WorldGen.genRand.Next(-2, 3);
                int num39 = num101 + WorldGen.genRand.Next(-2, 3);
                if (Main.tile[num38, num39].wall == 63 || Main.tile[num38, num39].wall == 65 || Main.tile[num38, num39].wall == 66 || Main.tile[num38, num39].wall == 68)
                {
                    bool flag22 = false;
                    for (int num40 = num100 - num; num40 < num100 + num; num40++)
                    {
                        for (int num41 = num101 - num; num41 < num101 + num; num41++)
                        {
                            if (Main.tile[num100, num101].active())
                            {
                                int type4 = Main.tile[num100, num101].type;
                                if (type4 == 109 || type4 == 110 || type4 == 113 || type4 == 115 || type4 == 116 || type4 == 117 || type4 == 164)
                                {
                                    flag22 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag22)
                    {
                        Main.tile[num38, num39].wall = 70;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendTileSquare(-1, num38, num39, 3);
                        }
                    }
                }
            }
            WorldGen.SpreadDesertWalls(num, num100, num101);
            TileLoader.RandomUpdate(num100, num101, Main.tile[num100, num101].type);
            WallLoader.RandomUpdate(num100, num101, Main.tile[num100, num101].wall);
        }

        internal static bool TileObstructedFromLight(int x, int y)
        {
            return Main.tile[x, y].wall > 0 || (Main.tile[x, y].active() && Main.tileBlockLight[Main.tile[x, y].type]);
        }
    }
}