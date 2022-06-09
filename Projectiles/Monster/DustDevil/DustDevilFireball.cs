using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.DustDevil
{
    public class DustDevilFireball : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.Flamelash;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.Flamelash];
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            Main.dust[d].velocity = Vector2.Lerp(Projectile.velocity, Main.dust[d].velocity, 0.5f);
            Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
            Main.dust[d].noGravity = true;
            Projectile.LoopingFrame(4);
        }
    }
}