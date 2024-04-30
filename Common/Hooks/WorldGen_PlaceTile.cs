using Aequus.Common.Tiles.Components;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_WorldGen_PlaceTile_TryPlayCustomSound(int i, int j, ModTile modTile, bool forced, int plr, int style, bool PlaceTile) {
        if (modTile is ICustomPlaceSound customPlaceSound) {
            customPlaceSound.PlaySound(i, j, forced, plr, style, PlaceTile);
        }
    }

    private static bool On_WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
        bool muteOld = mute;
        var modTile = TileLoader.GetTile(Type);

        if (modTile is ICustomPlaceSound) {
            mute = true;
        }

        if (modTile is IOnPlaceTile onPlaceTile) {
            var overrideValue = onPlaceTile.PlaceTile(i, j, mute, forced, plr, style);
            if (overrideValue.HasValue) {
                if (!muteOld) {
                    On_WorldGen_PlaceTile_TryPlayCustomSound(i, j, modTile, forced, plr, style, overrideValue.Value);
                }
                return overrideValue.Value;
            }
        }

        var value = orig(i, j, Type, mute, forced, plr, style);

        if (!muteOld) {
            On_WorldGen_PlaceTile_TryPlayCustomSound(i, j, modTile, forced, plr, style, value);
        }
        return value;
    }
}
