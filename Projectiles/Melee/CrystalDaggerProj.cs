using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee {
    public class CrystalDaggerProj : ModProjectile {
        public override string Texture => Helper.GetPath<CrystalDagger>();

        public float stabLength;

        public override void SetDefaults() {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(120);
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            if (stabLength == 0f) {
                stabLength = 145f * speedMultiplier;
                Projectile.netUpdate = true;
            }
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            float lerpAmount = MathHelper.Clamp(1f - speedMultiplier, 0.1f, 1f);
            if (!player.frozen && !player.stoned) {
                if ((int)Projectile.ai[0] == 0) {
                    Projectile.ai[0] = 25f;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN() * Projectile.ai[0];
                    Helper.CappedMeleeScale(Projectile);
                    Projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f) {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
                else {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], stabLength, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));

                    var d = Dust.NewDustDirect(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(175, 200, 220, 80) * Main.rand.NextFloat(0.6f, 1f), 0.8f);
                    d.velocity *= 0.1f;
                    d.velocity += Vector2.Normalize(Projectile.velocity).UnNaN() * 1.35f;
                }
            }
            if (player.itemAnimation == 0)
                Projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity).UnNaN() * Projectile.ai[0], lerpAmount * 0.4f);
            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.direction);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawColor = Projectile.GetAlpha(lightColor);
            var swordTip = Projectile.Center;
            var origin = new Vector2(texture.Width, 0f);
            var effects = SpriteEffects.None;

            CrystalDagger.DrawCrystalDagger(Main.spriteBatch, texture, swordTip - Vector2.Normalize(Projectile.velocity) * 16f - Main.screenPosition, Projectile.Frame(), drawColor, Projectile.rotation, origin, effects, Projectile.scale);
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