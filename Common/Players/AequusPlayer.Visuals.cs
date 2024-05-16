using Aequus.Core.CodeGeneration;
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