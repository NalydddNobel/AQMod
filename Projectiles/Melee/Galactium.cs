using AQMod.Assets;
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

namespace AQMod.Projectiles.Melee
{
    public class Galactium : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 1200;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 3;
        }

        public override void AI()
        {
            var center = projectile.Center;
            int target = -1;
            float dist = 400f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    var difference = npc.Center - center;
                    float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, projectile.position, projectile.width, projectile.height))
                        c *= 2; // enemies behind walls need to be 2x closer in order to be targeted
                    if (c < dist)
                    {
                        target = i;
                        dist = c;
                    }
                }
            }
            if (projectile.ai[1] == 0f)
                projectile.ai[1] = projectile.velocity.Length();
            if (target != -1)
            {
                projectile.timeLeft += 2;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.npc[target].Center - projectile.Center) * projectile.ai[1], 0.02f);
            }
            projectile.ai[0]++;
            projectile.rotation += 0.0628f;
            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color(projectile.timeLeft) * (0.1f + (1f - dist / 400f) * 0.9f) * ModContent.GetInstance<AQConfigClient>().EffectIntensity * ((255 - projectile.localAI[0]) / 255f), 1.25f);
                Main.dust[d].velocity = projectile.velocity * 0.1f;
            }
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.velocity *= 0.98f;
                projectile.localAI[0] += Main.rand.NextFloat(2f, 5f);
            }
            if (projectile.localAI[0] >= 255f)
                projectile.active = false;
        }

        private static Color color(float time)
        {
            return Color.Lerp(new Color(210, 245, 40, 10), new Color(255, 100, 40, 0), ((float)Math.Sin(time * 0.25f) + 1f) / 2f) * ModContent.GetInstance<AQConfigClient>().EffectIntensity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = color(projectile.timeLeft);
            drawColor.A = 0;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var center = projectile.Center;
            int target = -1;
            float dist = 400f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    var difference = npc.Center - center;
                    float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, projectile.position, projectile.width, projectile.height))
                        c *= 2;
                    if (c < dist)
                    {
                        target = i;
                        dist = c;
                    }
                }
            }
            float intensity = (0.1f + (1f - dist / 400f) * 0.9f) * ModContent.GetInstance<AQConfigClient>().EffectIntensity * ((255 - projectile.localAI[0]) / 255f);
            if (VertexStrip.ShouldDrawVertexTrails(VertexStrip.GetVertexDrawingContext_Projectile(projectile)))
            {
                var trueOldPos = new List<Vector2>();
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    trueOldPos.Add(ScreenShakeManager.UpsideDownScreenSupport(projectile.oldPos[i] + offset - Main.screenPosition));
                }
                if (trueOldPos.Count > 1)
                {
                    var trail = new VertexStrip(AQTextures.Trails[TrailTex.Line], VertexStrip.TextureTrail);
                    trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(20 - p * 20) * (1f + intensity), (p) => color(projectile.timeLeft - p) * 0.5f * (1f - p));
                    trail.Draw();
                }
            }
            else
            {
                int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    var trailClr = color(projectile.timeLeft - progress) * 0.5f;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, trailClr * progress, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            if (intensity > 0f)
            {
                var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.3f * intensity, projectile.rotation, spotlightOrig, projectile.scale * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.1f * intensity, projectile.rotation, spotlightOrig, projectile.scale * 1.25f * intensity, SpriteEffects.None, 0f);
                spotlight = AQTextures.Lights[LightTex.Spotlight240x66];
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.1f * intensity, (2f + (float)Math.Sin(Main.GlobalTime * 10f)) * intensity);
                var spotlightDrawColor = drawColor * intensity;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);

                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);

                float veloRot = projectile.velocity.ToRotation();
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * intensity, veloRot, spotlightOrig, crossScale * 0.8f, SpriteEffects.None, 0f);

                for (int i = 0; i < 8; i++)
                {
                    var normal = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    var clr = color(projectile.timeLeft + i * 0.0157f) * intensity;
                    Main.spriteBatch.Draw(texture, drawPos + normal, null, clr, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0) * intensity, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = projectile.velocity.ToRotation();
            var velo = projectile.velocity * 0.5f;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color(projectile.timeLeft), 1.5f);
                Main.dust[d].velocity = Vector2.Lerp(Main.dust[d].velocity, projectile.velocity, 0.5f);
            }
        }
    }
}