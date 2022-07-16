using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.RedSpriteProjs
{
    public class RedSpriteWindFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            this.SetTrail(12);
            AequusProjectile.HeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 750;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 24;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.alpha < 200)
            {
                for (int i = 0; i < 2; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch);
                    d.velocity *= 0.35f;
                    d.velocity += -Projectile.velocity / 16f;
                    d.scale *= 2f;
                    d.color *= Projectile.Opacity;
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var orig = texture.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            float speedX = Projectile.velocity.X.Abs();
            lightColor = Projectile.GetAlpha(lightColor);
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;

            int trailLength = Projectile.TrailLength();
            float opacity = Projectile.Opacity;
            for (int i = 0; i < trailLength; i++)
            {
                float progress = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset, frame, new Color(100, 20, 10, 0) * progress * opacity, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, new Color(222, 60, 20, 20) * opacity, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(Projectile.position, 16, 16, ModContent.DustType<RedSpriteDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
            }
        }
    }
}