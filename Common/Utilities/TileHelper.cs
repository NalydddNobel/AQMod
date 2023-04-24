using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Aequus {
    public static class TileHelper {
        public class Frames {
            public static void GemFraming(int i, int j, params int[] validTiles) {
                var tile = Framing.GetTileSafely(i, j);
                var top = Main.tile[i, j - 1];
                var bottom = Framing.GetTileSafely(i, j + 1);
                var left = Main.tile[i - 1, j];
                var right = Main.tile[i + 1, j];
                var obj = TileObjectData.GetTileData(Main.tile[i, j].TileType, 0);
                int coordinateFullHeight = obj?.CoordinateFullHeight ?? 18;
                if (top != null && top.HasTile && !top.BottomSlope && top.TileType >= 0 && validTiles.ContainsAny(top.TileType) && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType]) {
                    if (tile.TileFrameY < 54 || tile.TileFrameY > 90) {
                        tile.TileFrameY = (short)(coordinateFullHeight * 3 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                    }
                    return;
                }
                if (bottom != null && bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && validTiles.ContainsAny(bottom.TileType) && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType])) {
                    if (tile.TileFrameY < 0 || tile.TileFrameY > 36) {
                        tile.TileFrameY = (short)(WorldGen.genRand.Next(3) * coordinateFullHeight);
                    }
                    return;
                }
                if (left != null && left.HasTile && left.TileType >= 0 && validTiles.ContainsAny(left.TileType) && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType]) {
                    if (tile.TileFrameY < 108 || tile.TileFrameY > 54) {
                        tile.TileFrameY = (short)(coordinateFullHeight * 6 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                    }
                    return;
                }
                if (right != null && right.HasTile && right.TileType >= 0 && validTiles.ContainsAny(right.TileType) && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType]) {
                    if (tile.TileFrameY < 162 || tile.TileFrameY > 198) {
                        tile.TileFrameY = (short)(coordinateFullHeight * 9 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                    }
                    return;
                }
                WorldGen.KillTile(i, j);
            }
            public static void GemFraming(int i, int j) {
                var tile = Framing.GetTileSafely(i, j);
                var top = Main.tile[i, j - 1];
                var bottom = Framing.GetTileSafely(i, j + 1);
                var left = Main.tile[i - 1, j];
                var right = Main.tile[i + 1, j];
                if (top != null && top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType]) {
                    if (tile.TileFrameY < 54 || tile.TileFrameY > 90) {
                        tile.TileFrameY = (short)(54 + WorldGen.genRand.Next(3) * 18);
                    }
                    return;
                }
                if (bottom != null && bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType])) {
                    if (tile.TileFrameY < 0 || tile.TileFrameY > 36) {
                        tile.TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);
                    }
                    return;
                }
                if (left != null && left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType]) {
                    if (tile.TileFrameY < 108 || tile.TileFrameY > 54) {
                        tile.TileFrameY = (short)(108 + WorldGen.genRand.Next(3) * 18);
                    }
                    return;
                }
                if (right != null && right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType]) {
                    if (tile.TileFrameY < 162 || tile.TileFrameY > 198) {
                        tile.TileFrameY = (short)(162 + WorldGen.genRand.Next(3) * 18);
                    }
                    return;
                }
                WorldGen.KillTile(i, j);
            }
        }

        public static bool ScanDown(Point p, int limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
            int endY = Math.Min(p.Y + limit, Main.maxTilesY - 36);
            result = p;
            for (int j = p.Y; j < endY; j++) {
                for (int k = 0; k < tileActionAttempt.Length; k++) {
                    if (tileActionAttempt[k](p.X, j)) {
                        result.Y = j;
                        return true;
                    }
                }
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ScanDown(Point p, int limit, out Point result) {
            return ScanDown(p, limit, out result, IsSolid);
        }

        public static bool ScanUp(Point p, int limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
            int endY = Math.Max(p.Y - limit, 36);
            result = p;
            for (int j = p.Y; j > endY; j--) {
                for (int k = 0; k < tileActionAttempt.Length; k++) {
                    if (tileActionAttempt[k](p.X, j)) {
                        result.Y = j;
                        return true;
                    }
                }
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ScanUp(Point p, int limit, out Point result) {
            return ScanUp(p, limit, out result, IsFullySolid);
        }

        public static Utils.TileActionAttempt HasWallAction(int type) {
            return (i, j) => Main.tile[i, j].WallType == type;
        }
        public static Utils.TileActionAttempt HasWallAction(params int[] types) {
            return (i, j) => types.ContainsAny(Main.tile[i, j].WallType);
        }
        public static Utils.TileActionAttempt HasTileAction(int type) {
            return (i, j) => Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type;
        }
        public static Utils.TileActionAttempt HasTileAction(params int[] types) {
            return (i, j) => Main.tile[i, j].HasTile && types.ContainsAny(Main.tile[i, j].TileType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SolidType(this Tile tile) {
            return Main.tileSolid[tile.TileType];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SolidType(int i, int j) {
            return SolidType(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SolidTopType(this Tile tile) {
            return Main.tileSolidTop[tile.TileType];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SolidTopType(int i, int j) {
            return SolidTopType(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSolid(this Tile tile) {
            return tile.HasTile && SolidType(tile) && !tile.IsActuated;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSolid(int i, int j) {
            return IsSolid(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFullySolid(this Tile tile) {
            return IsSolid(tile) && !SolidTopType(tile);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFullySolid(int i, int j) {
            return IsFullySolid(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyLiquid(Tile tile) {
            return tile.LiquidAmount > 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyLiquid(int i, int j) {
            return HasAnyLiquid(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasShimmer(Tile tile) {
            return tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasShimmer(int i, int j) {
            return HasShimmer(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMinecartRail(Tile tile) {
            return tile.HasTile && tile.TileType == TileID.MinecartTrack;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasMinecartRail(int i, int j) {
            return HasMinecartRail(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasContainer(Tile tile) {
            return tile.HasTile && TileID.Sets.IsAContainer[tile.TileType];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasContainer(int i, int j) {
            return HasContainer(Main.tile[i, j]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTree(int i, int j) {
            return Main.tile[i, j].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[i, j].TileType];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ScanTilesSquare(int i, int j, int size, params Utils.TileActionAttempt[] tileActionAttempt) {
            return ScanTiles(new(i - size / 2, j - size / 2, size, size), tileActionAttempt);
        }

        public static bool ScanTiles(Rectangle rect, params Utils.TileActionAttempt[] tileActionAttempt) {
            rect = rect.Fluffize();
            foreach (var attempt in tileActionAttempt) {
                for (int i = rect.X; i < rect.X + rect.Width; i++) {
                    for (int j = rect.Y; j < rect.Y + rect.Height; j++) {
                        if (attempt(i, j)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public static bool IsSectionLoaded(int tileX, int tileY) {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.sectionManager == null) {
                return true;
            }
            return Main.sectionManager.SectionLoaded(Netplay.GetSectionX(tileX), Netplay.GetSectionY(tileY));
        }
        public static bool IsSectionLoaded(Point p) {
            return IsSectionLoaded(p.X, p.Y);
        }

        public static bool TryGrowGrass(int x, int y, int tileID) {
            for (int k = -1; k <= 1; k++) {
                for (int l = -1; l <= 1; l++) {
                    if (!Main.tile[x + k, y + l].IsFullySolid()) {
                        Main.tile[x, y].TileType = (ushort)tileID;
                        return true;
                    }
                }
            }
            return false;
        }
        public static void SpreadGrass(int i, int j, int dirt, int grass, int spread = 0, byte color = 0) {
            if (!WorldGen.InWorld(i, j, 6)) {
                return;
            }
            for (int k = i - 1; k <= i + 1; k++) {
                for (int l = j - 1; l <= j + 1; l++) {
                    if (WorldGen.genRand.NextBool(8)) {
                        if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == dirt) {
                            if (TryGrowGrass(k, l, grass))
                                WorldGen.SquareTileFrame(k, l, resetFrame: true);
                            return;
                        }
                    }
                }
            }
        }
    }
}