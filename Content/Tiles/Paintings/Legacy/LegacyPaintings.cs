using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Utilities.Helpers;
using System.Collections.Generic;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings.Legacy;

/// <summary>Handles loading and converting legacy paintings.</summary>
internal class LegacyPaintings(int Width, int Height) : InstancedTile($"WallPaintings{Width}x{Height}", string.Empty) {
    private int _frameWidth;

    public override string Texture => AequusTextures.None.FullPath;

    private static readonly Dictionary<Point, LegacyPaintings> _dimsToLegacy = [];
    private readonly Dictionary<int, ushort> Convert = [];

    public static void RegisterLegacy(Mod mod, int Width, int Height, int Style, ushort ToType) {
        Point key = new Point(Width, Height);
        LegacyPaintings? legacy;

        if (!_dimsToLegacy.TryGetValue(key, out legacy)) {
            legacy = new LegacyPaintings(Width, Height);
            mod.AddContent(legacy);
            _dimsToLegacy.Add(key, legacy);
        }

        legacy.Convert.Add(Style, ToType);
    }

    public sealed override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileSpelunker[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Width = Width;
        TileObjectData.newTile.Height = Height;
        TileObjectData.newTile.CoordinateHeights = ArrayTools.New(Height, (i) => 16);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;

        _frameWidth = TileObjectData.newTile.CoordinateFullWidth;

        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        AddMapEntry(new Color(120, 85, 60));

    }

    public sealed override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        int style = tile.TileFrameX / _frameWidth;

        if (Convert.TryGetValue(style, out ushort convertType)) {
            tile.TileType = convertType;
            tile.TileFrameX %= (short)_frameWidth;
            NetMessage.SendTileSquare(-1, i, j);
        }
        else {
            WorldGen.KillTile(i, j);
        }

        return base.PreDraw(i, j, spriteBatch);
    }
}
