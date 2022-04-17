using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class TouhouBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 275;
            Projectile.penetrate = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return FrameToColor(Projectile.frame);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] >= 100f)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 120f && Projectile.localAI[1] <= 0f)
                {
                    Projectile.position.X -= 40f;
                    Projectile.position.Y -= 40f;
                    Projectile.width += 80;
                    Projectile.height += 80;
                    Projectile.localAI[1] = 1f;
                }
                if (Projectile.ai[0] > 140f)
                {
                    Projectile.ai[0] = 100f;
                    Projectile.localAI[1] = 0f;
                    Projectile.position.X += 40f;
                    Projectile.position.Y += 40f;
                    Projectile.width -= 80;
                    Projectile.height -= 80;
                }
            }
            if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() < 1250f)
            {
                Projectile.timeLeft = 20;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.ai[1], Projectile.velocity.Length() / Projectile.ai[1] * 0.1f);

            if (Projectile.alpha < 200)
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 25;
                Projectile.localAI[0] *= 0.8f;
                float colorMultiplier = 1 - Projectile.alpha / 255f;
                Lighting.AddLight(Projectile.Center, Projectile.GetAlpha(default(Color)).ToVector3() * Projectile.scale * 0.5f * colorMultiplier);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.alpha > 255)
            {
                return false;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 20f;
            }
            float colorMultiplier = 1 - Projectile.alpha / 255f;
            float colorMultiplierSquared = colorMultiplier * colorMultiplier;
            var texture = TextureAssets.Projectile[Type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var spotlight = Aequus.MyTex("Assets/Bloom");
            Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * colorMultiplierSquared, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.6f, SpriteEffects.None, 0f);
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, new Color(255, 255, 255, 255) * colorMultiplierSquared, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            if (Projectile.localAI[0] > Projectile.scale * 0.6f)
            {
                Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * colorMultiplier, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * Projectile.localAI[0], SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255) * colorMultiplier, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * Projectile.localAI[0] * 0.7f, SpriteEffects.None, 0f);
            }

            if (Projectile.ai[0] > 120f)
            {
                var explosionTexture = Aequus.MyTex("Assets/Explosion");
                int explosionFrameNumber = (int)((Projectile.ai[0] - 120f) / 4f);
                var explosionFrame = explosionTexture.Frame(verticalFrames: 5, frameY: explosionFrameNumber);
                var explosionOrigin = explosionFrame.Size() / 2f;
                Main.spriteBatch.Draw(explosionTexture, Projectile.position + offset - Main.screenPosition, explosionFrame, Projectile.frame == 1 ? Color.Red : Color.Blue, 0f, explosionOrigin, Projectile.scale * 2f, SpriteEffects.None, 0f);
            }
            return false;
        }

        public static Color FrameToColor(int frame)
        {
            switch (frame)
            {
                default:
                    return new Color(255, 255, 255, 255);
                case 1:
                    return new Color(255, 10, 10, 255);
                case 2:
                    return new Color(255, 255, 10, 255);
                case 3:
                    return new Color(50, 255, 10, 255);
                case 4:
                    return new Color(10, 255, 255, 255);
                case 5:
                    return new Color(10, 10, 255, 255);
                case 6:
                    return new Color(255, 10, 255, 255);
            }
        }
    }
}