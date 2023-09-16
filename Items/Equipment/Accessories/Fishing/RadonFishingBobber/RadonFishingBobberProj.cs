using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Fishing.RadonFishingBobber;

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