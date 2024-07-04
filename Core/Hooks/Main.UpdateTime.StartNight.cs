namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents) {
        if (!Main.IsFastForwardingTime() && !stopEvents) {
        }
        orig(ref stopEvents);
    }
}
