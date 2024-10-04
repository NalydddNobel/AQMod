namespace Aequus.Common.Entities.Tiles;

public interface IPlaceTile {
    bool? ModifyPlaceTile(ref PlaceTileInfo info);
}

public record struct PlaceTileInfo(int X, int Y, bool Mute, bool Forced, int Style, int Player) {
    public Tile Tile => Main.tile[X, Y];
}