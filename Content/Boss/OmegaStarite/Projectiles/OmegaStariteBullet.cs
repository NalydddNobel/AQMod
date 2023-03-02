using Aequus.Biomes;
using Aequus.Common;
using Aequus.Common.Primitives;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.OmegaStarite.Projectiles
{
    public class OmegaStariteBullet : ModProjectile
    {
        public static Asset<Texture2D> Particle;
        public static SpriteInfo ParticleSpriteInfo;
        private bool _init;

        private TrailRenderer prim;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Particle = ModContent.Request<Texture2D>($"{Texture}Particle", AssetRequestMode.ImmediateLoad);
                ParticleSpriteInfo = new SpriteInfo(Particle, 1, 1, 0.5f, 0.5f);
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            PushableEntities.AddProj(Type);
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
            _init = false;
            prim = null;
        }

        public override void AI()
        {
            if (!_init)
            {
                ParticleSystem.New<OmegaStariteBulletFlashParticle>(ParticleLayer.AboveDust).Setup(
                    Projectile.Center, 
                    Projectile.velocity * 2f, 
                    GlimmerBiome.CosmicEnergyColor with { G = 80, A = 0 }, 
                    Color.Blue with { R = 100, G = 100, A = 0 } * 0.33f, 
                    Main.rand.NextFloat(0.4f, 0.5f), 
                    0.2f, Projectile.velocity.ToRotation());
                _init = true;
            }
            if (Projectile.ai[0] < 0f)
            {
                var target = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];

                Projectile.velocity = Utils.SafeNormalize(
                    Vector2.Lerp(
                        Projectile.velocity, 
                        Vector2.Normalize(target.Center - Projectile.Center) * Projectile.ai[1], 
                        0.015f), 
                    Vector2.One) 
                    * Projectile.velocity.Length();

                Projectile.ai[0]++;
            }
            Projectile.rotation += 0.0314f;
            if (Main.GameUpdateCount % 6 == 0)
            {
                var d = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(20f, 20f), 
                    ModContent.DustType<MonoDust>(),
                    Projectile.velocity * 0.1f,
                    newColor: GlimmerBiome.CosmicEnergyColor with { R = 150, A= 0, }, 
                    Scale: 1.25f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (prim == null)
            {
                prim = new TrailRenderer(Textures.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(Projectile.width - p * Projectile.width), (p) => Color.Lerp(new Color(200, 10, 255, 0), Color.Blue, p) * (1f - p), drawOffset: new Vector2(Projectile.width / 2f, Projectile.height / 2f));
            }

            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = GlimmerBiome.CosmicEnergyColor with { A = 255, };
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            prim.Draw(Projectile.oldPos);
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - Projectile.Center).Length();
            if (playerDistance < 480f)
                intensity = 1f - playerDistance / 480f;

            Main.spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, orig, Projectile.scale * (1.05f + 0.2f * intensity), SpriteEffects.None, 0f);

            if (intensity > 0f)
            {
                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var spotlight = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var spotlightOrig = spotlight.Size() / 2f;

                Main.spriteBatch.Draw(
                    Textures.Bloom[0].Value,
                    drawPos,
                    null,
                    Color.Blue with { A = 0, } * 0.66f,
                    Projectile.rotation,
                    Textures.Bloom[0].Value.Size() / 2f,
                    Projectile.scale * intensity, SpriteEffects.None, 0f);

                spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.66f * intensity, (2.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.3f) * intensity);

                var spotlightDrawColor = drawColor with { A = 0, } * 0.5f;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float veloRot = Projectile.velocity.ToRotation();
            var velo = Projectile.velocity * 0.5f;
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(Projectile.Center + new Vector2(6f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerBiome.CosmicEnergyColor, 0.75f);
                Main.dust[d].velocity = velo;
            }
        }
    }

    public class OmegaStariteBulletFlashParticle : BaseBloomParticle<OmegaStariteBulletFlashParticle>
    {
        public int flash;

        public override OmegaStariteBulletFlashParticle CreateInstance()
        {
            return new OmegaStariteBulletFlashParticle();
        }

        protected override void SetDefaults()
        {
            SetTexture(OmegaStariteBullet.ParticleSpriteInfo, 1);
            bloomTexture = Textures.Bloom[0].Value;
            bloomOrigin = Textures.Bloom[0].Value.Size() / 2f;
            flash = 0;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            flash++;
            Rotation += Velocity.Length() * 0.02f;
            Velocity *= 0.99f;
            if (flash < 10)
            {
                Scale *= 1.3f;
            }
            else
            {
                Scale *= 0.88f;
            }
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            float globalScale = 0.75f;
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale * 0.2f * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color * MathF.Pow(Scale, 3f) * 0.75f, 0f, origin, MathF.Pow(Scale, 1.5f) * 0.1f * globalScale, SpriteEffects.None, 0f);
            spritebatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor * Scale, Rotation, bloomOrigin, Scale * BloomScale * globalScale, SpriteEffects.None, 0f);
        }
    }
}