namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main main, bool behindTiles) {
        orig(main, behindTiles);
    }
}
