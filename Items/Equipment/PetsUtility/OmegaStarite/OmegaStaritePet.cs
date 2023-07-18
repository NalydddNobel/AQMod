using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsUtility.OmegaStarite {
    public class OmegaStaritePet : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
        }

        private float innerRingRotation;
        private float innerRingRoll;
        private float innerRingPitch;

        public override void AI() {
            Player player = Main.player[Projectile.owner];
            Helper.UpdateProjActive<OmegaStariteBuff>(Projectile);
            var gotoPos = player.Center + new Vector2((player.width / 2f + Projectile.width * 2.25f) * player.direction, player.height / -2f + Projectile.height - 32f);
            var center = Projectile.Center;
            float distance = (center - gotoPos).Length();
            if (Projectile.numUpdates == -1) {
                innerRingRotation += 0.0314f;
                innerRingRoll += 0.0157f;
                innerRingPitch += 0.01f;
            }
            if (distance < 2000f) {
                float amount = 0.025f;
                if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
                    distance = Math.Max(distance * 2f, 100f);
                    amount *= 2f;
                }
                if (distance < player.width + Projectile.width) {
                    amount = 0.001f;
                    if (Projectile.numUpdates == -1) {
                        Projectile.position += player.velocity * 0.75f;
                    }
                    if (Projectile.velocity.Length() > 0.1f) {
                        Projectile.velocity.X *= 0.97f;
                        Projectile.velocity.Y *= 0.97f;
                    }
                }
                else {
                    if (distance > 200f) {
                        amount *= 2f;
                    }
                    if (distance > 600f) {
                        amount *= 2f;
                    }
                    Projectile.position += Main.player[Projectile.owner].velocity / 4f;
                }
                int directionX = Math.Sign(gotoPos.X - Projectile.Center.X);
                if (Projectile.velocity.X.Abs() < 6f || Math.Sign(Projectile.velocity.X) != directionX) {
                    if (Math.Sign(Projectile.velocity.X) != directionX)
                        Projectile.velocity.X *= 0.985f;
                    Projectile.velocity.X += directionX * amount;
                }
                int directionY = Math.Sign(gotoPos.Y - Projectile.Center.Y);
                if (Projectile.velocity.Y.Abs() < 6f || Math.Sign(Projectile.velocity.Y) != directionY) {
                    if (Math.Sign(Projectile.velocity.X) != directionX)
                        Projectile.velocity.X *= 0.985f;
                    Projectile.velocity.Y += directionY * amount;
                }
            }
            else {
                Projectile.Center = player.Center;
                Projectile.velocity *= 0.1f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.3f, 0.5f) * 4f);
        }

        private void DrawGlowy(Texture2D texture, Vector2 drawCoordinates, Rectangle frame, Color color, float scale = 1f) {
            var origin = frame.Size() / 2f;
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2) {
                Main.EntitySpriteDraw(texture, drawCoordinates + (f + Projectile.rotation).ToRotationVector2() * 2f * scale, frame, color.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 0.95f), Projectile.rotation, origin, Projectile.scale * scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, drawCoordinates, frame, color, Projectile.rotation, origin, Projectile.scale * scale, SpriteEffects.None, 0);
        }

        private void RenderOrb(Vector3 orb, float scale, Rectangle frame) {
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Projectile.Center;

            var drawPosition = ViewHelper.GetViewPoint(new Vector2(center.X + orb.X, center.Y + orb.Y), orb.Z * 0.0314f) - Main.screenPosition;
            int frameNumber = 1;
            if (scale <= 0.925f)
                frameNumber = 2;
            else if (scale >= 1.075f)
                frameNumber = 0;
            var drawFrame = new Rectangle(frame.Width * frameNumber, frame.Height, frame.Width, frame.Height);
            DrawGlowy(texture, drawPosition, drawFrame, new Color(255, 255, 255, 255));
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = new Rectangle(0, 0, texture.Width / 3, texture.Height / 2);
            Vector2 center = Projectile.Center;
            float rotMult = MathHelper.TwoPi / 5f;
            var orbs = new Vector3[5];
            float[] orbScales = new float[5];
            for (int i = 0; i < 5; i++) {
                orbs[i] = Vector3.Transform(new Vector3(frame.Width, 0f, 0f), Matrix.CreateFromYawPitchRoll(innerRingPitch, innerRingRoll, innerRingRotation + rotMult * i));
                orbScales[i] = ViewHelper.GetViewScale(Projectile.scale, orbs[i].Z * 0.157f);
                if (orbs[i].Z > 0f) {
                    RenderOrb(orbs[i], orbScales[i], frame);
                }
            }
            DrawGlowy(texture, Projectile.Center - Main.screenPosition, frame, new Color(255, 255, 255, 255));
            for (int i = 0; i < 5; i++) {
                if (orbs[i].Z < 0f) {
                    RenderOrb(orbs[i], orbScales[i], frame);
                }
            }
            return false;
        }
    }
}