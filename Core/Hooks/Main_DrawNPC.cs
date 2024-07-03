using Aequu2.Core.Entities.NPCs;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
        if (!Main.npc[iNPCIndex].TryGetGlobalNPC<Aequu2NPC>(out var Aequu2NPC)) {
            orig(main, iNPCIndex, behindTiles);
            return;
        }

        Vector2 drawOffset = Vector2.Zero;
        Aequu2NPC.DrawBehindNPC(iNPCIndex, behindTiles, ref drawOffset);
        Main.npc[iNPCIndex].position += drawOffset;
        orig(main, iNPCIndex, behindTiles);
        Main.npc[iNPCIndex].position -= drawOffset;
        Aequu2NPC.DrawAboveNPC(iNPCIndex, behindTiles);
    }
}
