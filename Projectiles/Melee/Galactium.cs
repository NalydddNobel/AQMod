using AQMod.Assets;
using AQMod.Assets.Effects.Trails;
using AQMod.Common;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class Galactium : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            var center = Projectile.Center;
            int target = -1;
            float dist = 400f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    var difference = npc.Center - center;
                    float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, Projectile.position, Projectile.width, Projectile.height))
                        c *= 2; // enemies behind walls need to be 2x closer in order to be targeted
                    if (c < dist)
                    {
                        target = i;
                        dist = c;
                    }
                }
            }
            if (Projectile.ai[1] == 0f)
                Projectile.ai[1] = Projectile.velocity.Length();
            if (target != -1)
            {
                Projectile.timeLeft += 2;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * Projectile.ai[1], 0.02f);
            }
            Projectile.ai[0]++;
            Projectile.rotation += 0.0628f;
            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color(Projectile.timeLeft) * (0.1f + (1f - dist / 400f) * 0.9f) * ClientOptions.Instance.FXIntensity * ((255 - Projectile.localAI[0]) / 255f), 1.25f);
                Main.dust[d].velocity = Projectile.velocity * 0.1f;
            }
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.velocity *= 0.98f;
                Projectile.localAI[0] += Main.rand.NextFloat(2f, 5f);
            }
            if (Projectile.localAI[0] >= 255f)
                Projectile.active = false;
        }

        private static Color color(float time)
        {
            return Color.Lerp(new Color(210, 245, 40, 10), new Color(255, 100, 40, 0), ((float)Math.Sin(time * 0.25f) + 1f) / 2f) * ClientOptions.Instance.FXIntensity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = color(Projectile.timeLeft);
            drawColor.A = 0;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var center = Projectile.Center;
            int target = -1;
            float dist = 400f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    var difference = npc.Center - center;
                    float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                    if (Collision.CanHitLine(npc.position, npc.width, npc.height, Projectile.position, Projectile.width, Projectile.height))
                        c *= 2;
                    if (c < dist)
                    {
                        target = i;
                        dist = c;
                    }
                }
            }
            float intensity = (0.1f + (1f - dist / 400f) * 0.9f) * ClientOptions.Instance.FXIntensity * ((255 - Projectile.localAI[0]) / 255f);
            var trueOldPos = new List<Vector2>();
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == new Vector2(0f, 0f))
                    break;
                trueOldPos.Add(Projectile.oldPos[i] + offset - Main.screenPosition);
            }
            if (trueOldPos.Count > 1)
            {
                AQVertexStrip.ReversedGravity(trueOldPos);
                var trail = new AQVertexStrip(TextureAssets.Extra[ExtrasID.RainbowRodTrailShape].Value, AQVertexStrip.TextureTrail);
                trail.PrepareVertices(trueOldPos.ToArray(), (p) => new Vector2(20 - p * 20) * (1f + intensity), (p) => color(Projectile.timeLeft - p) * 0.5f * (1f - p));
                trail.Draw();
            }
            if (intensity > 0f)
            {
                var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.3f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * intensity, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.1f * intensity, Projectile.rotation, spotlightOrig, Projectile.scale * 1.25f * intensity, SpriteEffects.None, 0f);
                spotlight = AQTextures.Lights[LightTex.Spotlight240x66];
                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.1f * intensity, (2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f)) * intensity);
                var spotlightDrawColor = drawColor * intensity;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);

                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);

                float veloRot = Projectile.velocity.ToRotation();
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * intensity, veloRot, spotlightOrig, crossScale * 0.8f, SpriteEffects.None, 0f);

                for (int i = 0; i < 8; i++)
                {
                    var normal = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    var clr = color(Projectile.timeLeft + i * 0.0157f) * intensity;
                    Main.spriteBatch.Draw(texture, drawPos + normal, null, clr, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0) * intensity, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = Projectile.velocity.ToRotation();
            var velo = Projectile.velocity * 0.5f;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color(Projectile.timeLeft), 1.5f);
                Main.dust[d].velocity = Vector2.Lerp(Main.dust[d].velocity, Projectile.velocity, 0.5f);
            }
        }
    }
}