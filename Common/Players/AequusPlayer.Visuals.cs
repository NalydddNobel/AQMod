using Aequus.Common.Players;
using Aequus.Core.CodeGeneration;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool visualAfterImages;
    [ResetEffects]
    public float? CustomDrawShadow;
    [ResetEffects]
    public float? DrawScale;
    [ResetEffects]
    public int? DrawForceDye;

    internal static bool customDrawing;

    internal void ModifyDrawSet(ref PlayerDrawSet info) {
        if (info.headOnlyRender) {
            return;
        }

        if (DrawScale != null) {
            var drawPlayer = info.drawPlayer;
            var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
            to -= Main.screenPosition;
            for (int i = 0; i < info.DrawDataCache.Count; i++) {
                DrawData data = info.DrawDataCache[i];
                data.position -= (data.position - to) * (1f - DrawScale.Value);
                data.scale *= DrawScale.Value;
                info.DrawDataCache[i] = data;
            }
        }
        if (DrawForceDye != null) {
            var drawPlayer = info.drawPlayer;
            for (int i = 0; i < info.DrawDataCache.Count; i++) {
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
            float shadow = 1f - CustomDrawShadow.Value;
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