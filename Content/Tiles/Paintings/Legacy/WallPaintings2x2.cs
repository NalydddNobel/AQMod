namespace Aequus.Content.Tiles.Paintings.Legacy;

public class WallPaintings2x2() : LegacyPaintingTile(2, 2) {
    public override ushort[] ConvertIds() {
        return [
            Paintings.Instance.Yin!.TileType,
            Paintings.Instance.Yang!.TileType,
        ];
    }
}