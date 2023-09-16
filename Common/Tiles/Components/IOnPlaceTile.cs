namespace Aequus.Common.Tiles.Components;

public interface IOnPlaceTile {
    bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style);
}