namespace Aequus.Common.Tiles.Components;

public interface IOnPlaceTile {
    System.Boolean? PlaceTile(System.Int32 i, System.Int32 j, System.Boolean mute, System.Boolean forced, System.Int32 plr, System.Int32 style);
}