using Aequus.Assets.Effects.Prims;
using Aequus.Common.Configuration;
using Aequus.Content.Invasions;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Boss
{
    public class OmegaStariteBullet : ModProjectile
    {
        private LegacyPrimRenderer prim;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0f)
            {
                byte plr = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[plr].Center - Projectile.Center) * Projectile.ai[1], 0.015f);
                Projectile.ai[0]++;
            }
            Projectile.rotation += 0.0314f;
            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(Projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + Projectile.velocity.ToRotation()), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Glimmer.CosmicEnergyColor, 0.75f);
                Main.dust[d].velocity = Projectile.velocity * 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = Glimmer.CosmicEnergyColor;
            drawColor.A = 0;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            if (LegacyPrimRenderer.renderProjTrails)
            {
                if (prim == null)
                {
                    prim = new LegacyPrimRenderer(Aequus.MyTex("Assets/Effects/Prims/ThickerLine"), LegacyPrimRenderer.DefaultPass, (p) => new Vector2(Projectile.width - p * Projectile.width), (p) => drawColor * (1f - p), drawOffset: new Vector2(Projectile.width / 2f, Projectile.height / 2f));
                }
                prim.Draw(Projectile.oldPos);
            }
            else
            {
                int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
                for (int i = 0; i < trailLength; i++)
                {
                    if (Projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, drawColor * progress, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - Projectile.Center).Length();
            if (playerDistance < 480f)
                intensity = 1f - playerDistance / 480f;
            intensity *= ClientConfiguration.Instance.effectIntensity;
            if (intensity > 0f)
            {
                var spotlight = Aequus.MyTex("Assets/Bloom");
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.25f, Projectile.rotation, spotlightOrig, Projectile.scale * intensity, SpriteEffects.None, 0f);
                spotlight = Aequus.MyTex("Assets/BloomWide");
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.04f * intensity, (3f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 16f) * 0.2f) * intensity);
                var spotlightDrawColor = drawColor * 0.2f;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                crossScale.X *= 2f;
                crossScale.Y *= 1.5f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 255), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = Projectile.velocity.ToRotation();
            var velo = Projectile.velocity * 0.5f;
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(Projectile.Center + new Vector2(6f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Glimmer.CosmicEnergyColor, 0.75f);
                Main.dust[d].velocity = velo;
            }
        }
    }
}