namespace Aequus.Common.Entities.Tiles;

public class TileHooks : LoadedType {
    protected override void Load() {
        On_WorldGen.PlaceTile += On_WorldGen_PlaceTile;
    }

    static bool On_WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
        ModTile modTile = TileLoader.GetTile(Type);
        bool value = false;

        if (modTile is IPlaceTile onPlaceTile) {
            PlaceTileInfo info = new(i, j, mute, forced, style, plr);

            value = onPlaceTile.ModifyPlaceTile(ref info) ?? value;

            i = info.X;
            j = info.Y;
            mute = info.Mute;
            forced = info.Forced;
            style = info.Style;
            plr = info.Player;
        }

        if (!value) {
            value = orig(i, j, Type, mute, forced, plr, style);
        }
        return value;
    }
}
