using Aequus;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.RockMan {
    public class RockManProj : ModProjectile {
        public override string Texture => Helper.GetPath<RockMan>();

        public float stabLength;

        public override void SetDefaults() {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 32;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            float progress = player.itemAnimation / (float)player.itemAnimationMax;
            if (Projectile.localAI[1] == 0f) {
                Projectile.localAI[1] = Projectile.scale;
            }
            if (!player.frozen && !player.stoned) {
                if ((int)Projectile.ai[0] == 0) {
                    Helper.ScaleUp(Projectile);
                    Projectile.netUpdate = true;
                }
                if (stabLength == 0f) {
                    stabLength = 40f * speedMultiplier * Projectile.scale;
                    Projectile.netUpdate = true;
                }
                if (progress < 0.5f) {
                    Projectile.ai[0] = (float)Math.Sin(MathF.Pow(progress * 2f, 0.5f) * MathHelper.PiOver2) * stabLength;
                    if (progress < 0.25f) {
                        Projectile.Opacity = progress / 0.25f;
                        Projectile.scale = Projectile.localAI[1] * 0.75f + (Projectile.localAI[1] * 0.25f + 0.3f) * Projectile.Opacity;
                    }
                }
                else {
                    Projectile.ai[0] = (1f - (progress - 0.5f) / 0.5f) * stabLength;
                    Projectile.scale = Projectile.localAI[1] + MathF.Sin(progress * MathHelper.Pi) * 0.3f;
                }

                Projectile.ai[0] += 2;
                //if (player.itemAnimation < player.itemAnimationMax / 3f)
                //{
                //    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                //}
                //else
                //{
                //    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], stabLength, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));

                //    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(175, 200, 220, 80) * Main.rand.NextFloat(0.6f, 1f), 0.8f);
                //    d.velocity *= 0.1f;
                //    d.velocity += Vector2.Normalize(-Projectile.velocity) * 1.35f;
                //}
            }
            if (player.itemAnimation == 0)
                Projectile.Kill();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN();
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            Projectile.position += Projectile.velocity * Projectile.ai[0];
            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * Projectile.spriteDirection;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            if (progress > 0.5f) {
                var d = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Grass, 0f, 0f, 0, new Color(175, 200, 220, 80) * Main.rand.NextFloat(0.6f, 1f), 0.8f);
                d.velocity *= 0.1f;
                d.velocity += Vector2.Normalize(Projectile.velocity).UnNaN() * 1.35f;
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
            player.ChangeDir(Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var swordTip = Projectile.Center + Vector2.Normalize(Projectile.velocity) * Projectile.Size / 2f;
            var origin = new Vector2(texture.Width, 0f);
            if (Projectile.spriteDirection == -1) {
                origin = Vector2.Zero;
            }
            var effects = Projectile.spriteDirection.ToSpriteEffect();
            Main.EntitySpriteDraw(texture, swordTip - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(stabLength);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            stabLength = reader.ReadSingle();
        }
    }
}