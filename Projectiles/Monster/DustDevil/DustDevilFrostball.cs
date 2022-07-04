using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.DustDevil
{
    public class DustDevilFrostball : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.RainbowCrystalExplosion;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(70, 235, 255) * Projectile.Opacity;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha <= 10;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Projectile.velocity.Length();
                Projectile.velocity *= 0.1f;
            }
            else
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                float dir = Math.Sign(Projectile.ai[0]);
                if (dir == 0)
                    Projectile.ai[0] = 1;
                float power = Math.Clamp((float)Math.Pow(Projectile.ai[0].Abs() / 120f, 2f), 0.1f, 1f);

                var old = Projectile.velocity;
                Projectile.velocity = Vector2.Normalize(Projectile.velocity.RotatedBy(0.01f * dir)) * Projectile.ai[1] * power;
                Projectile.alpha = 0;
                Projectile.ai[0] += dir;
            }
            if (Projectile.alpha < 10)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                Main.dust[d].velocity = Vector2.Lerp(-Projectile.velocity, Main.dust[d].velocity, 0.5f);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.Chilled, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale), SpriteEffects.None, 0f);
            return false;
        }
    }
}