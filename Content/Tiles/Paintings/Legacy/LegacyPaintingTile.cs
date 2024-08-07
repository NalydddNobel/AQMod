using Aequus.Common.Utilities.Helpers;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings.Legacy;
public abstract class LegacyPaintingTile(int Width, int Height) : ModTile {
    public abstract ushort[] ConvertIds();

    private int _frameWidth;
    private int _frameHeight;
    private ushort[]? _convert;

    public override string Texture => AequusTextures.None.FullPath;

    public sealed override void SetStaticDefaults() {
        _convert = ConvertIds();
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
        _frameHeight = TileObjectData.newTile.CoordinateFullHeight;

        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        AddMapEntry(new Color(120, 85, 60));

    }

    public sealed override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];

        int style = tile.TileFrameX / _frameWidth;

        if (_convert.IndexInRange(style)) {
            tile.TileType = _convert![style];
            tile.TileFrameX %= (short)_frameWidth;
            NetMessage.SendTileSquare(-1, i, j);
        }
        else {
            WorldGen.KillTile(i, j);
        }

        return base.PreDraw(i, j, spriteBatch);
    }
}
