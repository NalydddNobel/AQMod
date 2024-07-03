namespace Aequu2.Core.Entities.Tiles.Components;

public interface ICustomPlaceSound {
    void PlaySound(int i, int j, bool forced, int plr, int style, bool PlaceTile);
}