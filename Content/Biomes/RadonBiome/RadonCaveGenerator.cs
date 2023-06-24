using Aequus.Items.Materials.Gems;
using Aequus.Tiles.MossCaves.Radon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.RadonBiome {
    public class RadonCaveGenerator {
        public float sizeX;
        public float sizeY;
        private int mossTileID;

        public int MaxWidth => (int)(Main.maxTilesX * sizeX);
        public int MaxHeight => (int)(Main.maxTilesY * sizeY);

        private UnifiedRandom Rand => WorldGen.genRand;

        public RadonCaveGenerator() {
            sizeX = 0.05f;
            sizeY = 0.33f;
        }

        public void GenerateWorld() {
            for (int k = 0; k < 100000; k++) {
                int x = Rand.Next(50, Main.maxTilesX / 4);
                if (Main.dungeonX * 2 > Main.maxTilesX) {
                    x = Main.maxTilesX - x;
                }
                int y = Rand.Next((int)Main.rockLayer + 200, (int)Main.rockLayer + 500);
                if (ValidSpotForCave(x, y) && CreateCave(x, y)) {
                    break;
                }
            }
        }

        public bool ValidSpotForCave(int x, int y) {
            return x - MaxHeight > (int)Main.worldSurface && WorldGen.InWorld(x, y, fluff: Math.Max(MaxWidth / 2, MaxHeight / 2) + 10);
        }

        private bool CheckStone(int x, int y) {

            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    var tile = Main.tile[x + i, y + j];
                    if (tile.IsFullySolid()) {
                        return tile.TileType == mossTileID;
                    }
                }
            }
            return false;
        }

        public void GenerateGemstones(int x, int y, int w, int h) {

            int tileID = ModContent.TileType<MonoGemTile>();
            int amt = Main.maxTilesX * Main.maxTilesY / 50;
            for (int i = 0; i < amt; i++) {
                int gemstoneX = Rand.Next(x - w, x + w);
                int gemstoneY = Rand.Next(y - h, y + h);

                if (!WorldGen.InWorld(gemstoneX, gemstoneY, 5)
                    || Main.wallDungeon[Main.tile[gemstoneX, gemstoneY].WallType] || Main.tile[gemstoneX, gemstoneY].WallType == WallID.LihzahrdBrickUnsafe
                    || !CheckStone(gemstoneX, gemstoneY)) {
                    continue;
                }

                WorldGen.PlaceTile(gemstoneX, gemstoneY, tileID, mute: true);

                if (Main.tile[gemstoneX, gemstoneY].HasTile) {
                    if (Main.tile[gemstoneX, gemstoneY].TileType == tileID) {
                        i += 1000;
                        continue;
                    }
                }
            }
        }

        public bool CreateCave(int x, int y) {
            mossTileID = ModContent.TileType<RadonMossTile>();
            int w = MaxWidth;
            int h = MaxHeight;
            if (!GrowGrass(x, y, w, h)) {
                return false;
            }
            for (int i = 0; i < w * 100; i++) {
                if (GrowStalactite(Rand.Next(x - w, x + w), Rand.Next(y - h, y + h), w, h)) {
                    i += 500;
                }
            }
            GrowGrass(x, y, w, h);
            GenerateGemstones(x, y, w, h);
            GenVars.structures.AddStructure(new(x - w / 2 - 10, y - h / 2 - 10, w + 20, h + 20));
            return true;
        }
        public bool GrowGrass(int x, int y, int w, int h) {
            var rect = new Rectangle(x - w / 2 - 10, y - h / 2 - 10, w + 20, h + 20);
            float xAspect = 1f;
            float yAspect = 1f;
            float distance = Math.Max(w, h) / 2f;
            if (w > h) {
                yAspect = w / (float)h;
            }
            else {
                xAspect = h / (float)w;
            }
            int grewGrass = 0;
            for (int i = rect.X; i < rect.X + rect.Width; i++) {
                int k = x - i;
                for (int j = rect.Y; j < rect.Y + rect.Height; j++) {
                    int l = y - j;
                    if (Main.tile[i, j].HasTile) {
                        if (Math.Sqrt(k * k * xAspect + l * l * yAspect) + Rand.Next(10) < distance) {
                            if (Main.tile[i, j].TileType == TileID.Dirt) {
                                Main.tile[i, j].TileType = TileID.Stone;
                            }
                            if (Main.tile[i, j].TileType == TileID.Stone || Main.tileMoss[Main.tile[i, j].TileType]) {
                                for (int m = -1; m <= 1; m++) {
                                    for (int n = -1; n <= 1; n++) {
                                        if (!Main.tile[i + m, j + n].IsFullySolid()) {
                                            WorldGen.PlaceTile(i, j, ModContent.TileType<RadonMossTile>());
                                            RadonMossTile.GrowLongMoss(i, j);
                                            grewGrass++;
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
            return grewGrass > 300;
        }
        public bool GrowStalactite(int x, int y, int w, int h) {
            int topY = y - h / 2;
            int radon = ModContent.TileType<RadonMossTile>();
            for (int j = y; j > topY; j--) {
                if (!WorldGen.InWorld(x, j, fluff: 40))
                    break;
                if (Main.tile[x, j].HasTile && Main.tile[x, j].TileType == radon && !Main.tile[x, j + 1].IsFullySolid()) {
                    y = j;
                    return GrowStalactite_ActuallyGenerate(x, y);
                }
            }
            return false;
        }
        private bool GrowStalactite_ActuallyGenerate(int x, int y) {
            int width = Rand.Next(2, 5);
            int height = width * 6;
            if (TileHelper.ScanDown(new Point(x, y + 1), height, out var solidGround, TileHelper.IsSolid, TileHelper.HasAnyLiquid)) {
                height = (solidGround.Y - y) / 2;
                if (width > height / 3)
                    width = height / 3;
                if (height < 5 || width <= 1)
                    return false;
            }

            for (int i = x - width; i <= x + width; i++) {
                if (TileHelper.ScanUp(new Point(i, y), 15, out var roof)) {
                    if (roof.Y <= y - 14)
                        return false;
                    for (int j = y; j >= roof.Y; j--) {
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

            for (int j = y; j < y + height; j++) {
                if (WorldGen.genRand.NextBool(height)) {
                    widthLeft--;
                    if (widthLeft < 0)
                        widthLeft = 0;
                }
                if (WorldGen.genRand.NextBool(height)) {
                    widthRight--;
                    if (widthRight < 0)
                        widthRight = 0;
                }
                if (forceReduction >= forceReductionTime) {
                    forceReduction = 0;
                    widthLeft--;
                    widthRight--;
                    if (widthLeft < 0)
                        widthLeft = 0;
                    if (widthRight < 0)
                        widthRight = 0;
                }
                for (int i = x - widthLeft; i <= x + widthRight; i++) {
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
