using Aequus.Graphics.Primitives;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.OmegaStariteProjs
{
    public class TrollStar : ModProjectile
    {
        private TrailRenderer prim;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            byte plr = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (Projectile.ai[1] == 0f)
                Projectile.ai[1] = Projectile.velocity.Length();
            Projectile.velocity = Vector2.Lerp(Projectile.velocity,
            Vector2.Normalize(Main.player[plr].Center - Projectile.Center) * Projectile.ai[1], 0.02f);
            Projectile.ai[0]++;
            Projectile.rotation += 0.0628f;
            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(Projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + Projectile.velocity.ToRotation()), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, getColor(Main.GlobalTimeWrappedHourly), 0.75f);
                Main.dust[d].velocity = Projectile.velocity * 0.1f;
            }
        }

        private static readonly ColorHelper.ColorGradient gradient = new ColorHelper.ColorGradient(3f,
            new Color(255, 0, 0, 128),
            new Color(255, 128, 0, 128),
            new Color(255, 255, 0, 128),
            new Color(0, 255, 10, 128),
            new Color(0, 128, 128, 128),
            new Color(0, 10, 255, 128),
            new Color(128, 0, 128, 128));

        private static Color getColor(float time)
        {
            return gradient.GetColor(time);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = getColor(Main.GlobalTimeWrappedHourly);
            drawColor.A = 0;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - Projectile.Center).Length();
            if (playerDistance < 1200f)
                intensity = 1f - playerDistance / 1200f;
            if (prim == null)
            {
                prim = new TrailRenderer(TextureCache.Trail[0].Value, TrailRenderer.DefaultPass,
                    (p) => new Vector2(20 - p * 20) * (1f + intensity * 2f), (p) => getColor(Main.GlobalTimeWrappedHourly + p) * 0.5f * (1f - p));
            }
            for (int i = 0; i < 4; i++)
                prim.Draw(Projectile.oldPos);
            if (intensity > 0f)
            {
                var spotlight = TextureCache.Bloom[0].Value;
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.8f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.5f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * 2.5f * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.3f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * 6f * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.1f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * 10f * intensity, SpriteEffects.None, 0f);
                spotlight = TextureCache.Bloom[4].Value;
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.08f * intensity, (5f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f) * 0.5f) * intensity);
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

                float veloRot = Projectile.velocity.ToRotation();
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
                    var clr = getColor(Main.GlobalTimeWrappedHourly + i * 0.0157f) * intensity;
                    Main.spriteBatch.Draw(texture, drawPos + normal, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = Projectile.velocity.ToRotation();
            var velo = Projectile.velocity * 0.5f;
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(Projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, getColor(Main.GlobalTimeWrappedHourly), 0.75f);
                Main.dust[d].velocity = velo;
            }
        }
    }
}