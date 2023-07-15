using Terraria.ModLoader;

namespace Aequus.Content.Necromancy {
    public class NecromancyHitbox : ModProjectile {
        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults() {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 16;
        }
    }
}