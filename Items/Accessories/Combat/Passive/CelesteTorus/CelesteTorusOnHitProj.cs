using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Passive.CelesteTorus;

public class CelesteTorusOnHitProj : ModProjectile {
    public override string Texture => AequusTextures.None.Path;

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 10;
    }

    public override void AI() {

    }

    public override bool PreDraw(ref Color lightColor) {
        return base.PreDraw(ref lightColor);
    }
}