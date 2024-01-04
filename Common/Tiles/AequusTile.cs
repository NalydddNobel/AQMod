using Aequus.Common.Tiles.Components;
using Aequus.Core.Initialization;
using System;

namespace Aequus.Common.Tiles;

public partial class AequusTile : GlobalTile, IPostSetupContent {
    internal static bool[] All;

    public override void Load() {
        On_WorldGen.PlaceTile += WorldGen_PlaceTile;
        On_WorldGen.PlaceChest += On_WorldGen_PlaceChest;
        On_WorldGen.PlaceChestDirect += On_WorldGen_PlaceChestDirect; ;
        On_WorldGen.UpdateWorld_OvergroundTile += WorldGen_UpdateWorld_OvergroundTile;
        On_WorldGen.UpdateWorld_UndergroundTile += WorldGen_UpdateWorld_UndergroundTile;
        //On_WorldGen.QuickFindHome += WorldGen_QuickFindHome;
    }

    private static void On_WorldGen_PlaceChestDirect(On_WorldGen.orig_PlaceChestDirect orig, int x, int y, ushort type, int style, int id) {
        if (TileLoader.GetTile(type) is IPlaceChestHook placeChest && !placeChest.PlaceChestDirect(x, y, style, id)) {
            return;
        }

        orig(x, y, type, style, id);
    }

    private static int On_WorldGen_PlaceChest(On_WorldGen.orig_PlaceChest orig, int x, int y, ushort type, bool notNearOtherChests, int style) {
        if (TileLoader.GetTile(type) is IPlaceChestHook placeChest) {
            int result = placeChest.PlaceChest(x, y, style, notNearOtherChests);
            if (result != -2) {
                return Math.Clamp(result, -1, Main.maxChests);
            }
            
        }
        return orig(x, y, type, notNearOtherChests, style);
    }

    #region Hooks
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