using AQMod.Assets;
using AQMod.Content.Dusts;
using AQMod.Content.WorldEvents.GlimmerEvent;
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

namespace AQMod.Projectiles.Monster.GaleStreams
{
    public class SpaceSquidLaser : ModProjectile
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
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = new Color(30, 255, 30, 0);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
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
                    Trailshader.FullDraw(AQTextures.Trails[TrailTex.Line], Trailshader.TextureTrail,
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
    }
}