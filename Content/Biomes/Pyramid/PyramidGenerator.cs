using Aequus.Common.World;
using Aequus.Tiles.Pyramid;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
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

        public void GenerateSecretRoom(int pyrX, int pyrY) {
            int left = pyrX - 14;
            int top = pyrY - 13;
            int right = pyrX + 14;
            int bottom = pyrY;
            int brick = TileID.SandstoneBrick;
            int platformStyle = 42;
            byte platformPaint = PaintID.None;
            int lampStyle = 38;
            // Make an empty square and place platforms
            for (int x = left; x <= right; x++) {
                for (int y = top; y <= bottom; y++) {
                    var wallType = Main.tile[x, y].WallType;
                    Main.tile[x, y].WallType = WallID.SandstoneBrick;
                    if ((y >= bottom - 1 || y <= top + 1) && wallType == WallID.SandstoneBrick && !Main.tile[x, y].IsFullySolid()) {
                        if (y == bottom || y == top + 1) {
                            continue;
                        }
                        WorldGen.PlaceTile(x, y, TileID.Platforms, forced: true, style: platformStyle);
                        WorldGen.paintTile(x, y, platformPaint, broadCast: false);
                    }
                    else {
                        WorldGen.KillTile(x, y);
                    }
                }
            }

            // Walls
            for (int x = -2; x < 2; x++) {
                for (int y = top; y < bottom; y++) {
                    WorldGen.PlaceTile(left + x, y, brick);
                    WorldGen.PlaceTile(right - x, y, brick);
                }
            }

            // Roof
            for (int y = 0; y < 2; y++) {
                for (int x = left; x <= right; x++) {
                    if (Main.tile[x, top + y - 1].TileType != TileID.Platforms) {
                        WorldGen.PlaceTile(x, top + y, brick);
                    }
                    if (Main.tile[x, bottom - y - 1].TileType != TileID.Platforms) {
                        WorldGen.PlaceTile(x, bottom - y, brick);
                    }
                }
            }

            // Small roof curve
            var platformsAttempt = TileHelper.HasTileAction(TileID.Platforms);
            for (int roofCurveX = 0; roofCurveX < 3; roofCurveX++) {
                if (!TileHelper.ScanUp(new Point(left + 2 + roofCurveX, top + 2), 3, out _, platformsAttempt)) {
                    WorldGen.PlaceTile(left + 2 + roofCurveX, top + 2, brick);
                    if (roofCurveX == 0) {
                        WorldGen.PlaceTile(left + 2 + roofCurveX, top + 3, brick);
                    }
                }
                if (!TileHelper.ScanUp(new Point(right - 2 - roofCurveX, top + 2), 3, out _, platformsAttempt)) {
                    WorldGen.PlaceTile(right - 2 - roofCurveX, top + 2, brick);
                    if (roofCurveX == 0) {
                        WorldGen.PlaceTile(right - 2 - roofCurveX, top + 3, brick);
                    }
                }
            }

            WorldGen.PlaceTile(pyrX, bottom - 2, ModContent.TileType<PyramidStatueTile>());
            WorldGen.PlaceTile(left + 4, bottom - 2, TileID.Lamps, style: lampStyle);
            WorldGen.PlaceTile(right - 4, bottom - 2, TileID.Lamps, style: lampStyle);
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
                    || TileHelper.ScanTilesSquare(pyrX, pyrY + 15, 100, TileHelper.HasContainer) 
                    || TileHelper.ScanTilesSquare(pyrX, pyrY, 3, TileHelper.IsSolid)) {
                    continue;
                }

                GenerateSecretRoom(pyrX, pyrY);
                break;
            }
        }
    }
}