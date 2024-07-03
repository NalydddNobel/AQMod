using Aequu2.Core.Systems;
using Aequu2.Content.TownNPCs;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Handles incrementing the day counter, and the NPC wants to settle down system.</summary>
    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        TimeSystem.OnStartDay();
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            NPCWantsToMoveIn.OnStartDay();
        }
        orig(ref stopEvents);
    }
}
