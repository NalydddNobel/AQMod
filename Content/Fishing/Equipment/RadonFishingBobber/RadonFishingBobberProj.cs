using Microsoft.Xna.Framework;

namespace Aequus.Content.Fishing.Equipment.RadonFishingBobber;

public class RadonFishingBobberProj : ModProjectile {
    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.FishingBobberGlowingRainbow);
        Projectile.light = 0f;
        Projectile.glowMask = -1;
        AIType = ProjectileID.FishingBobber;
    }

    public override void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor) {
        if (Main.player[Projectile.owner].HeldItem.type >= ItemID.Count) {
            return;
        }

        if (Main.player[Projectile.owner].direction == -1) {
            lineOriginOffset.X -= 13f; // Stupid
        }
    }
}