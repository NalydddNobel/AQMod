using Terraria.DataStructures;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    private static bool drawingPlayer;

    /// <summary>Allows for custom dash accessories added by Aequu2 to update dash movement.</summary>
    private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo) {
        if (drawingPlayer || !drawinfo.drawPlayer.TryGetModPlayer<AequusPlayer>(out var Aequu2Player)) {
            orig(ref drawinfo);
            return;
        }

        drawingPlayer = true;
        ModifyDrawSet(ref drawinfo);

        orig(ref drawinfo);

        drawingPlayer = false;
    }

    private static void ModifyDrawSet(ref PlayerDrawSet info) {
        if (info.headOnlyRender) {
            return;
        }

        AequusPlayer Aequu2Player = info.drawPlayer.GetModPlayer<AequusPlayer>();

        if (Aequu2Player.DrawScale != null) {
            ScalePlayer(info.drawPlayer, ref info, Aequu2Player.DrawScale.Value);
        }
        if (Aequu2Player.DrawForceDye != null) {
            DyePlayer(ref info, Aequu2Player.DrawForceDye.Value);
        }
    }

    private static void ScalePlayer(Player drawPlayer, ref PlayerDrawSet info, float drawScale) {
        var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
        to -= Main.screenPosition;
        for (int i = 0; i < info.DrawDataCache.Count; i++) {
            DrawData data = info.DrawDataCache[i];
            data.position -= (data.position - to) * (1f - drawScale);
            data.scale *= drawScale;
            info.DrawDataCache[i] = data;
        }
    }

    private static void DyePlayer(ref PlayerDrawSet info, int dye) {
        for (int i = 0; i < info.DrawDataCache.Count; i++) {
            DrawData data = info.DrawDataCache[i];
            data.shader = dye;
            info.DrawDataCache[i] = data;
        }
    }
}
