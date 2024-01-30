using Aequus.Common.Tiles;
using System;
using System.Runtime.CompilerServices;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Core.Utilities;

public static class TileHelper {
    public static Vector2 DrawOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

    public static Single GetWaterY(Byte liquidAmount) {
        return (1f - liquidAmount / 255f) * 16f;
    }

    public static Int32 GetTileDust(Int32 sampleX, Int32 sampleY, Int32 tileType, Int32 tileStyle) {
        lock (Main.instance.TilesRenderer) {

            var sampleTile = Main.tile[sampleX, sampleY];
            var oldActive = sampleTile.HasTile;
            var oldType = sampleTile.TileType;
            var oldFrameX = sampleTile.TileFrameX;
            var oldFrameY = sampleTile.TileFrameY;

            sampleTile.TileType = (UInt16)tileType;

            Int32 dust = -1;
            try {
                if (Main.tileFrameImportant[tileType]) {
                    var tileObjectData = TileObjectData.GetTileData(tileType, tileStyle);
                    if (tileObjectData != null) {
                        Int32 style = tileObjectData.StyleMultiplier * tileStyle;
                        Int32 wrapLimit = tileObjectData.StyleWrapLimit;
                        if (wrapLimit <= 0) {
                            wrapLimit = Int32.MaxValue;
                        }

                        Int32 styleX, styleY;
                        if (tileObjectData.StyleHorizontal) {
                            styleX = style % wrapLimit;
                            styleY = style / wrapLimit;
                        }
                        else {
                            styleX = style / wrapLimit;
                            styleY = style % wrapLimit;
                        }

                        sampleTile.TileFrameX = (Int16)(styleX * tileObjectData.CoordinateFullWidth);
                        sampleTile.TileFrameY = (Int16)(styleY * tileObjectData.CoordinateFullHeight);
                    }
                }

                dust = WorldGen.KillTile_MakeTileDust(sampleX, sampleY, sampleTile);
            }
            catch {
            }

            sampleTile.TileFrameX = oldFrameX;
            sampleTile.TileFrameY = oldFrameY;
            sampleTile.TileType = oldType;
            sampleTile.HasTile = oldActive;

            return dust;
        }
    }

    public static Int32 GetStyle(Int32 i, Int32 j, Int32 coordinateFullWidthBackup = 18) {
        var tile = Main.tile[i, j];
        return GetStyle(tile, TileObjectData.GetTileData(tile), coordinateFullWidthBackup);
    }

    public static Int32 GetStyle(Tile tile, Int32 coordinateFullWidthBackup = 18) {
        return GetStyle(tile, TileObjectData.GetTileData(tile), coordinateFullWidthBackup);
    }

    public static Int32 GetStyle(Tile tile, TileObjectData tileObjectData, Int32 coordinateFullWidthBackup = 18) {
        return tileObjectData != null ? tile.TileFrameX / tileObjectData.CoordinateFullWidth : tile.TileFrameX / coordinateFullWidthBackup;
    }

    public static Boolean IsShimmerBelow(Point tileCoordinates, Int32 distance = 1) {
        for (Int32 y = tileCoordinates.Y; y < tileCoordinates.Y + distance; y++) {
            if (!WorldGen.InWorld(tileCoordinates.X, y, 40) || Main.tile[tileCoordinates.X, y].IsFullySolid()) {
                return false;
            }
            if (Main.tile[tileCoordinates.X, y].LiquidAmount > 0 && Main.tile[tileCoordinates.X, y].LiquidType == LiquidID.Shimmer) {
                return true;
            }
        }
        return false;
    }

    public static Boolean IsSolidTileAnchor(this TileAnchorDirection tileAnchor) {
        return tileAnchor != TileAnchorDirection.Invalid && tileAnchor != TileAnchorDirection.Wall;
    }

    public static TileAnchorDirection GetGemFramingAnchor(Int32 i, Int32 j, Boolean[] invalidTiles = null) {
        var tile = Framing.GetTileSafely(i, j - 1);
        if (tile.HasTile && !tile.BottomSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Top;
        }

        tile = Framing.GetTileSafely(i, j + 1);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.TopSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])) {
            return TileAnchorDirection.Bottom;
        }

        tile = Framing.GetTileSafely(i - 1, j);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.RightSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Left;
        }

        tile = Framing.GetTileSafely(i + 1, j);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.LeftSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Right;
        }

        return TileAnchorDirection.Invalid;
    }

    public static void GemFraming(Int32 i, Int32 j, Boolean[] invalidTiles = null) {
        var tile = Framing.GetTileSafely(i, j);
        var obj = TileObjectData.GetTileData(tile.TileType, 0);
        Int32 coordinateFullHeight = obj?.CoordinateFullHeight ?? 18;
        switch (GetGemFramingAnchor(i, j, invalidTiles)) {
            case TileAnchorDirection.Bottom: {
                    tile.TileFrameY = (Int16)(WorldGen.genRand.Next(0, 3) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Top: {
                    tile.TileFrameY = (Int16)(WorldGen.genRand.Next(3, 6) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Left: {
                    tile.TileFrameY = (Int16)(WorldGen.genRand.Next(6, 9) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Right: {
                    tile.TileFrameY = (Int16)(WorldGen.genRand.Next(9, 12) * coordinateFullHeight);
                }
                break;
            default:
                WorldGen.KillTile(i, j);
                break;
        }
    }

    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, Boolean[] tileCutIgnore) {
        Int32 left = Math.Max((Int32)(box.X / 16f), 1);
        Int32 right = Math.Min((Int32)(left + box.Width / 16f) + 1, Main.maxTilesX);
        Int32 top = Math.Max((Int32)(box.Y / 16f), 0);
        Int32 bottom = Math.Min((Int32)(top + box.Height / 16f) + 1, Main.maxTilesY);
        for (Int32 i = left; i < right; i++) {
            for (Int32 j = top; j < bottom; j++) {
                if (Main.tile[i, j] != null && Main.tileCut[Main.tile[i, j].TileType] && !tileCutIgnore[Main.tile[i, j].TileType] && WorldGen.CanCutTile(i, j, context)) {
                    WorldGen.KillTile(i, j);
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                    }
                }
            }
        }
    }

    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, Player player, Boolean fromTrap = false) {
        CutTilesRectangle(box, context, player.GetTileCutIgnorance(allowRegrowth: false, fromTrap));
    }

    public static void SetMerge<T>(this ModTile modTile) where T : ModTile {
        modTile.SetMerge(ModContent.TileType<T>());
    }
    public static void SetMerge(this ModTile modTile, Int32 otherType) {
        SetMerge(modTile.Type, otherType);
    }
    public static void SetMerge(Int32 myType, Int32 otherType) {
        Main.tileMerge[myType][otherType] = true;
        Main.tileMerge[otherType][myType] = true;
    }

    /// <summary>
    /// Properly removes a liquid at the given tile coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="quiet"></param>
    public static void KillLiquid(Int32 x, Int32 y, Boolean quiet = false) {
        var tile = Main.tile[x, y];
        tile.LiquidType = 0;
        tile.LiquidAmount = 0;
        WorldGen.SquareTileFrame(x, y, resetFrame: false);
        if (Main.netMode == NetmodeID.MultiplayerClient && !quiet) {
            NetMessage.sendWater(x, y);
        }
        else {
            Liquid.AddWater(x, y);
        }
    }

    public static Boolean ScanDown(Point p, Int32 limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
        Int32 endY = Math.Min(p.Y + limit, Main.maxTilesY - 36);
        result = p;
        for (Int32 j = p.Y; j < endY; j++) {
            for (Int32 k = 0; k < tileActionAttempt.Length; k++) {
                if (tileActionAttempt[k](p.X, j)) {
                    result.Y = j;
                    return true;
                }
            }
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean ScanDown(Point p, Int32 limit, out Point result) {
        return ScanDown(p, limit, out result, IsSolid);
    }

    public static Boolean ScanUp(Point p, Int32 limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
        Int32 endY = Math.Max(p.Y - limit, 36);
        result = p;
        for (Int32 j = p.Y; j > endY; j--) {
            for (Int32 k = 0; k < tileActionAttempt.Length; k++) {
                if (tileActionAttempt[k](p.X, j)) {
                    result.Y = j;
                    return true;
                }
            }
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean ScanUp(Point p, Int32 limit, out Point result) {
        return ScanUp(p, limit, out result, IsFullySolid);
    }

    public static Utils.TileActionAttempt HasWallAction(Int32 type) {
        return (i, j) => Main.tile[i, j].WallType == type;
    }
    public static Utils.TileActionAttempt HasWallAction(params Int32[] types) {
        return (i, j) => types.Match(Main.tile[i, j].WallType);
    }
    public static Utils.TileActionAttempt HasTileAction(Int32 type) {
        return (i, j) => Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type;
    }
    public static Utils.TileActionAttempt HasTileAction(params Int32[] types) {
        return (i, j) => Main.tile[i, j].HasTile && types.Match(Main.tile[i, j].TileType);
    }

    public static Boolean HasNoTileAndNoWall(Tile tile) {
        return !tile.HasTile && tile.WallType == WallID.None;
    }
    public static Boolean HasNoTileAndNoWall(Int32 i, Int32 j) {
        return HasNoTileAndNoWall(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasTile(Tile tile) {
        return tile.HasTile;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasTile(Int32 i, Int32 j) {
        return HasTile(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean CuttableOrNoTile(this Tile tile) {
        return !tile.HasTile || Main.tileCut[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean CuttableOrNoTile(Int32 i, Int32 j) {
        return Main.tile[i, j].CuttableOrNoTile();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean CuttableType(this Tile tile) {
        return Main.tileCut[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean CuttableType(Int32 i, Int32 j) {
        return Main.tile[i, j].CuttableType();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean SolidType(this Tile tile) {
        return Main.tileSolid[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean SolidType(Int32 i, Int32 j) {
        return Main.tile[i, j].SolidType();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean SolidTopType(this Tile tile) {
        return Main.tileSolidTop[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean SolidTopType(Int32 i, Int32 j) {
        return Main.tile[i, j].SolidTopType();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsSolid(this Tile tile) {
        return tile.HasUnactuatedTile && tile.SolidType();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsSolid(Int32 i, Int32 j) {
        return Main.tile[i, j].IsSolid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsFullySolid(this Tile tile) {
        return tile.IsSolid() && !tile.SolidTopType();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsFullySolid(Int32 i, Int32 j) {
        return Main.tile[i, j].IsFullySolid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsNotSolid(this Tile tile) {
        return !tile.IsSolid();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsNotSolid(Int32 i, Int32 j) {
        return Main.tile[i, j].IsNotSolid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasNoLiquid(this Tile tile) {
        return tile.LiquidAmount == 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasNoLiquid(Int32 i, Int32 j) {
        return Main.tile[i, j].HasNoLiquid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasAnyLiquid(this Tile tile) {
        return tile.LiquidAmount > 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasAnyLiquid(Int32 i, Int32 j) {
        return Main.tile[i, j].HasAnyLiquid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasShimmer(this Tile tile) {
        return tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasShimmer(Int32 i, Int32 j) {
        return Main.tile[i, j].HasShimmer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasMinecartRail(this Tile tile) {
        return tile.HasTile && tile.TileType == TileID.MinecartTrack;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasMinecartRail(Int32 i, Int32 j) {
        return Main.tile[i, j].HasMinecartRail();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasContainer(this Tile tile) {
        return tile.HasTile && TileID.Sets.IsAContainer[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasContainer(Int32 i, Int32 j) {
        return Main.tile[i, j].HasContainer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean IsTree(Int32 i, Int32 j) {
        return Main.tile[i, j].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[i, j].TileType];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean ScanTilesSquare(Int32 i, Int32 j, Int32 size, params Utils.TileActionAttempt[] tileActionAttempt) {
        return ScanTiles(new(i - size / 2, j - size / 2, size, size), tileActionAttempt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasImportantTile(this Tile tile) {
        return !TileID.Sets.GeneralPlacementTiles[tile.TileType] || Main.wallDungeon[tile.WallType] || tile.WallType == WallID.LihzahrdBrickUnsafe;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasImportantTile(Int32 i, Int32 j) {
        return Main.tile[i, j].HasImportantTile();
    }

    public static Boolean ScanTiles(Rectangle rect, params Utils.TileActionAttempt[] tileActionAttempt) {
        rect = rect.Fluffize();
        foreach (var attempt in tileActionAttempt) {
            for (Int32 i = rect.X; i < rect.X + rect.Width; i++) {
                for (Int32 j = rect.Y; j < rect.Y + rect.Height; j++) {
                    if (attempt(i, j)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static Rectangle Fluffize(this Rectangle rectangle, Int32 padding = 0) {
        rectangle.X = Math.Clamp(rectangle.X, padding, Main.maxTilesX - rectangle.Width - padding);
        rectangle.Y = Math.Clamp(rectangle.Y, padding, Main.maxTilesY - rectangle.Height - padding);
        return rectangle;
    }

    public static Boolean IsInvisible(this Tile tile) {
        return !Main.LocalPlayer.CanSeeInvisibleBlocks && !Main.SceneMetrics.EchoMonolith && tile.BlockColorAndCoating().Invisible;
    }
    public static Boolean IsInvisible(Int32 x, Int32 y) {
        return Main.tile[x, y].IsInvisible();
    }

    public static Boolean IsSectionLoaded(Int32 tileX, Int32 tileY) {
        if (Main.netMode == NetmodeID.SinglePlayer || Main.sectionManager == null) {
            return true;
        }
        return Main.sectionManager.SectionLoaded(Netplay.GetSectionX(tileX), Netplay.GetSectionY(tileY));
    }
    public static Boolean IsSectionLoaded(Point p) {
        return IsSectionLoaded(p.X, p.Y);
    }

    #region Growth
    public static Boolean TryGrowGrass(Int32 x, Int32 y, Int32 tileID) {
        for (Int32 k = -1; k <= 1; k++) {
            for (Int32 l = -1; l <= 1; l++) {
                if (!Main.tile[x + k, y + l].IsFullySolid()) {
                    Main.tile[x, y].TileType = (UInt16)tileID;
                    return true;
                }
            }
        }
        return false;
    }
    public static void SpreadGrass(Int32 i, Int32 j, Int32 dirt, Int32 grass, Int32 spread = 0, Byte color = 0) {
        if (!WorldGen.InWorld(i, j, 6)) {
            return;
        }
        for (Int32 k = i - 1; k <= i + 1; k++) {
            for (Int32 l = j - 1; l <= j + 1; l++) {
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
    #endregion
}
