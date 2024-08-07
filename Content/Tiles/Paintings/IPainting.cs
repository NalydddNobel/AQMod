using System.Collections.Generic;
using Terraria.GameContent.Generation;

namespace Aequus.Content.Tiles.Paintings;

public interface IPainting {
    ushort TileType { get; }
    int ItemType { get; }
}

public static class PaintingExtensions {
    public static PaintingEntry GetEntry(this IPainting painting) {
        return new PaintingEntry() { tileType = painting.TileType, style = 0 };
    }

    public static IPainting AddEntry(this IPainting painting, List<PaintingEntry> list) {
        list.Add(painting.GetEntry());
        return painting;
    }
}
