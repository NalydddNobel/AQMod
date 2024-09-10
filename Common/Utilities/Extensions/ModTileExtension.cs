namespace Aequus.Common.Utilities.Extensions;

public static class ModTileExtension {
    public static void CloneStaticDefaults(this ModTile modTile, int tileId) {
        CopyFromArr(Main.tileSpelunker);
        CopyFromArr(Main.tileContainer);
        CopyFromArr(Main.tileShine2);
        CopyFromArr(Main.tileShine);
        CopyFromArr(Main.tileFrameImportant);
        CopyFromArr(Main.tileNoAttach);
        CopyFromArr(Main.tileOreFinderPriority);
        CopyFromArr(TileID.Sets.HasOutlines);
        CopyFromArr(TileID.Sets.BasicChest);
        CopyFromArr(TileID.Sets.DisableSmartCursor);

        void CopyFromArr<T>(T[] arr) => arr[modTile.Type] = arr[tileId];
    }
}
