using AQMod.Common.Config;
using AQMod.Content.Dusts;
using AQMod.NPCs.Boss.Starite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class OmegaRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
            projectile.timeLeft *= 5;
        }

        private const float size = OmegaStarite.CIRCUMFERENCE * 4f;

        public override void AI()
        {
            var npc = Main.npc[(int)projectile.ai[0]];
            var omegaStarite = (OmegaStarite)npc.modNPC;
            if (!npc.active)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = npc.Center;
            projectile.rotation = -omegaStarite.innerRingRoll;
            if (omegaStarite.IsOmegaLaserActive())
                projectile.timeLeft = LASER_DEATH_TIME;
        }

        private const float NORMAL_BEAM_LENGTH = 3410.5f;

        public const int LASER_DEATH_TIME = 14;

        public float GetLaserScale()
        {
            return projectile.timeLeft <= LASER_DEATH_TIME ? 1f / LASER_DEATH_TIME * projectile.timeLeft * projectile.scale : projectile.scale;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(projectile.rotation);
            var offset = normal * OmegaStarite.CIRCUMFERENCE;
            Vector2 end = projectile.Center + offset + normal * NORMAL_BEAM_LENGTH;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + offset, end, size * projectile.scale, ref _);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Texture2D texture = Main.projectileTexture[projectile.type];
            var frame = new Rectangle(0, 0, texture.Width, texture.Height / Main.projFrames[projectile.type]);
            float timeSin = (float)(Math.Sin(Main.GlobalTime) + 1f) / 2f;
            Vector2 normalizedRotation = new Vector2(1f, 0f).RotatedBy(projectile.rotation);
            Vector2 basePosition = drawPosition + normalizedRotation * OmegaStarite.CIRCUMFERENCE;
            Vector2 origin = frame.Size() / 2f;
            Color beamColor = AQMod.glimmerEvent.stariteProjectileColor * 0.065f;
            float rotation = projectile.rotation - MathHelper.PiOver2;
            float baseScale = GetLaserScale();
            Main.spriteBatch.Draw(texture, basePosition, frame, beamColor, rotation, origin, new Vector2(1f * baseScale, 1f * baseScale), SpriteEffects.None, 0f);
            Vector2 basePosition2 = basePosition + normalizedRotation * (origin.Y - 1.9999998f);
            var origin2 = new Vector2(texture.Width / 2f, 0f);
            var frame2 = new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height);
            int repetitions = (int)((5 + Main.frameRate) * ModContent.GetInstance<AQConfigClient>().EffectQuality);
            float lerpValueMult = 1f / repetitions;
            float length = Main.screenHeight / (20 - repetitions / 60);
            Main.spriteBatch.Draw(texture, basePosition2, frame2, beamColor, rotation, origin2, new Vector2(1f * baseScale, length), SpriteEffects.None, 0f);
            float laserIntensity = ModContent.GetInstance<AQConfigClient>().EffectIntensity;
            beamColor = new Color((int)(beamColor.R * laserIntensity), (int)(beamColor.G * laserIntensity), (int)(beamColor.B * laserIntensity), (int)(beamColor.A * laserIntensity));
            var goToColor = new Color(ModContent.GetInstance<AQConfigClient>().EffectIntensity, ModContent.GetInstance<AQConfigClient>().EffectIntensity, ModContent.GetInstance<AQConfigClient>().EffectIntensity, ModContent.GetInstance<AQConfigClient>().EffectIntensity);
            for (int i = 0; i < repetitions; i++)
            {
                float lerpValue = lerpValueMult * i;
                float progress = 1f - lerpValue;
                var drawColor = Color.Lerp(beamColor, goToColor, lerpValue);
                Main.spriteBatch.Draw(texture, basePosition + normalizedRotation * MathHelper.Lerp(0f, origin.Y, lerpValueMult * i), frame, drawColor, rotation, origin, new Vector2(progress * baseScale, progress), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, basePosition2, frame2, drawColor, rotation, origin2, new Vector2(progress * baseScale, length), SpriteEffects.None, 0f);
            }
            if (Main.gamePaused || Main.gameInactive)
                return false;
            int dustAmount = 1 + Main.frameRate / 6;
            Vector2 dustVelocityNormal = new Vector2(1f, 0f).RotatedBy(projectile.rotation - MathHelper.PiOver2);
            Vector2 dustPositionOffset = dustVelocityNormal * (size / 2 - 60f) * baseScale;
            int type = ModContent.DustType<MonoDust>();
            Vector2 spawnBase = projectile.Center + normalizedRotation * (OmegaStarite.CIRCUMFERENCE + 30f);
            for (int i = 0; i < dustAmount; i++)
            {
                float x = Main.rand.NextFloat(0f, length) * 60f;
                float dir = Main.rand.NextBool() ? -1 : 1;
                int d = Dust.NewDust(spawnBase + normalizedRotation * x + dustPositionOffset * dir, 2, 2, type);
                if (d == -1)
                    continue;
                Main.dust[d].color = beamColor * 20;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = dustVelocityNormal * Main.rand.NextFloat(3f, 9.5f) * dir;
            }
            return false;
        }
    }
}