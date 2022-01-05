using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class Narrizuul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = Main.rand.NextFloat(0.77f, 3.33f);
                projectile.localAI[1] = Main.rand.NextFloat(0, 120);
            }
            projectile.localAI[1] += Main.rand.NextFloat(0.01f, 0.1f);
            if (projectile.ai[1] < 6)
            {
                if (projectile.ai[0] == 0)
                    projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(projectile.localAI[0]) * projectile.ai[0]);
                projectile.ai[1]++;
            }
            else
            {
                int targetIndex = AQNPC.FindTarget(projectile.Center, 1000f);
                if (targetIndex != -1)
                {
                    NPC target = Main.npc[targetIndex];
                    if (Vector2.Distance(projectile.Center, target.Center).Abs() > target.width)
                    {
                        projectile.timeLeft += 2;
                        projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(target.Center - projectile.Center) * 22.77f, 0.1f);
                    }
                }
                if (projectile.ai[1] < 15)
                {
                    if (projectile.ai[0] == 0)
                        projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                    projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(projectile.localAI[0]) * projectile.ai[0]);
                    projectile.ai[1]++;
                }
            }
            if (Main.rand.NextBool(4))
            {
                SpawnPatterns.SpawnDustCentered(projectile.Center, ModContent.DustType<NarrenBoltDust>(), new Vector2(0f, 0f), NarrizuulRainbow(projectile.localAI[1]) * 1.5f);
            }
            Lighting.AddLight(projectile.Center, (NarrizuulRainbow(projectile.localAI[1]) * 1.5f).ToVector3() * projectile.scale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = ModContent.GetTexture(this.GetPath("_Glow"));
            Vector2 origin = glow.Size() / 2f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            float colorMult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            if (VertexStrip.ShouldDrawVertexTrails(VertexStrip.GetVertexDrawingContext_Projectile(projectile)))
            {
                var trueOldPos = new List<Vector2>();
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == Vector2.Zero)
                        break;
                    trueOldPos.Add(ScreenShakeManager.UpsideDownScreenSupport(projectile.oldPos[i] + offset - Main.screenPosition));
                }
                if (trueOldPos.Count > 1)
                {
                    var trail = new VertexStrip(AQTextures.Trails[TrailTex.ThickLine], VertexStrip.TextureTrail);
                    var clr2 = NarrizuulRainbow(projectile.localAI[1]) * 3;
                    trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(20f - p * 20f), (p) => clr2 * (0.65f + (float)(Math.Sin(Main.GlobalTime + p * 20f) * 0.1f)) * (1f - p));
                    trail.Draw();
                }
            }
            else
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    var clr2 = NarrizuulRainbow(projectile.localAI[1] + i * 0.1f) * 0.225f;
                    var orig2 = Main.projectileTexture[projectile.type].Size() / 2f;
                    Main.spriteBatch.Draw(glow, projectile.oldPos[i] + offset / 2f - Main.screenPosition, null, clr2 * progress, projectile.rotation, orig2, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            glow = AQTextures.Lights[LightTex.Spotlight66x66];
            spriteBatch.Draw(glow, projectile.Center - Main.screenPosition, null, NarrizuulRainbow(projectile.localAI[1]) * 0.5f, projectile.rotation, glow.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            glow = Main.projectileTexture[projectile.type];
            var drawPos = projectile.Center - Main.screenPosition;
            var orig = Main.projectileTexture[projectile.type].Size() / 2f;
            spriteBatch.Draw(this.GetTexture(), drawPos, null, new Color(250, 250, 250, 20), projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            var clr = Color.Lerp(new Color(250, 250, 250, 20), NarrizuulRainbow(projectile.localAI[1]), 0.5f);
            float scale1 = projectile.scale + (float)Math.Sin(Main.GlobalTime * 5f) * 0.4f;
            spriteBatch.Draw(glow, drawPos, null, clr * 0.5f, projectile.rotation, orig, scale1 + 0.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, drawPos, null, clr * 0.32f, projectile.rotation + MathHelper.PiOver4, orig, scale1 + 0.1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(glow, drawPos, null, clr * 0.2f, projectile.rotation, orig, (scale1 + 0.2f) * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, drawPos, null, clr * 0.1f, projectile.rotation + MathHelper.PiOver4, orig, (scale1 + 0.1f) * 1.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner && AQConfigClient.c_TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 800)
                    ScreenShakeManager.AddShake(new BasicScreenShake(8, AQGraphics.MultIntensity((int)(800f - distance) / 128)));
            }
            Color color = NarrizuulRainbow(projectile.localAI[1]) * 1.5f;
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<NarrenBoltDust>());
                Main.dust[d].scale *= Main.rand.NextFloat(1.1f, 1.7f);
                Main.dust[d].color = color;
                Main.dust[d].velocity = new Vector2(6, 0).RotatedByRandom(MathHelper.TwoPi);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.Center, 0, 0, ModContent.DustType<NarrenBoltDust>());
                Main.dust[d].scale *= Main.rand.NextFloat(1.1f, 1.7f);
                Main.dust[d].color = NarrizuulRainbow(projectile.localAI[1] + i * 0.1f) * 1.5f;
                Main.dust[d].velocity = new Vector2(6, 0).RotatedByRandom(MathHelper.TwoPi);
            }
            if (Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) < Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight))
                Main.PlaySound(SoundID.Item14, projectile.Center);
        }

        public static Color NarrizuulRainbow(float position) => AQUtils.LerpColors(new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Violet, Color.Magenta, }, position);
    }
}