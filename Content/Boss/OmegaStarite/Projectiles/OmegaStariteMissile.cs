using Aequus.Biomes;
using Aequus.Common.Primitives;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.OmegaStarite.Projectiles
{
    public class OmegaStariteMissile : ModProjectile
    {
        public const int TrackingTime = 60;

        public static Asset<Texture2D> MissileTarget;

        public int targetPlayer;
        public Vector2 targetLocation;

        private TrailRenderer prim;
        private bool _init;

        public int TargetPlayer { get => targetPlayer - 1; set => targetPlayer = value + 1; }
        public int NPCHost { get => (int)Projectile.ai[1]; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                MissileTarget = ModContent.Request<Texture2D>($"{Texture}Target");
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void Unload()
        {
            MissileTarget = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 250;
            _init = false;
            prim = null;
        }

        private void ChooseTarget()
        {
            TargetPlayer = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (TargetPlayer == 0 && Main.player[TargetPlayer].dead)
            {
                Projectile.Kill();
            }
        }

        private void AI_TrackPlayer()
        {
            if (!Main.npc[NPCHost].active)
            {
                Projectile.Kill();
                return;
            }
            if (targetPlayer <= 0)
            {
                ChooseTarget();
                Projectile.ai[0] = -40f;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.ai[0]++;
                targetLocation = Main.player[TargetPlayer].Center;
            }
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.ai[0] > TrackingTime)
            {
                Projectile.ai[0]++;
                if (!_init)
                {
                    ParticleSystem.New<OmegaStariteBulletFlashParticle>(ParticleLayer.AboveDust).Setup(
                        Projectile.Center,
                        Projectile.velocity * 70f,
                        GlimmerBiome.CosmicEnergyColor with { G = 80, A = 0 },
                        Color.Blue with { R = 100, G = 100, A = 0 } * 0.33f,
                        Main.rand.NextFloat(0.4f, 0.5f),
                        0.2f, Projectile.velocity.ToRotation());
                    _init = true;
                }
                float distance = Projectile.Distance(targetLocation);
                if (Projectile.velocity.Length() < 5f)
                {
                    Projectile.velocity *= 1.33f;
                }
                else
                {
                    float lerpValue = MathF.Pow(Projectile.ai[0] * 0.0004f, 2f);
                    if (distance < 400f)
                    {
                        lerpValue = MathHelper.Lerp(lerpValue, 1f, 1f - distance / 400f);
                    }
                    Projectile.velocity = Utils.SafeNormalize(Vector2.Lerp(Projectile.velocity, targetLocation - Projectile.Center, lerpValue), Vector2.One) * Math.Clamp(Projectile.velocity.Length() * 1.02f, 8f, 32f);
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (distance < 48f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    ScreenShake.SetShake(32f, where: Projectile.Center);
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity = Utils.SafeNormalize(Projectile.velocity, -Vector2.UnitY) * 0.1f;
                Projectile.Center = Main.npc[NPCHost].Center + Projectile.velocity * 600f;
                var d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.PurpleCrystalShard,
                    Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 40f,
                    Scale: 2f
                );
                d.noGravity = true;
                d.noLight = true;
                if (Projectile.ai[0] == (TrackingTime - 1))
                {
                    Projectile.netUpdate = true;
                }
                AI_TrackPlayer();
            }
        }

        private void DrawTrack()
        {
            if (targetPlayer <= 0)
                return;

            var player = Main.player[TargetPlayer];
            float totalScale = Math.Clamp(1f - Projectile.ai[0] / TrackingTime, 0.33f, 1f);
            for (int i = 0; i < 2; i++)
            {
                float scale = 1f - (Main.GlobalTimeWrappedHourly * 2.5f + i * 0.5f) % 1f;
                Main.spriteBatch.Draw(
                    MissileTarget.Value,
                    (targetLocation - Main.screenPosition).Floor(),
                    null,
                    (new Color(200, 40, 255, 0) * scale).UseB(255) * scale * Projectile.Opacity,
                    0f,
                    MissileTarget.Value.Size() / 2f,
                    3f * totalScale * scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            DrawTrack();

            if (Projectile.ai[0] < TrackingTime)
            {
                return false;
            }

            if (prim == null)
            {
                prim = new TrailRenderer(Textures.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(Projectile.width - p * Projectile.width), (p) => Color.Lerp(new Color(200, 10, 255, 0), Color.Blue, p) * (1f - p), drawOffset: new Vector2(Projectile.width / 2f, Projectile.height / 2f));
            }
            prim.Draw(Projectile.oldPos);

            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = GlimmerBiome.CosmicEnergyColor with { A = 255, };
            float intensity = 0f;
            float playerDistance = (Main.player[Main.myPlayer].Center - Projectile.Center).Length();
            if (playerDistance < 480f)
                intensity = 1f - playerDistance / 480f;

            Main.spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, orig, Projectile.scale * (1.05f + 0.2f * intensity), SpriteEffects.None, 0f);

            if (intensity > 0f)
            {
                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);

                Main.spriteBatch.Draw(
                    Textures.Bloom[0].Value,
                    drawPos,
                    null,
                    Color.Blue with { A = 0, } * 0.66f,
                    Projectile.rotation,
                    Textures.Bloom[0].Value.Size() / 2f,
                    Projectile.scale * intensity, SpriteEffects.None, 0f);

                var spotlight = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var spotlightOrig = spotlight.Size() / 2f;
                var crossScale = new Vector2(0.66f * intensity, (2.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 8f) * 0.3f) * intensity);

                var spotlightDrawColor = drawColor with { A = 0, } * 0.5f;
                spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}