using Aequu2.Core.ContentGeneration;
using Terraria.ObjectData;

namespace Aequu2.Content.Tiles.Furniture.Trash;

internal class TrashCampfire(UnifiedFurniture parent) : InstancedFurnitureCampfire(parent) {
    protected override void PreAddTileObjectData() {
        base.PreAddTileObjectData();
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.Origin = new(0, 2);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 18];
        TileObjectData.newTile.DrawYOffset = -2;
        TileObjectData.newTile.AnchorBottom = TileObjectData.newTile.AnchorBottom with { tileCount = TileObjectData.newTile.Width };
        LightRGB = new Vector3(0.9f, 0.3f, 0.1f);
    }
}
