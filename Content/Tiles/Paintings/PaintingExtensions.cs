using Aequus.Content.Tiles.Paintings.Legacy;
using System.Collections.Generic;
using Terraria.GameContent.Generation;

namespace Aequus.Content.Tiles.Paintings;

public static class PaintingExtensions {
    public static PaintingEntry ToEntry(this IPainting painting) {
        return new PaintingEntry() { tileType = painting.TileType, style = 0 };
    }

    public static IPainting AddEntry(this IPainting painting, List<IPainting> list) {
        list.Add(painting);
        return painting;
    }

    public static IPainting RegisterLegacy(this IPainting painting, int legacyStyle) {
        LegacyPaintings.RegisterLegacy(painting.Mod, painting.Width, painting.Height, legacyStyle, painting.TileType);
        return painting;
    }
}
