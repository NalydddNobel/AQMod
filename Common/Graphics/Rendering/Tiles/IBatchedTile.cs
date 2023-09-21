using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Graphics.Rendering.Tiles;

/// <summary>
/// Grants a ModTile the ability to render post-draw elements in a batch to reduce on Begin/End calls.
/// <para>Place SpecialTileDrawing.Add(i, j, Type); into ModTile.PostDraw(...) to add it to the rendering list.</para>
/// </summary>
public interface IBatchedTile : ILoadable {
    bool SolidLayerTile { get; }

    void BatchedPreDraw(List<BatchedTileDrawInfo> tiles, int count) {
    }
    void BatchedPostDraw(List<BatchedTileDrawInfo> tiles, int count) {
    }
}
