namespace Aequus.Items.Equipment.Accessories.Misc.Fishing.RadonFishingBobber;

public class RadonFishingBobberProj : ModProjectile {
    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.FishingBobberGlowingRainbow);
        Projectile.light = 0f;
        Projectile.glowMask = -1;
        AIType = ProjectileID.FishingBobber;
    }
}