using AequusRemake.Core.Entities.Tiles.Components;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_WorldGen_PlaceTile_TryPlayCustomSound(int i, int j, ModTile modTile, bool forced, int plr, int style, bool PlaceTile) {
        if (modTile is ICustomPlaceSound customPlaceSound) {
            customPlaceSound.PlaySound(i, j, forced, plr, style, PlaceTile);
        }
    }

    private static bool On_WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
        bool muteOld = mute;
        ModTile modTile = TileLoader.GetTile(Type);

        if (modTile is ICustomPlaceSound) {
            mute = true;
        }

        bool value = true;

        if (modTile is IOnPlaceTile onPlaceTile) {
            value = onPlaceTile.PlaceTile(i, j, mute, forced, plr, style) ?? value;
        }

        if (value) {
            value = orig(i, j, Type, mute, forced, plr, style);
        }

        if (!muteOld) {
            On_WorldGen_PlaceTile_TryPlayCustomSound(i, j, modTile, forced, plr, style, value);
        }
        return value;
    }
}
