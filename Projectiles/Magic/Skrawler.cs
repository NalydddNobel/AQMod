using AQMod.Dusts;
using AQMod.Effects.Trails;
using AQMod.Effects.Trails.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class Skrawler : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 200;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.extraUpdates = 20;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 2;
            height = 2;
            fallThrough = true;
            return true;
        }

        protected Func<float, Vector2> GetSizeMethod() => (p) => new Vector2(2f, 2f);
        protected Func<float, Color> GetColorMethod() => (p) => Color.Lerp(new Color(255, 230, 10, 250), new Color(255, 50, 10, 0), p) * (1f - p);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var textureOrig = new Vector2(texture.Width / 2f, 2f);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            if (PrimitivesRenderer.ShouldDrawVertexTrails(PrimitivesRenderer.GetVertexDrawingContext_Projectile(projectile)))
            {
                var renderingPositions = PrimitivesRenderer.GetValidRenderingPositions(projectile.oldPos, new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y));
                if (renderingPositions.Count > 1)
                {
                    PrimitivesRenderer.FullDraw(texture, PrimitivesRenderer.TextureTrail,
                        renderingPositions.ToArray(), GetSizeMethod(), GetColorMethod());
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
                    var trailClr = lightColor;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, trailClr * progress, projectile.rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var renderingPositions = PrimitivesRenderer.GetValidRenderingPositions(projectile.oldPos, new Vector2(projectile.width / 2f, projectile.height / 2f));
                if (renderingPositions.Count > 3)
                {
                    renderingPositions.RemoveAt(renderingPositions.Count - 1);
                    Trail.PreDrawProjectiles.NewTrail(new DeathTrail(Main.projectileTexture[projectile.type], PrimitivesRenderer.TextureTrail,
                    renderingPositions, GetSizeMethod(), GetColorMethod(), default, default, projectile.extraUpdates));
                }
            }
            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDust(projectile.Center, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(255, 0, 0, 0));
                Main.dust[d].velocity -= projectile.velocity;
                Main.dust[d].velocity *= 0.2f;
            }
        }
    }
}