using Aequus.Core.Initialization;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Aequus.Common.Tiles;

public abstract class MultiMergeTile : ModTile {
    public const int Right = 0;
    public const int Left = 1;
    public const int Bottom = 2;
    public const int Top = 3;
    public const int BottomRight = 4;
    public const int BottomLeft = 5;
    public const int TopRight = 6;
    public const int TopLeft = 7;

    private record struct MergeRules(bool MergeWithHalfBlocks = false, bool MergeLeftSlope = false, bool MergeRightSlope = false, bool MergeTopSlope = false, bool MergeBottomSlope = false);

    private interface IMergeFrame {
        bool CanRender { get; }
        Rectangle GetFrame(FastRandom random);
    }
    private readonly record struct CornerMergeFrame(int X, int Y) : IMergeFrame {
        public bool CanRender => true;

        public Rectangle GetFrame(FastRandom random) {
            return new(X * 18, (Y + random.Next(3) * 2) * 18, 18, 18);
        }
    }
    private readonly record struct MergeFrame(int MinX, int MaxX, int MinY, int MaxY) : IMergeFrame {
        public bool CanRender => true;

        public Rectangle GetFrame(FastRandom random) {
            return new(random.Next(MinX, MaxX + 1) * 18, random.Next(MinY, MaxY + 1) * 18, 18, 18);
        }
    }
    private readonly record struct InvisibleFrame() : IMergeFrame {
        public bool CanRender => false;

        public Rectangle GetFrame(FastRandom random) {
            return default;
        }
    }

    private static readonly MergeRules[] _mergeRules = new MergeRules[] {
        new MergeRules(MergeRightSlope: true),
        new MergeRules(MergeLeftSlope: true),
        new MergeRules(MergeBottomSlope: true),
        new MergeRules(MergeWithHalfBlocks: true, MergeTopSlope: true),
        new MergeRules(MergeBottomSlope: true, MergeRightSlope: true),
        new MergeRules(MergeBottomSlope: true, MergeLeftSlope: true),
        new MergeRules(MergeTopSlope: true, MergeRightSlope: true),
        new MergeRules(MergeTopSlope: true, MergeLeftSlope: true),
    };
    private static readonly int[] _opposingMerges = new int[] {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    };
    private static readonly IMergeFrame[] _frameLookups;

    private static byte[][] _mergeCache;
    private static Asset<Texture2D>[] _mergeTextures = Array.Empty<Asset<Texture2D>>();
    private const string Path = "Aequus/Assets/Textures/TileMerges/";

    public List<int> Merges { get; private set; } = new();

    static MultiMergeTile() {
        _frameLookups = new IMergeFrame[byte.MaxValue + 1];
        for (int b = 0; b <= byte.MaxValue; b++) {
            GetMergeFlags((byte)b, out bool right, out bool left, out bool bottom, out bool top, out bool bottomRight, out bool bottomLeft, out bool topRight, out bool topLeft);
            _frameLookups[b] = GetMergeFrame(right, left, bottom, top, bottomRight, bottomLeft, topRight, topLeft);
        }
    }

    public void AddMerge(int with) {
        Main.tileMerge[Type][with] = true;
        Main.tileMerge[with][Type] = true;

        Merges.Add(with);

        if (Main.netMode != NetmodeID.Server) {
            LoadingSteps.EnqueuePostSetupContent(() => {
                if (_mergeTextures.Length <= with) {
                    Array.Resize(ref _mergeTextures, with + 1);
                }
                _mergeTextures[with] ??= ModContent.Request<Texture2D>($"{Path}{TileID.Search.GetName(with).Replace("Aequus/", "")}");
            });
        }
    }

    #region Merging
    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        if (!WorldGen.InWorld(i, j, 20)) {
            return false;
        }
        var tile = Main.tile[i, j];
        var right = Main.tile[i + 1, j];
        var left = Main.tile[i - 1, j];
        var bottom = Main.tile[i, j + 1];
        var top = Main.tile[i, j - 1];
        var bottomRight = Main.tile[i + 1, j + 1];
        var bottomLeft = Main.tile[i - 1, j + 1];
        var topRight = Main.tile[i + 1, j - 1];
        var topLeft = Main.tile[i - 1, j - 1];

        try {
            for (int k = 0; k < Merges.Count; k++) {
                EnsureCacheLength(Merges[k]);
                SetMergeInfo(i, j, Merges[k], 0);
            }
            Merge(in tile, in right, Right, i, j);
            Merge(in tile, in left, Left, i, j);
            Merge(in tile, in bottom, Bottom, i, j);
            Merge(in tile, in top, Top, i, j);
            if (MergesWith(in tile, bottom)) {
                if (MergesWith(in tile, right)) {
                    MergeCorner(in tile, in bottomRight, BottomRight, i, j);
                }
                if (MergesWith(in tile, left)) {
                    MergeCorner(in tile, in bottomLeft, BottomLeft, i, j);
                }
            }
            if (MergesWith(in tile, top)) {
                if (MergesWith(in tile, right)) {
                    MergeCorner(in tile, in topRight, TopRight, i, j);
                }
                if (MergesWith(in tile, left)) {
                    MergeCorner(in tile, in topLeft, TopLeft, i, j);
                }
            }
        }
        catch (Exception ex) {
        }
        return true;
    }

    private bool MergesWith(in Tile tile, in Tile other) {
        return other.HasTile && Main.tileMerge[tile.TileType][other.TileType];
    }

    private void MergeCorner(in Tile tile, in Tile other, int index, int i, int j) {
        Merge(in tile, in other, index, i, j);
    }

    private void Merge(in Tile tile, in Tile other, int index, int i, int j) {
        if (Merges.Contains(other.TileType)) {
            SetMergeInfo(i, j, other.TileType, index, MergeInner(in tile, in other, index, i, j));
        }
    }

    private bool MergeInner(in Tile tile, in Tile other, int index, int i, int j) {
        // Prevent merge if the tiles do not merge in the first place
        // Also prevent merge if both tiles are sloped.
        if (!other.HasTile || !Main.tileMerge[Type][other.TileType] || (tile.Slope != 0 && other.Slope != 0)) {
            return false;
        }

        var rules = _mergeRules[index];
        // Only blocks on the bottom can merge safely with half blocks above them
        if (!rules.MergeWithHalfBlocks && other.IsHalfBlock) {
            return false;
        }

        // Check if both slopes are valid for merging
        // 
        // ________          _______  
        //         |        ╱
        //         |      ╱
        //         |    ╱  
        //         |  ╱ 
        //
        // It'd be dumb for these to merge properly

        if (!GetSlopeStatus(other, index) || !GetSlopeStatus(tile, _opposingMerges[index])) {
            return false;
        }

        return true;
    }

    private static bool GetSlopeStatus(in Tile tile, int index) {
        var rules = _mergeRules[index];
        return (tile.Slope == SlopeType.Solid) || (tile.LeftSlope && rules.MergeLeftSlope) || (tile.RightSlope && rules.MergeRightSlope) || (tile.BottomSlope && rules.MergeBottomSlope) || (tile.TopSlope && rules.MergeTopSlope);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetMergeInfo(int i, int j, int type) {
        return _mergeCache[type][i + j * Main.maxTilesX];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte SetMergeInfo(int i, int j, int type, byte val) {
        return _mergeCache[type][i + j * Main.maxTilesX] = val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetMergeInfo(int i, int j, int type, int byteOffset, bool value) {
        SetMergeInfo(i, j, type, (byte)TileDataPacking.SetBit(value, _mergeCache[type][i + j * Main.maxTilesX], byteOffset));
    }

    public static void GetMergeFlags(byte mergeData, out bool right, out bool left, out bool bottom, out bool top, out bool bottomRight, out bool bottomLeft, out bool topRight, out bool topLeft) {
        right = TileDataPacking.GetBit(mergeData, Right);
        left = TileDataPacking.GetBit(mergeData, Left);
        bottom = TileDataPacking.GetBit(mergeData, Bottom);
        top = TileDataPacking.GetBit(mergeData, Top);
        bottomRight = TileDataPacking.GetBit(mergeData, BottomRight);
        bottomLeft = TileDataPacking.GetBit(mergeData, BottomLeft);
        topRight = TileDataPacking.GetBit(mergeData, TopRight);
        topLeft = TileDataPacking.GetBit(mergeData, TopLeft);
    }

    private static IMergeFrame GetMergeFrame(bool right, bool left, bool bottom, bool top, bool bottomRight, bool bottomLeft, bool topRight, bool topLeft) {
        if (right && left && bottom && top) {
            return new MergeFrame(6, 8, 11, 11);
        }

        if (right && left && bottom) {
            return new MergeFrame(11, 11, 8, 10);
        }
        if (right && left && top) {
            return new MergeFrame(11, 11, 5, 7);
        }
        if (right && bottom && top) {
            return new MergeFrame(12, 12, 8, 10);
        }
        if (left && bottom && top) {
            return new MergeFrame(12, 12, 5, 7);
        }

        if (left && right) {
            return new MergeFrame(10, 10, 7, 9);
        }
        if (bottom && top) {
            return new MergeFrame(8, 10, 10, 10);
        }

        if (bottom && right) {
            return new CornerMergeFrame(3, 6);
        }
        if (bottom && left) {
            return new CornerMergeFrame(2, 6);
        }
        if (top && right) {
            return new CornerMergeFrame(3, 5);
        }
        if (top && left) {
            return new CornerMergeFrame(2, 5);
        }

        if (right) {
            return new MergeFrame(3, 5, 11, 14);
        }
        if (left) {
            return new MergeFrame(0, 2, 11, 14);
        }
        if (bottom) {
            return new MergeFrame(4, 7, 5, 7);
        }
        if (top) {
            return new MergeFrame(4, 7, 8, 10);
        }

        if (bottomRight) {
            return new CornerMergeFrame(0, 5);
        }
        if (bottomLeft) {
            return new CornerMergeFrame(1, 5);
        }
        if (topRight) {
            return new CornerMergeFrame(0, 6);
        }
        if (topLeft) {
            return new CornerMergeFrame(1, 6);
        }
        return new InvisibleFrame();
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureCacheLength() {
        _mergeCache ??= Array.Empty<byte[]>();
        if (_mergeCache.Length != TileLoader.TileCount) {
            EnumerableHelper.ResizeAndPopulate(ref _mergeCache, TileLoader.TileCount, () => Array.Empty<byte>());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureCacheLength(int type) {
        int length = Main.maxTilesX * Main.maxTilesY;
        EnsureCacheLength();
        if (_mergeCache[type].Length != length) {
            Array.Resize(ref _mergeCache[type], length);
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        var lighting = LightHelper.GetLightingSection(i, j, 3);
        for (int k = 0; k < Merges.Count; k++) {
            var frame = _frameLookups[GetMergeInfo(i, j, Merges[k])];
#if DEBUG
            try {
#endif
                if (frame.CanRender) {
                    spriteBatch.Draw(_mergeTextures[Merges[k]].Value, new Vector2(i, j) * 16f - Main.screenPosition + TileHelper.DrawOffset, frame.GetFrame(Helper.RandomTileCoordinates(i, j)), lighting, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
#if DEBUG
            }
            catch (Exception ex) {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, frame?.ToString() ?? "null", new Vector2(i, j) * 16f - Main.screenPosition, Main.errorColor, 0f, Vector2.Zero, Vector2.One);
            }
#endif
        }
    }
}