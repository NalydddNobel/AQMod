using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.DustDevil.Projectiles
{
    public class DustDevilFireball : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.Flamelash;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.Flamelash];
            PushableEntities.AddProj(Type);
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
                float power = Math.Clamp((float)Math.Pow(Projectile.ai[0].Abs() / 60f, 2f), 0.1f, 1f);

                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[1] * power;
                Projectile.alpha = 0;
                Projectile.ai[0] += dir;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.alpha < 10)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[d].velocity = Vector2.Lerp(Projectile.velocity, Main.dust[d].velocity, 0.5f);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].noGravity = true;
            }
            Projectile.LoopingFrame(4);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}