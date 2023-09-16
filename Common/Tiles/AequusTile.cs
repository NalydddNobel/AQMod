using Aequus.Common.Tiles.Components;
using Terraria;
using Terraria.ID;
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

    private static bool WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
        if (Type >= TileID.Count && TileLoader.GetTile(Type) is IOnPlaceTile onPlaceTile) {
            var val = onPlaceTile.OnPlaceTile(i, j, mute, forced, plr, style);
            if (val.HasValue) {
                return val.Value;
            }
        }
        return orig(i, j, Type, mute, forced, plr, style);
    }
    #endregion

    public void PostSetupContent(Aequus aequus) {
        All = new bool[TileLoader.TileCount];
        for (int i = 0; i < All.Length; i++) {
            All[i] = true;
        }
    }
}