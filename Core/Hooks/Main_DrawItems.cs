namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawItems(On_Main.orig_DrawItems orig, Main main) {
        orig(main);
    }
}
