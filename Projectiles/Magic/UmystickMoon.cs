using AQMod;
using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class UmystickMoon : ModProjectile
    {
        private Color _glowClr;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.scale = 0.75f;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1f;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                _glowClr = new Color(128, 70, 70, 0);
                if (projectile.frame == 1)
                    _glowClr = new Color(90, 128, 50, 0);
                else if (projectile.frame == 2)
                    _glowClr = new Color(70, 70, 128, 0);
            }
            projectile.rotation += projectile.velocity.Length() * 0.0157f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var origin = frame.Size() / 2f;
            var center = projectile.Center;
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
                    var trail = new Trailshader(TextureCache.Trails[TrailTex.ThickLine], Trailshader.TextureTrail);
                    trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(14f - p * 14f) * projectile.scale, (p) => _glowClr * (1f - p));
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
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(100, 100, 100, 0) * progress, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                var glow = TextureCache.Lights[LightTex.Spotlight66x66];
                Main.spriteBatch.Draw(glow, center - Main.screenPosition, null, _glowClr, projectile.rotation, glow.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 250, 250, 160), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            float size = projectile.width / 2f;
            if (Main.netMode != NetmodeID.Server)
            {
                AQSoundPlayer.PlaySound(SoundType.Item, "Sounds/Item/MysticUmbrellaDestroy_" + Main.rand.Next(4), 0.5f, 0.5f);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Content.Dusts.MonoDust>());
                var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = _glowClr * Main.rand.NextFloat(0.8f, 2.5f);
            }
        }
    }
}