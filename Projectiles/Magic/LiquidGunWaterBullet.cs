using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class LiquidGunWaterBullet : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 8;
            Projectile.hide = true;
            Projectile.alpha = 4;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.alpha > 0)
            {
                Projectile.alpha--;
            }
            else if ((int)Projectile.ai[1] >= -1)
            {
                for (int i = 0; i < 2; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[1], Scale: Main.rand.NextFloat(0.75f, 1.25f));
                    if (d.type == DustID.Torch)
                    {
                        d.scale *= Main.rand.NextFloat(1f, 2f);
                    }
                    d.velocity *= 0.2f;
                    d.noGravity = true;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 2;
            height = 2;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] == DustID.Torch)
            {
                target.AddBuff(BuffID.OnFire, 120 + 120 * Main.rand.Next(3));
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.ai[1] == DustID.Torch)
            {
                target.AddBuff(BuffID.OnFire, 120 + 120 * Main.rand.Next(3));
            }
            if (Projectile.ai[1] == DustID.Honey2)
            {
                target.AddBuff(BuffID.Honey, 120);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[1], Scale: Main.rand.NextFloat(0.75f, 1.25f));
                d.velocity *= 0.2f;
                float rot = Main.rand.NextFloat(-0.5f, 0.5f);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                d.velocity -= Projectile.oldVelocity.RotatedBy(rot) * Main.rand.NextFloat(1f);
                d.velocity.Y -= 1f;
                d.noGravity = true;
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }
    }
}