using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Pets
{
    public class MicroStarite : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.aiStyle = -1;
        }

        private float innerRingRotation;
        private float innerRingRoll;
        private float innerRingPitch;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            AQPlayer aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.dead)
                aQPlayer.microStarite = false;
            if (aQPlayer.microStarite)
                projectile.timeLeft = 2;
            Vector2 gotoPos = player.Center;
            var center = projectile.Center;
            float distance = (center - gotoPos).Length();
            if (distance < (player.width + projectile.width) * 2f)
            {
                innerRingRotation += 0.0314f;
                innerRingRoll += 0.0157f;
                innerRingPitch += 0.01f;
                projectile.velocity += player.velocity * 0.01f;
                if (projectile.velocity.Length() > 1f)
                    projectile.velocity *= 0.97f;
            }
            else if (distance < 2000f)
            {
                innerRingRotation += 0.0314f;
                innerRingRoll += 0.0157f;
                innerRingPitch += 0.01f;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(gotoPos - center) * 20f, 0.02f);
            }
            else
            {
                projectile.Center = player.Center;
                projectile.velocity *= 0.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var frame = new Rectangle(0, 0, texture.Width / 3, texture.Height / 2);
            Vector2 center = projectile.Center;
            float rotMult = MathHelper.TwoPi / 5f;
            const int orbsCount = 5;
            var orbs = new Vector3[orbsCount];
            float[] orbScales = new float[orbsCount];
            for (int i = 0; i < orbsCount; i++)
            {
                orbs[i] = Vector3.Transform(new Vector3(frame.Width, 0f, 0f), Matrix.CreateFromYawPitchRoll(innerRingPitch, innerRingRoll, innerRingRotation + rotMult * i));
                orbScales[i] = AQUtils.OmegaStarite3DHelper.GetParralaxScale(projectile.scale, orbs[i].Z * 0.157f);
                if (orbs[i].Z > 0f)
                {
                    var drawPosition = AQUtils.OmegaStarite3DHelper.GetParralaxPosition(new Vector2(center.X + orbs[i].X, center.Y + orbs[i].Y), orbs[i].Z * 0.0314f) - Main.screenPosition;
                    int frameNumber = 1;
                    if (orbScales[i] <= 0.925f)
                        frameNumber = 2;
                    var drawFrame = new Rectangle(frame.Width * frameNumber, frame.Height, frame.Width, frame.Height);
                    Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            for (int i = 0; i < orbsCount; i++)
            {
                if (orbs[i].Z < 0f)
                {
                    var drawPosition = AQUtils.OmegaStarite3DHelper.GetParralaxPosition(new Vector2(center.X + orbs[i].X, center.Y + orbs[i].Y), orbs[i].Z * 0.0314f) - Main.screenPosition;
                    int frameNumber = 1;
                    if (orbScales[i] >= 1.075f)
                        frameNumber = 0;
                    var drawFrame = new Rectangle(frame.Width * frameNumber, frame.Height, frame.Width, frame.Height);
                    Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
    }
}