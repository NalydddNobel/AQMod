using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    public class ZombieSceptreProjOnHit : ModProjectile {
        public override string Texture => AequusTextures.Flare.Path;

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Blue with { A = 0 };
        }

        public override bool PreDraw(ref Color lightColor) {
            return true;
        }
    }
}
