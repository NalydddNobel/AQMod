using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Pets.LanternCat {
    public class LanternCatPet : ModProjectile {
        private int frame;
        private double frameCounter;

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            DrawOriginOffsetY = -9;
            AIType = ProjectileID.BlackCat;
        }

        public override bool PreAI() {
            Helper.UpdateProjActive<LanternCatBuff>(Projectile);
            Lighting.AddLight(Projectile.Top, new Vector3(0.8f, 0.6f, 0.2f));
            if ((int)Projectile.ai[0] == 1) {
                frameCounter++;
                if (frameCounter > 6.0d) {
                    frameCounter = 0.0d;
                    frame++;
                    if (frame >= Main.projFrames[Type]) {
                        frame = 6;
                    }
                }
                if (frame < 6) {
                    frame = 6;
                }
            }
            else if (Projectile.velocity.Y != 0f) {
                frame = 3;
            }
            else {
                float speedX = Math.Abs(Projectile.velocity.X);
                if (speedX < 0.1f) {
                    frameCounter = 0.0d;
                    frame = 0;
                }
                else {
                    frameCounter += speedX * 0.5f;
                    if (frameCounter > 6.0d) {
                        frameCounter = 0.0d;
                        frame++;
                        if (frame >= 6) {
                            frame = 0;
                        }
                    }
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.frame = this.frame;
            var lantern = AequusTextures.LanternCatPet_Lantern.Value;
            var lanternPosition = Projectile.Center + (new Vector2(24f * -Projectile.spriteDirection, -24f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f) * 8f) + new Vector2(Projectile.velocity.X * 2f, Projectile.velocity.Y * -0.1f - Math.Abs(Projectile.velocity.X) * 0.5f)) * Projectile.scale;
            float lanternRotation = Projectile.velocity.X * 0.1f * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.5f, 1f);
            var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
            Main.EntitySpriteDraw(
                texture,
                Projectile.position + offset + new Vector2(0f, DrawOriginOffsetY + Projectile.gfxOffY) - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                spriteEffects,
                0
            );
            Main.EntitySpriteDraw(
                lantern,
                lanternPosition - Main.screenPosition,
                null,
                Color.White,
                lanternRotation,
                lantern.Size() / 2f,
                Projectile.scale,
                spriteEffects,
                0
            );

            Main.EntitySpriteDraw(
                AequusTextures.Bloom0,
                lanternPosition - Main.screenPosition,
                null,
                Color.Orange with { A = 0 } * 0.2f,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                Projectile.scale * 0.8f,
                spriteEffects,
                0
            );
            return false;
        }
    }
}