using Aequus.Common.Players;
using Aequus.Core.Generator;
using Terraria.DataStructures;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public System.Boolean visualAfterImages;
    [ResetEffects]
    public System.Single? CustomDrawShadow;
    [ResetEffects]
    public System.Single? DrawScale;
    [ResetEffects]
    public System.Int32? DrawForceDye;

    private static System.Boolean customDrawing;

    private static void On_Player_UpdateVisibleAccessories(On_Player.orig_UpdateVisibleAccessories orig, Player player) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            orig(player);
            return;
        }

        CustomDashData dashData = player.dashDelay < 0 && player.dash == -1 && aequusPlayer.DashData != null ? aequusPlayer.DashData : null;
        dashData?.PreUpdateVisibleAccessories(player, aequusPlayer);
        orig(player);
        dashData?.PostUpdateVisibleAccessories(player, aequusPlayer);
    }
    private static void PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo) {
        try {
            if (customDrawing || !drawinfo.drawPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
                orig(ref drawinfo);
                return;
            }
            customDrawing = true;
            aequusPlayer.ModifyDrawSet(ref drawinfo);
            orig(ref drawinfo);
        }
        catch {

        }
        customDrawing = false;
    }

    private void ModifyDrawSet(ref PlayerDrawSet info) {
        if (info.headOnlyRender) {
            return;
        }

        if (DrawScale != null) {
            var drawPlayer = info.drawPlayer;
            var to = new Vector2((System.Int32)drawPlayer.position.X + drawPlayer.width / 2f, (System.Int32)drawPlayer.position.Y + drawPlayer.height);
            to -= Main.screenPosition;
            for (System.Int32 i = 0; i < info.DrawDataCache.Count; i++) {
                DrawData data = info.DrawDataCache[i];
                data.position -= (data.position - to) * (1f - DrawScale.Value);
                data.scale *= DrawScale.Value;
                info.DrawDataCache[i] = data;
            }
        }
        if (DrawForceDye != null) {
            var drawPlayer = info.drawPlayer;
            for (System.Int32 i = 0; i < info.DrawDataCache.Count; i++) {
                DrawData data = info.DrawDataCache[i];
                data.shader = DrawForceDye.Value;
                info.DrawDataCache[i] = data;
            }
        }
    }

    public override void FrameEffects() {
        if (visualAfterImages) {
            Player.armorEffectDrawShadow = true;
        }
        if (Player.dashDelay != 0 && Player.dash == -1 && DashData != null) {
            DashData.OnPlayerFrame(Player, this);
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
        if (CustomDrawShadow != null) {
            drawInfo.shadow = CustomDrawShadow.Value;
            System.Single shadow = 1f - CustomDrawShadow.Value;
            drawInfo.colorArmorBody *= shadow;
            drawInfo.colorArmorHead *= shadow;
            drawInfo.colorArmorLegs *= shadow;
            drawInfo.colorBodySkin *= shadow;
            drawInfo.colorElectricity *= shadow;
            drawInfo.colorEyes *= shadow;
            drawInfo.colorEyeWhites *= shadow;
            drawInfo.colorHair *= shadow;
            drawInfo.colorHead *= shadow;
            drawInfo.colorLegs *= shadow;
            drawInfo.colorMount *= shadow;
            drawInfo.colorPants *= shadow;
            drawInfo.colorShirt *= shadow;
            drawInfo.colorShoes *= shadow;
            drawInfo.colorUnderShirt *= shadow;
            drawInfo.ArkhalisColor *= shadow;
            drawInfo.armGlowColor *= shadow;
            drawInfo.bodyGlowColor *= shadow;
            drawInfo.floatingTubeColor *= shadow;
            drawInfo.headGlowColor *= shadow;
            drawInfo.itemColor *= shadow;
            drawInfo.legsGlowColor *= shadow;
        }
    }
}