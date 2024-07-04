using AequusRemake.Core.Entities.NPCs;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPC(On_Main.orig_DrawNPC orig, Main main, int iNPCIndex, bool behindTiles) {
        if (!Main.npc[iNPCIndex].TryGetGlobalNPC<AequusRemakeNPC>(out var AequusRemakeNPC)) {
            orig(main, iNPCIndex, behindTiles);
            return;
        }

        Vector2 drawOffset = Vector2.Zero;
        AequusRemakeNPC.DrawBehindNPC(iNPCIndex, behindTiles, ref drawOffset);
        Main.npc[iNPCIndex].position += drawOffset;
        orig(main, iNPCIndex, behindTiles);
        Main.npc[iNPCIndex].position -= drawOffset;
        AequusRemakeNPC.DrawAboveNPC(iNPCIndex, behindTiles);
    }
}
