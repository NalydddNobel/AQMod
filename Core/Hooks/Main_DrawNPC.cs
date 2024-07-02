using Aequus.Core.Entities.NPCs;

namespace Aequus.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
        if (!Main.npc[iNPCIndex].TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            orig(main, iNPCIndex, behindTiles);
            return;
        }

        Vector2 drawOffset = Vector2.Zero;
        aequusNPC.DrawBehindNPC(iNPCIndex, behindTiles, ref drawOffset);
        Main.npc[iNPCIndex].position += drawOffset;
        orig(main, iNPCIndex, behindTiles);
        Main.npc[iNPCIndex].position -= drawOffset;
        aequusNPC.DrawAboveNPC(iNPCIndex, behindTiles);
    }
}
