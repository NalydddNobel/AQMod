using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.Trails.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class Memorialist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 19;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            int target = AQNPC.FindTarget(projectile.Center, 600f);
            if (target != -1)
            {
                float l = projectile.velocity.Length();
                projectile.velocity = Vector2.Normalize(Main.npc[target].Center - projectile.Center) * l;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var origin = frame.Size() / 2f;
            var center = projectile.Center;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            if (PrimitivesRenderer.ShouldDrawVertexTrails(PrimitivesRenderer.GetVertexDrawingContext_Projectile(projectile)))
            {
                var trueOldPos = new List<Vector2>();
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    trueOldPos.Add(FX.FlippedScreenCheck(projectile.oldPos[i] + offset - Main.screenPosition));
                }
                if (trueOldPos.Count > 1)
                {
                    var trail = new PrimitivesRenderer(AQTextures.Trails[TrailTex.ThickLine], PrimitivesRenderer.TextureTrail);
                    trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(14f - p * 14f) * projectile.scale,
                        (p) => Color.Lerp(new Color(255, 255, 50, 20), new Color(255, 10, 10, 0), p) * (1f - p));
                    trail.Draw();
                }
            }
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 200 + (int)AQUtils.Wave(Main.GlobalTime * 10f, -15, 15), 200, 160), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                var glow = AQTextures.Lights[LightTex.Spotlight66x66];
                var spotlightOrigin = glow.Size() / 2f;
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    Main.spriteBatch.Draw(glow, projectile.oldPos[i] + offset - Main.screenPosition, null,
                        Color.Lerp(new Color(255, 255, 50, 20), new Color(100, 10, 10, 0), 1f - progress) * progress, projectile.rotation, spotlightOrigin, projectile.scale * 0.7f * progress, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.Draw(glow, center - Main.screenPosition, null, new Color(222, 150, 0, 0), projectile.rotation, spotlightOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            float size = projectile.width / 2f;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.PlaySound(SoundID.Item100, projectile.Center);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
                var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = new Color(255, 110, 20, 0);
            }
            for (float f = 0; f < 1f; f += 0.25f)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
                var n = (f + Main.rand.NextFloat(-0.1f, 0.1f)).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = new Color(240, 200, 30, 0);
            }
        }
    }
}