using Aequus.Common.Tiles.Components;
using Aequus.Core.Initialization;

namespace Aequus.Common.Tiles;

public partial class AequusTile : GlobalTile, IPostSetupContent {
    internal static System.Boolean[] All;

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

    private static void TryPlayCustomSound(System.Int32 i, System.Int32 j, ModTile modTile, System.Boolean forced, System.Int32 plr, System.Int32 style, System.Boolean PlaceTile) {
        if (modTile is ICustomPlaceSound customPlaceSound) {
            customPlaceSound.PlaySound(i, j, forced, plr, style, PlaceTile);
        }
    }

    private static System.Boolean WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, System.Int32 i, System.Int32 j, System.Int32 Type, System.Boolean mute, System.Boolean forced, System.Int32 plr, System.Int32 style) {
        System.Boolean muteOld = mute;
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
        All = new System.Boolean[TileLoader.TileCount];
        for (System.Int32 i = 0; i < All.Length; i++) {
            All[i] = true;
        }
    }

    public override void RightClick(System.Int32 i, System.Int32 j, System.Int32 type) {
        CheckCustomKeys(i, j, type);
    }
}