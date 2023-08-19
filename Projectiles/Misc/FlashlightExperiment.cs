using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc {
    public class FlashlightExperiment : ModProjectile {
        public float coneAngles;

        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults() {
            Projectile.DefaultToNoInteractions();
            Projectile.width = 16;
            Projectile.height = 16;
        }

        public override void AI() {
            Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld);
            Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.timeLeft = 2;
        }

        private void ScanLine(Vector2 startPosition, Vector2 endPosition, bool edge, out Vector2 result, out bool collision) {
            result = startPosition;
            collision = false;

            var difference = endPosition - startPosition;
            float length = difference.Length();
            var stepOffsetNormalized = Vector2.Normalize(difference);
            var stepOffset = stepOffsetNormalized * 8f;
            int steps = (int)(length / 8f) + 1;

            for (int i = 0; i < steps; i++) {
                if (Collision.SolidCollision(result - new Vector2(1f), 2, 2)) {
                    collision = true;
                    break;
                }

                if (i >= steps - 2) {
                    result = endPosition;
                }
                else {
                    result += stepOffset;
                }
            }
            if (collision) {
                for (int i = 0; i < steps; i++) {
                    if (!Collision.SolidCollision(result - new Vector2(1f), 2, 2)) {
                        result += stepOffsetNormalized * 12f;
                        break;
                    }

                    result -= stepOffsetNormalized;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            int count = 51;
            coneAngles = MathHelper.PiOver4;
            var lastPosition = Projectile.Center;
            for (int i = 0; i < count; i++) {
                var startPosition = Projectile.Center;
                ScanLine(startPosition, Projectile.Center + Projectile.velocity.RotatedBy(coneAngles / count * (i - count / 2)) * 400f, i == 0 || i == count - 1, out var result, out var collision);
                Helper.DrawLine(startPosition - Main.screenPosition, result - Main.screenPosition, 2f, Color.Red);
                Helper.DrawLine(lastPosition - Main.screenPosition, result - Main.screenPosition, 2f, Color.Red);
                lastPosition = result;
            }
            Helper.DrawLine(Projectile.Center - Main.screenPosition, lastPosition - Main.screenPosition, 2f, Color.Red);
            return false;
        }
    }
}