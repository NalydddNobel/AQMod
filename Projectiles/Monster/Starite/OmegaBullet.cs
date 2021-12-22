using AQMod.Assets;
using AQMod.Content.WorldEvents.GlimmerEvent;
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

namespace AQMod.Projectiles.Monster.Starite
{
    public class OmegaBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 360;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0f)
            {
                byte plr = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.player[plr].Center - projectile.Center) * projectile.ai[1], 0.015f);
                projectile.ai[0]++;
            }
            projectile.rotation += 0.0314f;
            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + projectile.velocity.ToRotation()), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerEvent.stariteProjectileColor, 0.75f);
                Main.dust[d].velocity = projectile.velocity * 0.1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = GlimmerEvent.stariteProjectileColor;
            drawColor.A = 0;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
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
                    VertexStrip.FullDraw(AQTextures.Trails[TrailTex.Line], VertexStrip.TextureTrail,
                        trueOldPos.ToArray(), (p) => new Vector2(projectile.width - p * projectile.width), (p) => drawColor * (1f - p));
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
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, drawColor * progress, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - projectile.Center).Length();
            if (playerDistance < 480f)
                intensity = 1f - playerDistance / 480f;
            intensity *= ModContent.GetInstance<AQConfigClient>().EffectIntensity;
            if (intensity > 0f)
            {
                var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.25f, projectile.rotation, spotlightOrig, projectile.scale * intensity, SpriteEffects.None, 0f);
                spotlight = AQTextures.Lights[LightTex.Spotlight240x66];
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.04f * intensity, (3f + (float)Math.Sin(Main.GlobalTime * 16f) * 0.2f) * intensity);
                var spotlightDrawColor = drawColor * 0.2f;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                crossScale.X *= 2f;
                crossScale.Y *= 1.5f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 255), projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = projectile.velocity.ToRotation();
            var velo = projectile.velocity * 0.5f;
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(projectile.Center + new Vector2(6f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerEvent.stariteProjectileColor, 0.75f);
                Main.dust[d].velocity = velo;
            }
        }
    }
}