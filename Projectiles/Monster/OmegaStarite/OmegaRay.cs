using AQMod.Common;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.OmegaStarite
{
    public class OmegaRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft *= 5;
        }

        private const float size = NPCs.Boss.OmegaStarite.CIRCUMFERENCE * 4f;

        public override void AI()
        {
            var npc = Main.npc[(int)Projectile.ai[0]];
            var omegaStarite = (NPCs.Boss.OmegaStarite)npc.ModNPC;
            if (!npc.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = npc.Center;
            Projectile.rotation = -omegaStarite.innerRingRoll;
            if (omegaStarite.IsOmegaLaserActive())
                Projectile.timeLeft = LASER_DEATH_TIME;
        }

        private const float NORMAL_BEAM_LENGTH = 3410.5f;

        public const int LASER_DEATH_TIME = 14;

        public float GetLaserScale()
        {
            return Projectile.timeLeft <= LASER_DEATH_TIME ? 1f / LASER_DEATH_TIME * Projectile.timeLeft * Projectile.scale : Projectile.scale;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            var offset = normal * NPCs.Boss.OmegaStarite.CIRCUMFERENCE;
            Vector2 end = Projectile.Center + offset + normal * NORMAL_BEAM_LENGTH;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + offset, end, size * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var frame = new Rectangle(0, 0, texture.Width, texture.Height / Main.projFrames[Projectile.type]);
            float timeSin = (float)(Math.Sin(Main.GlobalTimeWrappedHourly) + 1f) / 2f;
            var normalizedRotation = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            var basePosition = drawPosition + normalizedRotation * NPCs.Boss.OmegaStarite.CIRCUMFERENCE;
            var origin = frame.Size() / 2f;
            var options = ClientOptions.Instance;
            var beamColor = options.StariteProjectileColoring * 0.065f;
            float rotation = Projectile.rotation - MathHelper.PiOver2;
            float baseScale = GetLaserScale();
            Main.spriteBatch.Draw(texture, basePosition, frame, beamColor, rotation, origin, new Vector2(1f * baseScale, 1f * baseScale), SpriteEffects.None, 0f);
            var basePosition2 = basePosition + normalizedRotation * (origin.Y - 1.9999998f);
            var origin2 = new Vector2(texture.Width / 2f, 0f);
            var frame2 = new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height);
            int repetitions = (int)((5 + Main.frameRate) * options.FXQuality);
            float lerpValueMult = 1f / repetitions;
            float length = Main.screenHeight / (20 - repetitions / 60);
            Main.spriteBatch.Draw(texture, basePosition2, frame2, beamColor, rotation, origin2, new Vector2(1f * baseScale, length), SpriteEffects.None, 0f);
            float laserIntensity = options.FXIntensity;
            beamColor = new Color((int)(beamColor.R * laserIntensity), (int)(beamColor.G * laserIntensity), (int)(beamColor.B * laserIntensity), (int)(beamColor.A * laserIntensity));
            var goToColor = new Color(options.FXIntensity, options.FXIntensity, options.FXIntensity, options.FXIntensity);
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
            var dustVelocityNormal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            var dustPositionOffset = dustVelocityNormal * (size / 2 - 60f) * baseScale;
            int type = ModContent.DustType<MonoDust>();
            var spawnBase = Projectile.Center + normalizedRotation * (NPCs.Boss.OmegaStarite.CIRCUMFERENCE + 30f);
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