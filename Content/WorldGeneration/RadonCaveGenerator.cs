using Aequus.Tiles.Moss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.WorldGeneration
{
    public class RadonCaveGenerator
    {
        public float sizeX;
        public float sizeY;

        public int MaxWidth => (int)(Main.maxTilesX * sizeX);
        public int MaxHeight => (int)(Main.maxTilesY * sizeY);

        private UnifiedRandom Rand => WorldGen.genRand;

        public RadonCaveGenerator()
        {
            sizeX = 0.02f;
            sizeY = 0.18f;
        }

        public void GenerateWorld()
        {
            int spawnedCount = 0;
            int amt = Main.maxTilesX / (AequusWorld.SmallWidth / 6);
            for (int k = 0; k < 100000 && spawnedCount < amt; k++)
            {
                int x = WorldGen.genRand.Next(MaxWidth, Main.maxTilesX - MaxWidth);
                int y = WorldGen.genRand.Next((int)Main.rockLayer - 50, Main.UnderworldLayer);
                if (ValidSpotForCave(x, y) && CreateCave(x, y))
                {
                    spawnedCount++;
                }
            }
        }

        public bool ValidSpotForCave(int x, int y)
        {
            return WorldGen.InWorld(x, y, fluff: Math.Max(MaxWidth, MaxHeight) + 10) && AequusWorldGenerator.CanPlaceStructure(x, y, MaxWidth, MaxHeight);
        }

        public bool CreateCave(int x, int y)
        {
            int w = MaxWidth;
            int h = MaxHeight;
            if (!GrowGrass(x, y, w, h))
            {
                return false;
            }
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    WorldGen.digTunnel(x, y, i, j, w / 2, 5, Wet: false);
                }
            }
            for (int i = 0; i < w * 100; i++)
            {
                if (GrowStalactite(Rand.Next(x - w, x + w), Rand.Next(y - h, y + h), w, h))
                {
                    i += 500;
                }
            }
            GrowGrass(x, y, w, h);
            return true;
        }
        public bool GrowGrass(int x, int y, int w, int h)
        {
            var rect = new Rectangle(x - w / 2 - 10, y - h / 2 - 10, w + 20, h + 20);
            float xAspect = 1f;
            float yAspect = 1f;
            float distance = Math.Max(w, h) / 2f;
            if (w > h)
            {
                yAspect = w / (float)h;
            }
            else
            {
                xAspect = h / (float)w;
            }
            bool grewGrass = false;
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                int k = x - i;
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    int l = y - j;
                    if (Main.tile[i, j].HasTile)
                    {
                        if (Math.Sqrt(k * k * xAspect + l * l * yAspect) + Rand.Next(10) < distance)
                        {
                            if (Main.tile[i, j].TileType == TileID.Dirt)
                            {
                                Main.tile[i, j].TileType = TileID.Stone;
                            }
                            if (Main.tile[i, j].TileType == TileID.Stone || Main.tileMoss[Main.tile[i, j].TileType])
                            {
                                for (int m = -1; m <= 1; m++)
                                {
                                    for (int n = -1; n <= 1; n++)
                                    {
                                        if (!Main.tile[i + m, j + n].IsFullySolid())
                                        {
                                            WorldGen.PlaceTile(i, j, ModContent.TileType<RadonMossTile>());
                                            RadonMossTile.GrowMoss(i, j);
                                            grewGrass = true;
                                            goto NextTile;
                                        }
                                    }
                                }
                            }
                        }
                    }
                NextTile:
                    continue;
                }
            }
            return grewGrass;
        }
        public bool GrowStalactite(int x, int y, int w, int h)
        {
            int topY = y - h / 2;
            int radon = ModContent.TileType<RadonMossTile>();
            for (int j = y; j > topY; j--)
            {
                if (Main.tile[x, j].HasTile && Main.tile[x, j].TileType == radon && !Main.tile[x, j + 1].IsFullySolid())
                {
                    y = j;
                    return GrowStalactite_ActuallyGenerate(x, y);
                }
            }
            return false;
        }
        private bool GrowStalactite_ActuallyGenerate(int x, int y)
        {
            int width = Rand.Next(2, 5);
            int height = width * 6;
            if (AequusHelpers.CheckForSolidGroundOrLiquidBelow(new Point(x, y + 1), height, out var solidGround))
            {
                height = (solidGround.Y - y) / 2;
                if (width > height / 3)
                    width = height / 3;
                if (height < 5 || width <= 1)
                    return false;
            }

            for (int i = x - width; i <= x + width; i++)
            {
                if (AequusHelpers.CheckForSolidRoofAbove(new Point(i, y), 15, out var roof))
                {
                    if (roof.Y <= y - 14)
                        return false;
                    for (int j = y; j >= roof.Y; j--)
                    {
                        var tile = Main.tile[i, j];
                        tile.Slope = SlopeType.Solid;
                        tile.IsHalfBlock = false;
                        WorldGen.PlaceTile(i, j, TileID.Stone, mute: true, forced: true);
                    }
                }
            }

            int widthRight = width;
            int widthLeft = width;
            int forceReductionTime = height / (width + 1);
            int forceReduction = 0;
            //Main.NewText(string.Join("|", width, height, forceReductionTime));

            for (int j = y; j < y + height; j++)
            {
                if (WorldGen.genRand.NextBool(height))
                {
                    widthLeft--;
                    if (widthLeft < 0)
                        widthLeft = 0;
                }
                if (WorldGen.genRand.NextBool(height))
                {
                    widthRight--;
                    if (widthRight < 0)
                        widthRight = 0;
                }
                if (forceReduction >= forceReductionTime)
                {
                    forceReduction = 0;
                    widthLeft--;
                    widthRight--;
                    if (widthLeft < 0)
                        widthLeft = 0;
                    if (widthRight < 0)
                        widthRight = 0;
                }
                for (int i = x - widthLeft; i <= x + widthRight; i++)
                {
                    var tile = Main.tile[i, j];
                    tile.Slope = SlopeType.Solid;
                    tile.IsHalfBlock = false;
                    WorldGen.PlaceTile(i, j, TileID.Stone, mute: true, forced: true);
                }
                forceReduction++;
            }
            return true;
        }
    }
}
