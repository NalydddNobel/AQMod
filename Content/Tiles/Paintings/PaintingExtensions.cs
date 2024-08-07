using Aequus.Content.Tiles.Paintings.Legacy;
using System.Collections.Generic;
using Terraria.GameContent.Generation;

namespace Aequus.Content.Tiles.Paintings;

public static class PaintingExtensions {
    public static PaintingEntry GetEntry(this IPainting painting) {
        return new PaintingEntry() { tileType = painting.TileType, style = 0 };
    }

    public static IPainting AddEntry(this IPainting painting, List<PaintingEntry> list) {
        list.Add(painting.GetEntry());
        return painting;
    }

    public static IPainting RegisterLegacy<T>(this IPainting painting, int style) where T : LegacyPaintingTile {
        ModContent.GetInstance<T>().Convert.Add(style, painting.TileType);
        return painting;
    }
}
