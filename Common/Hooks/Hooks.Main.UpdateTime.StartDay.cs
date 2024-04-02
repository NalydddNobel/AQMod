using Aequus.Common.Systems;
using Aequus.Content.TownNPCs;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        TimeSystem.OnStartDay();
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            NPCWantsToMoveIn.OnStartDay();
        }
        orig(ref stopEvents);
    }
}
