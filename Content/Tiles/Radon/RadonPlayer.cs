using Aequus.Common.Particles;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Content.Tiles.Radon;

public class RadonPlayer : ModPlayer {
    #region Hair
    public static ushort ExposureRemoveHair { get; set; } = 7200;
    public static ushort ExposureShortenHair { get; set; } = 3600;

    public static byte TouchReduceDelay { get; set; } = 90;
    public static byte HairAnimationLength { get; set; } = 90;

    public byte hairAnimation;
    public ushort exposureTimeOld;
    public ushort exposureTime;
    public byte exposureReduceDelay;

    public override void PreUpdate() {
        ExposureRemoveHair = 120;
        if (hairAnimation == 0) {
            Main.NewText(exposureTime + ":" + exposureTimeOld + ":" + hairAnimation);
            if (exposureTimeOld < ExposureRemoveHair) {
                if (exposureTime >= ExposureRemoveHair) {
                    var offset = new Vector2(-10f, -10f);
                    EmberParticles.FromTexture(Player.position + Player.bodyPosition + offset, TextureAssets.PlayerHair[Player.hair].Value, Player.bodyFrame, 2, 2, ParticleLayer.AboveLiquid);
                    hairAnimation = HairAnimationLength;
                }
            }
            else if (exposureTimeOld > ExposureRemoveHair) {
                if (exposureTime < ExposureRemoveHair) {
                    hairAnimation = HairAnimationLength;
                }
            }

            exposureTimeOld = exposureTime;
        }
        else {
            hairAnimation--;
        }

        if (exposureReduceDelay > 0) {
            exposureReduceDelay--;
        }
        else if (exposureTime > 0) {
            exposureTime = (ushort)Math.Max(exposureTime - 10, 0);
        }
    }

    private bool CheckHairAnimation(ushort compareTime) {
        return false;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
        if (exposureTime > ExposureRemoveHair) {
            drawInfo.fullHair = false;
            drawInfo.hatHair = false;
            drawInfo.hideHair = true;
            drawInfo.colorHair *= 0f;
        }
        else if (hairAnimation != 0) {
            drawInfo.colorHair *= Math.Clamp(1f - hairAnimation / (float)HairAnimationLength, 0f, 1f);
        }
    }
    #endregion
}
