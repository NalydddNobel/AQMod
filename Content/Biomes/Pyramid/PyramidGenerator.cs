using Aequus.Content.Biomes.Pyramid.Tiles;
using Aequus.Content.World.Generation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.Pyramid {
    public class PyramidGenerator : Generator {
        private bool _generatedPyramid;

        public override void Load() {
            On_WorldGen.Pyramid += On_WorldGen_Pyramid;
        }

        private bool On_WorldGen_Pyramid(On_WorldGen.orig_Pyramid orig, int i, int j) {
            var value = orig(i, j);
            _generatedPyramid |= value;
            return value;
        }

        public override void Initialize() {
            _generatedPyramid = false;
        }

        public override void AddPass(List<GenPass> tasks, ref double totalWeight) {
            AddPass("Pyramids", "Pyramid", (progress, configuration) 
                => ModContent.GetInstance<PyramidGenerator>().Generate(progress, configuration), tasks);
        }

        public void ForceGeneratePyramid() {
            for (int i = 0; i < 100000; i++) {
                int size = Math.Max(100 - i / 1000, 2);
                int pyrX = Rand.Next(300, Main.maxTilesX - 300);
                int pyrY = Rand.Next(200, (int)Main.worldSurface);
                var tile = Framing.GetTileSafely(pyrX, pyrY);
                if (!tile.HasTile || !Main.tileSand[tile.TileType]
                    || !AequusWorldGenerator.CanPlaceStructure(pyrX, pyrY, 100, 200) 
                    || !TileHelper.ScanUp(new(pyrX, pyrY), 100, out var p, TileHelper.HasNoTileAndNoWall)) {
                    continue;
                }

                if (WorldGen.Pyramid(p.X, p.Y - 20)) {
                    break;
                }
            }
        }

        private void PlaceChest(int chestX, int chestY) {
            int loot = Rand.Next(3) switch {
                0 => ItemID.PharaohsMask,
                1 => ItemID.SandstorminaBottle,
                _ => ItemID.FlyingCarpet,
            };
            WorldGen.AddBuriedChest(chestX, chestY, Style: 1, contain: loot, notNearOtherChests: false);
        }

        public void GenerateRoom(int pyrX, int pyrY) {
            int left = pyrX - 14;
            int top = pyrY - 13;
            int right = pyrX + 14;
            int bottom = pyrY;
            for (int x = left; x <= right; x++) {
                for (int y = top; y <= bottom; y++) {
                    if ((y >= bottom - 1 || y <= top + 1) && Main.tile[x, y].WallType == WallID.SandstoneBrick && !Main.tile[x, y].IsFullySolid()) {
                        WorldGen.PlaceTile(x, y, TileID.Platforms, forced: true, style: 0);
                    }
                    else {
                        WorldGen.KillTile(x, y);
                    }
                    Main.tile[x, y].WallType = WallID.SandstoneBrick;
                }
            }

            for (int x = -2; x < 2; x++) {
                for (int y = top; y < bottom; y++) {
                    WorldGen.PlaceTile(left + x, y, TileID.SandstoneBrick);
                    WorldGen.PlaceTile(right - x, y, TileID.SandstoneBrick);
                }
            }

            for (int y = 0; y < 2; y++) {
                for (int x = left; x <= right; x++) {
                    WorldGen.PlaceTile(x, top + y, TileID.SandstoneBrick);
                    WorldGen.PlaceTile(x, bottom - y, TileID.SandstoneBrick);
                }
            }


            WorldGen.PlaceTile(pyrX, bottom - 2, ModContent.TileType<PyramidStatueTile>());
            WorldGen.PlaceTile(left + 4, bottom - 2, TileID.Lamps, style: 38);
            WorldGen.PlaceTile(right - 4, bottom - 2, TileID.Lamps, style: 38);
            for (int k = 0; k < 300; k++) {
                int x = Rand.Next(left, right);
                if (!TileHelper.ScanTiles(new(x - 1, bottom - 3, 3, 2), TileHelper.HasTile)) {
                    PlaceChest(x, bottom - 2);
                    break;
                }
            }
            for (int k = 0; k < 30; k++) {
                int x = Rand.Next(left, right);
                WorldGen.PlacePot(x, bottom - 2, TileID.Pots, Rand.Next(25, 28));
            }
        }

        protected override void Generate() {
            if (!_generatedPyramid) {
                ForceGeneratePyramid();
            }

            for (int i = 0; i < Main.maxTilesX * 100; i++) {
                int pyrX = Rand.Next(300, Main.maxTilesX - 300);
                int pyrY = Rand.Next(200, (int)GenVars.rockLayer);
                var tile = Framing.GetTileSafely(pyrX, pyrY);
                if (tile.WallType != WallID.SandstoneBrick || tile.IsFullySolid()
                    || TileHelper.ScanTilesSquare(pyrX, pyrY + 30, 60, TileHelper.HasContainer)) {
                    continue;
                }

                GenerateRoom(pyrX, pyrY);
                break;
            }
        }
    }
}