using Aequus.Buffs.Debuffs;
using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class NightfallProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.RainbowCrystalExplosion}";

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0f)
            {
                Projectile.timeLeft = 60;
                Projectile.alpha = 0;
                if (Projectile.localAI[0] == 0f)
                {
                    Projectile.Center = Main.npc[(int)Projectile.ai[0] - 1].Center + Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.localAI[0] = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < 10; i++)
                    {
                        var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.White, Color.HotPink, 0.6f).UseA(75), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.velocity = (Projectile.localAI[0] + MathHelper.PiOver2).ToRotationVector2() * i / 2f * (Main.rand.NextBool() ? -1f : 1f);
                    }
                }
                Projectile.rotation = Projectile.localAI[0];
                Projectile.scale += 0.33f;
                if (Projectile.scale > 3f)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 30)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.velocity *= 0.98f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override void Kill(int timeLeft)
        {
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity = (Projectile.Center - target.Center) * 0.5f;
            Projectile.ai[0] = target.whoAmI + 1f;
            Projectile.damage = 0;
            NightfallDebuff.AddBuff(target, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.Lerp(Color.White, Color.HotPink, 0.6f).UseA(150) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(1f, Projectile.scale) * 0.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}