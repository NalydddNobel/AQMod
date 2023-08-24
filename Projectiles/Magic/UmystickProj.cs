using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic {
    public class UmystickProj : ModProjectile {
        private float _gfxOffY;

        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults() {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage() {
            return false;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            if ((int)Projectile.ai[0] == -1) {
                if (Projectile.timeLeft < 14) {
                    float progress = 1f - Projectile.timeLeft / 14f;
                    _gfxOffY = progress * 36f;
                    Projectile.alpha = (int)((progress) * 255f);

                    if (Projectile.frame < Main.projFrames[Type]) {
                        Projectile.frame++;
                    }
                }
                else if (Projectile.frame > 0) {
                    Projectile.frame--;
                }
                Projectile.Center = player.Center + Projectile.velocity * 36f;
                return;
            }
            var aequus = Main.player[Projectile.owner].Aequus();
            player.heldProj = Projectile.whoAmI;
            Vector2 rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter);
            bool ignoreChannel = (int)Projectile.localAI[0] < 100;

            if ((int)Projectile.ai[1] < 0) {
                ignoreChannel = true;
                int timer = (int)Projectile.ai[1] % 5;
                if (Projectile.frame < Main.projFrames[Projectile.type] - 1) {
                    Projectile.frame++;
                }
                if (timer == 0) {
                    Projectile.localAI[0] = 100f;
                    _gfxOffY = 14f;
                    SoundEngine.PlaySound(Aequus.GetSounds("Item/Umystick/shoot", 3, 0.44f, variance: 0.33f), Projectile.Center);
                    if (Main.myPlayer == Projectile.owner) {
                        var shootPosition = Projectile.Center;
                        if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height)) {
                            shootPosition = player.Center;
                        }
                        if (aequus.itemCombo > 0) {
                            aequus.itemCombo = 10;
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), shootPosition, Projectile.velocity.RotatedBy((Projectile.ai[1] + 10f) * 0.01f * (aequus.itemCombo > 0 ? -1 : 1)) * 27.5f / 3f,
                            ModContent.ProjectileType<UmystickBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Projectile.netUpdate = true;
                }
                else {
                    _gfxOffY *= 0.94f;
                }
                if (Projectile.ai[1] <= -15) {
                    if (Projectile.ai[0] == 0) {
                        Projectile.ai[0]++;
                    }
                    else if (!player.CheckMana(player.inventory[player.selectedItem], pay: true)) {
                        Projectile.ai[0] = -1f;
                        return;
                    }
                    Projectile.ai[1] = 24f;
                    if (aequus.itemCombo <= 0) {
                        Main.player[Projectile.owner].Aequus().itemCombo = 120;
                    }
                }
            }
            else {
                if (Projectile.frame > 0) {
                    Projectile.frame--;
                }
                _gfxOffY *= 0.94f;
            }
            if (Projectile.localAI[0] >= 8f)
                Projectile.ai[1]--;

            AI_UpdateHeldTimes(player);

            if (Main.myPlayer == Projectile.owner) {
                if (ignoreChannel || (player.channel && !player.noItems && !player.CCed)) {
                    if (ignoreChannel) {
                        var difference = Main.MouseWorld - player.Center;
                        Projectile.velocity = Vector2.Normalize(difference);
                    }
                }
                else {
                    Projectile.ai[0] = -1f;
                    Projectile.timeLeft = player.itemTime;
                }
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + Projectile.velocity * 36f;
            player.ChangeDir(Math.Sign(Projectile.velocity.X));
            Projectile.hide = false;

            if (Projectile.localAI[0] < 8f) {
                if ((int)Projectile.localAI[0] == 0) {
                    Projectile.frame = Main.projFrames[Type];
                }
                else if (Projectile.frame > 0) {
                    Projectile.frame--;
                }
                float progress = 1f - Projectile.localAI[0] / 8f;
                _gfxOffY = progress * 36f;
                Projectile.alpha = (int)((progress) * 255f);

                Projectile.localAI[0]++;
            }
        }

        public void AI_UpdateHeldTimes(Player player) {
            int minTime = 16;
            if (player.itemTime <= minTime) {
                player.itemTime = minTime;
                if (Projectile.ai[1] > minTime) {
                    player.itemTime = (int)Projectile.ai[1];
                }
            }
            else {
                Projectile.friendly = false;
            }
            if (player.itemAnimation <= minTime) {
                player.itemAnimation = minTime;
                if (Projectile.ai[1] > minTime) {
                    player.itemTime = (int)Projectile.ai[1];
                }
            }
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            var center = Projectile.Center;
            var effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            var player = Main.player[Projectile.owner];
            origin.X += Projectile.spriteDirection * 4f;
            center += Projectile.velocity * -_gfxOffY;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            return false;
        }
    }
}