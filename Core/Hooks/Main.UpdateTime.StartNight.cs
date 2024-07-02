namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Handles spawning the Glimmer event.</summary>
    private static void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents) {
        if (!Main.IsFastForwardingTime() && !stopEvents) {
#if !DEBUG
            Old.Content.Events.Glimmer.GlimmerSystem.OnTransitionToNight(ref stopEvents);
#endif
        }
        orig(ref stopEvents);
    }
}
