using AQMod.Content.World;
using AQMod.Localization;
using AQMod.Tiles;
using AQMod.Tiles.Furniture;
using AQMod.Tiles.Nature;
using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
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
            var tileType = ModContent.TileType<NobleMushroomsNew>();
            var anchorValidTiles = NobleMushroomsNew.AnchorValidTiles;
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
                        for (int k = 0; k < anchorValidTiles.Length; k++)
                        {
                            if (anchorValidTiles[k] == type)
                            {
                                canPlace = true;
                                break;
                            }
                        }
                        type = Main.tile[i + 1, j + 1].type;
                        bool canPlace2 = false;
                        for (int k = 0; k < anchorValidTiles.Length; k++)
                        {
                            if (anchorValidTiles[k] == type)
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
            for (int i = 0; i < 6000; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow + 80, Main.maxTilesY - 240);

                int style = WorldGen.genRand.Next(3);
                int size = WorldGen.genRand.Next(50, 150);
                if (ExoticCoralNew.TryPlaceExoticBlotch(x, y, style, size))
                    i += 500;
            }
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
                    if (!Main.tile[i, j].active() && Main.tile[i, j].liquid > 40 && Main.tile[i, j + 1].active() && !Main.tile[i, j + 1].halfBrick() && Main.tile[i, j + 1].slope() == 0 && Main.tile[i, j + 1].type == ModContent.TileType<SedimentSand>())
                    {
                        Main.tile[i, j].active(active: true);
                        Main.tile[i, j].halfBrick(halfBrick: false);
                        Main.tile[i, j].slope(slope: 0);
                        Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                        Main.tile[i, j].frameX = (short)(22 * ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                        Main.tile[i, j].frameY = 0;
                    }
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int i;
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
                tasks.Insert(i, getPass("AQMod: Fix Baby Pools", GenerationWaterCleaner.PassFix1TileHighWater));
            }
            i = tasks.FindIndex((t) => t.Name.Equals("Micro Biomes"));
            if (i != -1)
            {
                i++;
                tasks.Insert(i, getPass("AQMod: Noble Mushrooms", GenerateNobleMushrooms));
                tasks.Insert(i, getPass("AQMod: Candelabra Traps", GenerateCandelabraTraps));
                tasks.Insert(i, getPass("AQMod: Exotic Coral", GenerateExoticBlotches));
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

        internal static bool TileObstructedFromLight(int x, int y)
        {
            return Main.tile[x, y].wall > 0 || (Main.tile[x, y].active() && Main.tileBlockLight[Main.tile[x, y].type]);
        }
    }
}