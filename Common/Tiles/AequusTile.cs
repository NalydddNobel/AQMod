using Aequus.Common.Tiles.Components;
using Aequus.Core.Autoloading;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public partial class AequusTile : GlobalTile, IPostSetupContent {
    internal static bool[] All;

    public override void Load() {
        LoadHooks();
    }

    #region Hooks
    private static void LoadHooks() {
        On_WorldGen.PlaceTile += WorldGen_PlaceTile;
        On_WorldGen.UpdateWorld_OvergroundTile += WorldGen_UpdateWorld_OvergroundTile;
        On_WorldGen.UpdateWorld_UndergroundTile += WorldGen_UpdateWorld_UndergroundTile;
        //On_WorldGen.QuickFindHome += WorldGen_QuickFindHome;
    }

    private static void TryPlayCustomSound(int i, int j, ModTile modTile, bool forced, int plr, int style, bool PlaceTile) {
        if (modTile is ICustomPlaceSound customPlaceSound) {
            customPlaceSound.PlaySound(i, j, forced, plr, style, PlaceTile);
        }
    }

    private static bool WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
        bool muteOld = mute;
        var modTile = TileLoader.GetTile(Type);

        if (modTile is ICustomPlaceSound) {
            mute = true;
        }

        if (modTile is IOnPlaceTile onPlaceTile) {
            var overrideValue = onPlaceTile.PlaceTile(i, j, mute, forced, plr, style);
            if (overrideValue.HasValue) {
                if (!muteOld) {
                    TryPlayCustomSound(i, j, modTile, forced, plr, style, overrideValue.Value);
                }
                return overrideValue.Value;
            }
        }

        var value = orig(i, j, Type, mute, forced, plr, style);

        if (!muteOld) {
            TryPlayCustomSound(i, j, modTile, forced, plr, style, value);
        }
        return value;
    }
    #endregion

    public void PostSetupContent(Aequus aequus) {
        All = new bool[TileLoader.TileCount];
        for (int i = 0; i < All.Length; i++) {
            All[i] = true;
        }
    }

    public override void RightClick(int i, int j, int type) {
        CheckCustomKeys(i, j, type);
    }
}