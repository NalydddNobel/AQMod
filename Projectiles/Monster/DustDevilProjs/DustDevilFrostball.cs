using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.DustDevilProjs
{
    public class DustDevilFrostball : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.RainbowCrystalExplosion;

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

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
            return new Color(80, 255, 255, 0) * 0.6f * Projectile.Opacity;
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
                Projectile.frame = Main.rand.Next(5);
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
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                d.velocity = Vector2.Lerp(-Projectile.velocity, d.velocity, 0.75f);
                d.velocity *= 0.8f;
                d.scale *= Main.rand.NextFloat(0.7f, 1f);
                d.fadeIn = d.scale + 0.2f;
                d.noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.Chilled, 60);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var _, out var origin, out int _);
            Main.instance.LoadProjectile(ProjectileID.Blizzard);
            var t = TextureAssets.Projectile[ProjectileID.Blizzard];
            var tFrame = t.Frame(verticalFrames: 5, frameY: Projectile.frame);
            Main.spriteBatch.Draw(t.Value, Projectile.position + offset - Main.screenPosition, tFrame, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tFrame.Width / 2f, 4f),
                Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2, origin, new Vector2(Projectile.scale * 0.5f, Projectile.scale), SpriteEffects.None, 0f);
            return false;
        }
    }
}