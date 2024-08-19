using Aequus.Common.Utilities.Helpers;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings;

public record struct PaintingInitInfo(int Width, int Height, int[]? CoordinateHeightsOverride = null) {
    public void SetupPainting(ModTile painting, TileObjectData obj) {
        int Type = painting.Type;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileSpelunker[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        obj.CopyFrom(TileObjectData.Style3x3Wall);
        obj.Width = Width;
        obj.Height = Height;
        obj.CoordinateHeights = GetCoordinateHeights();
        obj.StyleHorizontal = true;
        obj.StyleWrapLimit = 36;
    }

    public readonly int[] GetCoordinateHeights() {
        return CoordinateHeightsOverride ?? ArrayTools.New(Height, (i) => 16);
    }
}