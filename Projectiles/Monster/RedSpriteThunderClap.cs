using Aequus;
using Aequus.Common.Catalogues;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class RedSpriteThunderClap : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;

            HeatDamageTypes.HeatProjectile.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 16;
        }

        public override void AI()
        {
            var center = Projectile.Center;
            float minimumLength = 280f;
            byte closest = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (Main.player[closest].dead || !Main.player[closest].active)
            {
                return;
            }
            if (Main.player[closest].position.Y + Main.player[closest].height > center.Y + minimumLength)
            {
                minimumLength = Main.player[closest].position.Y + Main.player[closest].height - center.Y;
            }
            for (Projectile.ai[0] = minimumLength; Projectile.ai[0] < 1500f; Projectile.ai[0] += 4f)
            {
                if (!Collision.CanHit(center, 1, 1, new Vector2(center.X, center.Y + Projectile.ai[0]), 1, 1))
                {
                    Projectile.ai[0] -= 4f;
                    break;
                }
            }

            DelegateMethods.v3_1 = new Vector3(0.8f, 0.5f, 0.1f);
            Utils.PlotTileLine(center, center + new Vector2(0f, Projectile.ai[0]), 1f, DelegateMethods.CastLight);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var center = Projectile.Center;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center,
                center + new Vector2(0f, Projectile.ai[0]), 22, ref point);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center;
            var scale = new Vector2(Projectile.scale, Projectile.scale);
            scale.X -= 1f - Projectile.timeLeft / 32f;
            lightColor = Projectile.GetAlpha(lightColor);
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
            var orig = new Vector2(texture.Width / 2f, frame.Height - 4f);
            float electric = 2f + ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) + 1f) * 2f;

            int separation = frame.Height - 6;
            var glow = Images.Bloom[4].Value;
            var glowScale = new Vector2(Projectile.ai[0] / glow.Width * 2f, scale.X * 2f);
            var thunderGlowOrig = new Vector2(glow.Width / 4f, glow.Height / 2f);
            var glowBright = new Color(200, 140, 30);
            var glowDark = new Color(80, 20, 2, 0);

            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, Projectile.ai[0] / 2f) - Main.screenPosition, new Rectangle(0, 0, glow.Width / 2, glow.Height), new Color(100, 20, 2, 0), Projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, new Vector2(glowScale.X, glowScale.Y * 1.5f), SpriteEffects.None, 0f);

            if (ClientConfig.Instance.HighQuality)
            {
                var clr = new Color(255, 100, 0, 20);
                for (int i = 0; i < 8; i++)
                {
                    float length2 = Projectile.ai[0];
                    var off = new Vector2(electric, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    off.X *= scale.X;
                    while (true)
                    {
                        var position = drawPosition + new Vector2(0f, length2 - separation) + off;
                        length2 -= separation;
                        if (length2 < separation)
                        {
                            frame.Y = 1 * frame.Height;
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            frame.Y = 0;
                            Main.spriteBatch.Draw(texture, drawPosition + off - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            frame.Y = frame.Height * 3;
                            Main.spriteBatch.Draw(texture, drawPosition + off + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            break;
                        }
                        else
                        {
                            frame.Y = (Main.rand.Next(2) + 1) * frame.Height;
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            float length = Projectile.ai[0];

            while (true)
            {
                var position = drawPosition + new Vector2(0f, length - separation);
                length -= separation;
                if (length < separation)
                {
                    var glow2 = Images.Bloom[0].Value;
                    var glow2Orig = glow2.Size() / 2f;
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, -frame.Height / 2f) - Main.screenPosition, null, glowBright, Projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);
                    frame.Y = 1 * frame.Height;
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    frame.Y = 0;
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    frame.Y = frame.Height * 3;
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, Projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowDark, Projectile.rotation, glow2Orig, scale * 3f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, Projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowBright, Projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);
                    break;
                }
                else
                {
                    frame.Y = (Main.rand.Next(2) + 1) * frame.Height;
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, Projectile.ai[0] / 2f) - Main.screenPosition, new Rectangle(0, 0, glow.Width / 2, glow.Height), new Color(255, 220, 20, 0), Projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, glowScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}