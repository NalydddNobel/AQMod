using Aequus.Content.World.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.Aether {
    public class LegacyAetherCavesGenerator : Generator {
        private class ActionBlockReplace : GenAction {
            private int w;
            private int h;
            public ActionBlockReplace(int radiusHorizontal, int radiusVertical) {
                w = radiusHorizontal;
                h = radiusVertical;
            }

            private bool RollPlacement(Point origin, int x, int y) {
                float progress = (y - origin.Y) / (float)h;
                if (progress > 0f) {
                    return true;
                }

                progress = MathF.Pow(1f - Math.Abs(progress), 6f);
                //progress *= 1f - Helper.CircleDistanceInterval(origin, x, y, w, h);
                return WorldGen.genRand.NextFloat() < progress;
            }

            public override bool Apply(Point origin, int x, int y, params object[] args) {
                if (!WorldGen.InWorld(x, y) || !RollPlacement(origin, x, y)) {
                    return false;
                }

                var tile = Main.tile[x, y];
                if (Main.tile[x, y].IsFullySolid() || Helper.CircleDistanceInterval(origin, x, y, w, h) > 0.9f) {
                    WorldGen.PlaceTile(x, y, TileID.Stone, mute: true, forced: true);
                }
                if (tile.LiquidType != LiquidID.Shimmer) {
                    Main.tile[x, y].LiquidAmount = 0;
                }
                tile.WallType = WallID.None;
                return true;
            }
        }

        public override void AddPass(List<GenPass> tasks, ref double totalWeight) {
            //AddPass("Shimmer", "Aether 2", (progress, configuration) => {
            //    ModContent.GetInstance<AetherCavesGenerator>().Generate(progress, configuration);
            //}, tasks);
            //AddPass("Settle Liquids", "Aether 2.5", (progress, configuration) => {
            //    ModContent.GetInstance<AetherCavesGenerator>().FinishAether();
            //}, tasks);
        }

        public bool GenerateWaterfall(int x, int y) {
            for (int l = 3; l < 10; l++) {
                int waterfallDir = Rand.NextBool() ? 1 : -1;
                int waterfallY = y + l;
                for (int k = 3; k < 10; k++) {
                    int waterfallX = x + k * waterfallDir;
                    var tile = Main.tile[waterfallX, waterfallY];
                    if (!tile.HasTile || tile.TileType != TileID.Stone) {

                        if (tile.IsFullySolid()) {
                            break;
                        }

                        continue;
                    }

                    if (!Main.tile[waterfallX + waterfallDir * 2, waterfallY].IsFullySolid()
                        || !Main.tile[waterfallX + waterfallDir, waterfallY + 1].IsFullySolid()
                        || !Main.tile[waterfallX + waterfallDir, waterfallY - 1].IsFullySolid()) {
                        break;
                    }

                    for (int m = -1; m <= 1; m++) {
                        for (int n = -1; n <= 1; n++) {
                            WorldGen.PlaceTile(waterfallX + m + waterfallDir, waterfallY + n, TileID.Stone, mute: true, forced: true);
                        }
                    }

                    var waterFallTile = Main.tile[waterfallX + waterfallDir, waterfallY];
                    waterFallTile.ClearTile();
                    waterFallTile.LiquidType = LiquidID.Shimmer;
                    waterFallTile.LiquidAmount = byte.MaxValue;

                    tile.Slope = SlopeType.Solid;
                    tile.HalfBrick(value: true);
                    return true;
                }
            }
            return false;
        }
        public bool GenerateTunnel(int startX, int startY, int originX, int originY, int horizontalRadius, int verticalRadius) {
            double x = startX;
            double y = startY;
            var direction = new Vector2(1f).RotatedBy(Main.rand.NextFloat(MathHelper.Pi));
            float directionRotation = 0f;
            for (int i = 0; i < 120; i++) {

                if (!WorldGen.InWorld((int)x, (int)y)) {
                    return false;
                }

                var tile = Main.tile[(int)x, (int)y];
                if (TileHelper.ScanTilesSquare((int)x, (int)y, 50, TileHelper.HasImportantTile)) {
                    return i > 10;
                }

                direction.Normalize();
                var v = WorldGen.digTunnel(x, y, direction.X, direction.Y, 3, 6,
                    Wet: false);
                x = v.X;
                y = v.Y;
                if (y < startY) {
                    direction.Y += 1f;
                }
                if (Helper.CircleDistanceInterval(originX, originY, (int)x, (int)y, horizontalRadius, verticalRadius) > 0.66f) {
                    direction = Vector2.Lerp(direction, Vector2.Normalize(new Vector2(originX - (float)x, originY - (float)y)), 0.35f);
                }
                else {
                    direction = direction.RotatedBy(directionRotation);
                }

                if (i % 10 == 0) {
                    directionRotation = WorldGen.genRand.NextFloat(-0.2f, 0.2f);
                }
            }
            return true;
        }
        public bool GenerateFill(int startX, int startY, int originX, int originY, int horizontalRadius, int verticalRadius) {
            double x = startX;
            double y = startY;
            var direction = new Vector2(1f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
            float directionRotation = 0f;
            for (int i = 0; i < 15; i++) {

                if (!WorldGen.InWorld((int)x, (int)y)) {
                    return false;
                }

                var tile = Main.tile[(int)x, (int)y];
                //if (TileHelper.ScanTilesSquare((int)x, (int)y, 50, TileHelper.ProtectedWorldgenTile)) {
                //    return i > 10;
                //}
                if (!TileHelper.ScanTilesSquare((int)x, (int)y, 50, TileHelper.HasTileAction(TileID.Stone))) {
                    return i > 10;
                }

                WorldGen.TileRunner((int)x, (int)y, 10, 10, TileID.Stone, addTile: true, direction.X, direction.Y);
                direction.Normalize();
                x += direction.X * 7f;
                y += direction.Y * 7f;
                if (y < startY) {
                    direction.Y += 1f;
                }
                if (Helper.CircleDistanceInterval(originX, originY, (int)x, (int)y, horizontalRadius, verticalRadius) > 0.66f) {
                    direction = Vector2.Lerp(direction, Vector2.Normalize(new Vector2(originX - (float)x , originY- (float)y)), 0.2f);
                }
                else {
                    direction = direction.RotatedBy(directionRotation);
                }

                if (i % 10 == 0) {
                    directionRotation = WorldGen.genRand.NextFloat(-0.2f, 0.2f);
                }
            }
            return true;
        }
        public void GenerateAetheriumSpike(int startX, int startY) {
            int dir = WorldGen.genRand.NextBool() ? 1 : -1;
            int amtX = WorldGen.genRand.Next(3, 5);
            int lastAmtY = 0;
            int x = startX;
            int y = startY;
            for (int i = 0; i < amtX; i++) {
                int amtY = WorldGen.genRand.Next(4, 10);
                if (Main.tile[x, y].IsFullySolid()) {
                    amtY -= 2;
                }
                if (lastAmtY >= amtY * 2 - 1) {
                    amtY = (int)Math.Ceiling(lastAmtY / 2f) + 1;
                }

                for (int j = 0; j < amtY; j++) {
                    Main.tile[x, y + j].ClearTile();
                    WorldGen.PlaceTile(x, y + j, TileID.ShimmerBlock, mute: true, forced: true);
                }
                x += dir;
                y += amtY / 2;
                lastAmtY = amtY;
            }
        }

        private void FindAether(out int aetherX, out int aetherY) {

            int i = 0;
            int xOffset = Main.dungeonX * 2 < Main.maxTilesX ? Main.maxTilesX / 2 : 0;
            do {
                aetherX = Rand.Next(100, Main.maxTilesX / 2) + xOffset;
                aetherY = Rand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (TileHelper.HasShimmer(aetherX, aetherY)) {
                    return;
                }

                i++;
            }
            while (i < Main.maxTilesX * Main.maxTilesY);
        }
        private void EmitShimmer(int x, int y, int size) {
            TileHelper.ScanTilesSquare(x, y, size, (i, j) => {
                var tile = Main.tile[i, j];
                if (tile.IsFullySolid()) {
                    return false;
                }
                tile.LiquidAmount = byte.MaxValue;
                tile.LiquidType = LiquidID.Shimmer;
                return false;
            });
        }

        protected override void Generate() {

            SetText("WorldGeneration.OmniGem");
            SetProgress(0f);
            FindAether(out int x, out int y);
            int width = 150;
            int height = Main.maxTilesY / 6;

            y += (int)(height * 0.4f);

            int left = x - width / 2;
            int top = y - height / 2;

            y = Math.Min(y, Main.maxTilesY - height - 100);
            var shape = new Shapes.Circle(width, height);
            shape.Perform(new(x, y), new ActionBlockReplace(width, height));

            for (int i = 0; i < 230; i++) {
                int tunnelX = left + Rand.Next(width);
                int tunnelY = y + Rand.Next(height / 2 + 100) - 50;

                SetProgress(i / 200f);
                if (!WorldGen.InWorld(tunnelX, tunnelY, 60)) {
                    continue;
                }
                if (i < 200) {
                    if (!GenerateFill(tunnelX, tunnelY, x, y, width, height)) {
                        continue;
                    }
                }
                else if (!GenerateTunnel(tunnelX, tunnelY, x, y, width, height)) {
                    continue;
                }

                i += 5;

                if (!TileHelper.ScanUp(new(tunnelX, tunnelY), 60, out Point waterfall, TileHelper.SolidType)
                    || Main.tile[waterfall].TileType != TileID.Stone) {
                    continue;
                }

                EmitShimmer(waterfall.X, waterfall.Y, 40);
                GenerateWaterfall(waterfall.X, waterfall.Y);
            }
        }

        private void CheckWaterfallSloping(int x, int y) {
            for (int dir = 1; dir >= -1; dir -= 2) {
                var tile = Main.tile[x + dir, y];
                if (tile.IsFullySolid() && !Main.tile[x + dir * 2, y].IsFullySolid()) {
                    tile.Slope = SlopeType.Solid;
                    tile.IsHalfBlock = true;
                }
            }
        }

        public void FinishAether() {
            int amt = Main.maxTilesY / 4;
            for (int i = 0; i < amt; i++) {
                FindAether(out int x, out int y);

                CheckWaterfallSloping(x, y);

                if (!Rand.NextBool(8)) {
                    continue;
                }

                for (int j = 0; j < 50; j++) {
                    int spikeX = x + Rand.Next(-60, 60);
                    int spikeY = y + Rand.Next(-30, 60);
                    if (!WorldGen.InWorld(spikeX, spikeY) || Main.tile[spikeX, spikeY].IsFullySolid() || !TileHelper.ScanDown(new(spikeX, spikeY), 10, out var spikeCoords)) {
                        continue;
                    }

                    if (TileHelper.ScanTilesSquare(spikeCoords.X, spikeCoords.Y, 10, TileHelper.HasShimmer, TileHelper.HasTileAction(TileID.ShimmerBlock))) {
                        continue;
                    }

                    spikeCoords.Y -= Rand.Next(6);
                    GenerateAetheriumSpike(spikeCoords.X, spikeCoords.Y);
                    i += 10;
                }
            }
        }
    }
}