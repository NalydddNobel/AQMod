using Terraria.DataStructures;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static bool drawingPlayer;

    /// <summary>Allows for custom dash accessories added by AequusRemake to update dash movement.</summary>
    private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo) {
        if (drawingPlayer || !drawinfo.drawPlayer.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer)) {
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

        AequusPlayer AequusRemakePlayer = info.drawPlayer.GetModPlayer<AequusPlayer>();

        if (AequusRemakePlayer.DrawScale != null) {
            ScalePlayer(info.drawPlayer, ref info, AequusRemakePlayer.DrawScale.Value);
        }
        if (AequusRemakePlayer.DrawForceDye != null) {
            DyePlayer(ref info, AequusRemakePlayer.DrawForceDye.Value);
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
