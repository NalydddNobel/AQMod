namespace Aequus.Common.Tiles;

public interface IOnPlaceTile {
    bool? PlaceTile(int i, int j, bool mute, bool forced, int plr, int style);
}