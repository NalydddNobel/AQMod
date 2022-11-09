using Aequus.Buffs.Debuffs;
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
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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
                    Projectile.localAI[0] = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                Projectile.Center = Main.npc[(int)Projectile.ai[0] - 1].Center;
                Projectile.rotation = Projectile.localAI[0];
                Projectile.scale += 0.33f;
                if (Projectile.scale > 3f)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.alpha > 0)
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
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.Lerp(Color.White, Color.HotPink, 0.33f) * Projectile.Opacity, Projectile.rotation, origin, new Vector2(1f, Projectile.scale) * 0.4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}