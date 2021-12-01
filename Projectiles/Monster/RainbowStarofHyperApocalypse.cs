using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Content.Dusts;
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

namespace AQMod.Projectiles.Monster
{
    public class RainbowStarofHyperApocalypse : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 3600;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            byte plr = Player.FindClosest(projectile.position, projectile.width, projectile.height);
            if (projectile.ai[1] == 0f)
                projectile.ai[1] = projectile.velocity.Length();
            projectile.velocity = Vector2.Lerp(projectile.velocity,
            Vector2.Normalize(Main.player[plr].Center - projectile.Center) * projectile.ai[1], 0.02f);
            projectile.ai[0]++;
            projectile.rotation += 0.0628f;
            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + projectile.velocity.ToRotation()), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, getColor(Main.GlobalTime), 0.75f);
                Main.dust[d].velocity = projectile.velocity * 0.1f;
            }
        }

        private static readonly Color[] pattern = new Color[]
        {
            new Color(255, 0, 0, 128),
            new Color(255, 128, 0, 128),
            new Color(255, 255, 0, 128),
            new Color(0, 255, 10, 128),
            new Color(0, 128, 128, 128),
            new Color(0, 10, 255, 128),
            new Color(128, 0, 128, 128),
        };

        private static Color getColor(float time)
        {
            var config = ModContent.GetInstance<AQConfigClient>();
            return AQUtils.colorLerps(pattern, time * 3f * config.EffectIntensity) * config.EffectIntensity;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = getColor(Main.GlobalTime);
            drawColor.A = 0;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - projectile.Center).Length();
            if (playerDistance < 1200f)
                intensity = 1f - playerDistance / 1200f;
            intensity *= ModContent.GetInstance<AQConfigClient>().EffectIntensity;
            if (Trailshader.ShouldDrawVertexTrails(Trailshader.GetVertexDrawingContext_Projectile(projectile)))
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
                    for (int i = 0; i < 4; i++)
                    {
                        var trail = new Trailshader(TextureCache.Trails[TrailTex.Line], Trailshader.TextureTrail);
                        trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(20 - p * 20) * (1f + intensity * 2f), (p) => getColor(Main.GlobalTime + p) * 0.5f * (1f - p));
                        trail.Draw();
                    }
                    // amazing code
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
                    var trailClr = getColor(Main.GlobalTime + progress) * 0.5f;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, trailClr * progress, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            if (intensity > 0f)
            {
                var spotlight = TextureCache.Lights[LightTex.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.8f * intensity, projectile.rotation, spotlightOrig, projectile.scale * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.5f * intensity, projectile.rotation, spotlightOrig, projectile.scale * 2.5f * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.3f * intensity, projectile.rotation, spotlightOrig, projectile.scale * 6f * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.1f * intensity, projectile.rotation, spotlightOrig, projectile.scale * 10f * intensity, SpriteEffects.None, 0f);
                spotlight = TextureCache.Lights[LightTex.Spotlight240x66];
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.08f * intensity, (5f + (float)Math.Sin(Main.GlobalTime * 20f) * 0.5f) * intensity);
                var spotlightDrawColor = drawColor * intensity;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);

                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);

                crossScale.X *= 2f;
                crossScale.Y *= 1.5f;

                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.1f, MathHelper.PiOver4, spotlightOrig, crossScale * 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.1f, MathHelper.PiOver4 * 3f, spotlightOrig, crossScale * 0.5f, SpriteEffects.None, 0f);

                float veloRot = projectile.velocity.ToRotation();
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * intensity, veloRot, spotlightOrig, crossScale * 0.45f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.5f * intensity, veloRot, spotlightOrig, crossScale * 0.6f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.15f * intensity, veloRot, spotlightOrig, crossScale * 0.8f, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.1f, 0f, spotlightOrig, crossScale * 2.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.1f, MathHelper.PiOver2, spotlightOrig, crossScale * 2.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.05f, MathHelper.PiOver4, spotlightOrig, crossScale * 1.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.05f, MathHelper.PiOver4 * 3f, spotlightOrig, crossScale * 1.5f, SpriteEffects.None, 0f);

                for (int i = 0; i < 8; i++)
                {
                    var normal = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    var clr = getColor(Main.GlobalTime + i * 0.0157f) * intensity;
                    Main.spriteBatch.Draw(texture, drawPos + normal, null, clr, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = projectile.velocity.ToRotation();
            var velo = projectile.velocity * 0.5f;
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, getColor(Main.GlobalTime), 0.75f);
                Main.dust[d].velocity = velo;
            }
        }
    }
}