using AQMod.Assets;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class ThunderClap : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft = 30;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 50;

            var aQProjectile = projectile.GetGlobalProjectile<AQProjectile>();
            aQProjectile.canFreeze = false;
            aQProjectile.temperature = 20;
        }

        public override void AI()
        {
            var center = projectile.Center;
            float minimumLength = 280f;
            if ((int)projectile.ai[0] == 0)
            {
                List<Rectangle> validNPCHits = new List<Rectangle>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && !Main.npc[i].dontTakeDamage)
                    {
                        validNPCHits.Add(Main.npc[i].getRect());
                    }
                }
                float halfHeight = projectile.height / 2f;
                int x = (int)(projectile.position.X + projectile.width / 2f) / 16;
                for (float length = minimumLength; length < 1500f; length += 16f)
                {
                    bool end = false;
                    foreach (var n in validNPCHits)
                    {
                        projectile.ai[0] = length + n.Height;
                        if (projectile.Colliding(
                            new Rectangle((int)projectile.position.X, (int)(projectile.position.Y + length), projectile.width, projectile.height), 
                            n))
                        {
                            end = true;
                            projectile.ai[0] = length + n.Height;
                            break;
                        }
                    }
                    if (end)
                    {
                        break;
                    }
                    int y = (int)(projectile.position.Y + halfHeight + length) / 16;
                    projectile.ai[0] = y * 16f - projectile.position.Y;
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                        continue;
                    }
                    if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type])
                    {
                        break;
                    }
                }
            }

            DelegateMethods.v3_1 = new Vector3(0.8f, 0.5f, 0.1f);
            Utils.PlotTileLine(center, center + new Vector2(0f, projectile.ai[0]), 1f, DelegateMethods.CastLight);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var center = projectile.Center;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center,
                center + new Vector2(0f, projectile.ai[0]), 22, ref point);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 800);
            target.GetGlobalNPC<AQNPC>().ChangeTemperature(target, 60);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var drawPosition = projectile.Center;
            var scale = new Vector2(projectile.scale, projectile.scale);
            scale.X -= 1f - projectile.timeLeft / 80f;
            lightColor = projectile.GetAlpha(lightColor);
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, 0);
            var orig = new Vector2(texture.Width / 2f, frame.Height - 4f);

            int separation = frame.Height - 6;
            var glow = TextureCache.Lights[LightTex.Spotlight240x66];
            var glowScale = new Vector2(projectile.ai[0] / glow.Width * 2f, scale.X * 2f);
            var thunderGlowOrig = new Vector2(glow.Width / 4f, glow.Height / 2f);
            var glowBright = new Color(200, 200, 50);
            var glowDark = new Color(80, 80, 20, 0);

            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, projectile.ai[0] / 2f) - Main.screenPosition, new Rectangle(0, 0, glow.Width / 2, glow.Height), new Color(100, 100, 25, 0), projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, new Vector2(glowScale.X, glowScale.Y * 1.5f), SpriteEffects.None, 0f);

            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                float electric = 2f + ((float)Math.Sin(Main.GlobalTime * 5f) + 1f) * 2f;
                var clr = new Color(255, 100, 0, 20);
                for (int i = 0; i < 8; i++)
                {
                    float length2 = projectile.ai[0];
                    var off = new Vector2(electric, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    while (true)
                    {
                        var position = drawPosition + new Vector2(0f, length2 - separation) + off;
                        length2 -= separation;
                        if (length2 < separation)
                        {
                            frame.Y = 1 * frame.Height;
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            frame.Y = 0;
                            Main.spriteBatch.Draw(texture, drawPosition + off - Main.screenPosition, frame, clr, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            frame.Y = frame.Height * 3;
                            Main.spriteBatch.Draw(texture, drawPosition + off + new Vector2(0f, projectile.ai[0]) - Main.screenPosition, frame, clr, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                            break;
                        }
                        else
                        {
                            frame.Y = (Main.rand.Next(2) + 1) * frame.Height;
                            Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            float length = projectile.ai[0];

            while (true)
            {
                var position = drawPosition + new Vector2(0f, length - separation);
                length -= separation;
                if (length < separation)
                {
                    var glow2 = TextureCache.Lights[LightTex.Spotlight66x66];
                    var glow2Orig = glow2.Size() / 2f;
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, -frame.Height / 2f) - Main.screenPosition, null, glowBright, projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);
                    frame.Y = 1 * frame.Height;
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    frame.Y = 0;
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    frame.Y = frame.Height * 3;
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowDark, projectile.rotation, glow2Orig, scale * 3f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition + new Vector2(0f, projectile.ai[0]) - Main.screenPosition, frame, lightColor, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowBright, projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);
                    break;
                }
                else
                {
                    frame.Y = (Main.rand.Next(2) + 1) * frame.Height;
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, projectile.ai[0] / 2f) - Main.screenPosition, new Rectangle(0, 0, glow.Width / 2, glow.Height), new Color(255, 255, 75, 0), projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, glowScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}