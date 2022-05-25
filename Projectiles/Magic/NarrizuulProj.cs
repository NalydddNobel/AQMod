using Aequus.Graphics;
using Aequus.Graphics.Prims;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class NarrizuulProj : ModProjectile
    {
        public static Asset<Texture2D> GlowmaskTexture { get; private set; }

        private PrimRenderer prim;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                GlowmaskTexture = ModContent.Request<Texture2D>(this.GetPath() + "_Glow");
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void Unload()
        {
            GlowmaskTexture = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 80;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextFloat(0.77f, 3.33f);
                Projectile.localAI[1] = Main.rand.NextFloat(0, 120);
            }
            Projectile.localAI[1] += Main.rand.NextFloat(0.01f, 0.1f);
            if (Projectile.ai[1] < 6)
            {
                if (Projectile.ai[0] == 0)
                    Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.localAI[0]) * Projectile.ai[0]);
                Projectile.ai[1]++;
            }
            else
            {
                var target = Projectile.FindTargetWithinRange(1000f);
                if (target != null)
                {
                    if (Vector2.Distance(Projectile.Center, target.Center).Abs() > target.width)
                    {
                        Projectile.timeLeft += 2;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * 22.77f, 0.1f);
                    }
                }
                if (Projectile.ai[1] < 15)
                {
                    if (Projectile.ai[0] == 0)
                        Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.localAI[0]) * Projectile.ai[0]);
                    Projectile.ai[1]++;
                }
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MonoDust>(), Projectile.velocity * -0.05f, 0, NarrizuulRainbow(Projectile.localAI[1]) * 1.5f);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, NarrizuulRainbow(Projectile.localAI[1]) * 0.3f);
                Main.dust[d].velocity = Projectile.velocity * 0.125f;
            }
            Lighting.AddLight(Projectile.Center, (NarrizuulRainbow(Projectile.localAI[1]) * 1.5f).ToVector3() * Projectile.scale);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = GlowmaskTexture.Value;
            Vector2 origin = glow.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            float colorMult = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            if (prim == null)
            {
                prim = new PrimRenderer(Images.Trail[0].Value, PrimRenderer.DefaultPass,
                    (p) => new Vector2(20f - p * 20f),
                    (p) => NarrizuulRainbow(Projectile.localAI[1]) * 3 * (0.65f + (float)(Math.Sin(Main.GlobalTimeWrappedHourly + p * 20f) * 0.1f)) * (1f - p),
                    drawOffset: Projectile.Size / 2f);
            }
            prim.Draw(Projectile.oldPos);
            glow = Images.Bloom[0].Value;
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, NarrizuulRainbow(Projectile.localAI[1]).UseA(0) * 0.5f, Projectile.rotation, glow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            glow = texture;
            var drawPos = Projectile.Center - Main.screenPosition;
            var orig = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(250, 250, 250, 20), Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0f);
            var clr = Color.Lerp(new Color(250, 250, 250, 20), NarrizuulRainbow(Projectile.localAI[1]), 0.5f);
            float scale1 = Projectile.scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.4f;
            Main.spriteBatch.Draw(glow, drawPos, null, clr * 0.5f, Projectile.rotation, orig, scale1 + 0.2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, drawPos, null, clr * 0.32f, Projectile.rotation + MathHelper.PiOver4, orig, scale1 + 0.1f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(glow, drawPos, null, clr * 0.2f, Projectile.rotation, orig, (scale1 + 0.2f) * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, drawPos, null, clr * 0.1f, Projectile.rotation + MathHelper.PiOver4, orig, (scale1 + 0.1f) * 1.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float distance = Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center);
                if (distance < 800)
                {
                    EffectsSystem.Shake.Set((int)(800f - distance) / 32);
                }
            }
            Color color = NarrizuulRainbow(Projectile.localAI[1]) * 1.5f;
            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>());
                Main.dust[d].scale *= Main.rand.NextFloat(1.1f, 1.7f);
                Main.dust[d].color = color * Main.dust[d].scale;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(3f, 7.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<MonoSparkleDust>());
                Main.dust[d].scale *= Main.rand.NextFloat(0.8f, 1.4f);
                Main.dust[d].color = NarrizuulRainbow(Projectile.localAI[1] + i * 0.1f) * 1.5f * Main.dust[d].scale;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(3f, 7.5f), 0).RotatedByRandom(MathHelper.TwoPi);
                Main.dust[d].alpha = Main.rand.Next(30);
            }
            if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) < Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight))
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
        }

        public static Color NarrizuulRainbow(float position) => AequusHelpers.LerpBetween(new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Violet, Color.Magenta, }, position);
    }
}