using Terraria.DataStructures;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo) {
        if (AequusPlayer.customDrawing || !drawinfo.drawPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            orig(ref drawinfo);
            return;
        }

        AequusPlayer.customDrawing = true;
        aequusPlayer.ModifyDrawSet(ref drawinfo);

        orig(ref drawinfo);

        AequusPlayer.customDrawing = false;
    }
}
