using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class LiquidGunProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];

            player.itemTimeMax = Math.Max(player.itemTimeMax, 6);
            if (Main.myPlayer == Projectile.owner)
            {
                if ((!ItemLoader.CanUseItem(player.HeldItem, player) && player.itemTime < 4) || !player.channel || player.noItems || player.CCed)
                {
                    Projectile.ai[0] = -1f;
                    Projectile.timeLeft = player.itemTime;
                    player.itemAnimation = player.itemTime;
                }
                Projectile.netUpdate = true;
            }

            Projectile.Center = player.Center + Projectile.velocity * 18f;

            player.heldProj = Projectile.whoAmI;

            if ((int)Projectile.ai[0] == -1)
            {
                return;
            }

            player.itemAnimation = 2;

            if (player.itemTime == player.itemTimeMax - 2)
            {
                if (player.HeldItem.UseSound != null)
                {
                    SoundEngine.PlaySound(player.HeldItem.UseSound, Projectile.Center);
                }
                DecideProjectileShot(player, out int projID, out float ai0, out float ai1);
                if (player.HeldItem.ModItem is LiquidGun heldItem)
                {
                    if (heldItem.LiquidAmount > 0)
                        heldItem.LiquidAmount--;
                    if (heldItem.LiquidAmount == 0)
                    {
                        heldItem.LiquidType = 0;
                        heldItem.CheckTexture();
                    }
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - new Vector2(0f, 4f),
                        Projectile.velocity * 5f, projID, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0, ai1);
                }
            }
            if (player.itemTime == 2)
            {
                player.itemTime = player.itemTimeMax;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * Projectile.velocity.Length();
                }
            }

            var aequus = Main.player[Projectile.owner].Aequus();
            var rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter);
            bool ignoreChannel = (int)Projectile.localAI[0] < 100;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + Projectile.velocity * 18f;
            player.ChangeDir(Math.Sign(Projectile.velocity.X));
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            AequusHelpers.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
            Projectile.hide = false;
        }

        public void DecideProjectileShot(Player player, out int proj, out float ai0, out float ai1)
        {
            proj = ModContent.ProjectileType<LiquidGunWaterBullet>();
            ai0 = 0f;
            if (player.HeldItem.ModItem is LiquidGun heldItem && LiquidGun.TryGetLiquidInfo(heldItem.LiquidType, out var info))
            {
                ai1 = info.ProjDustID();
                return;
            }
            ai1 = Dust.dustWater();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = null;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            if (Main.player[Projectile.owner].HeldItem.ModItem is LiquidGun heldItem)
            {
                if (LiquidGun.TryGetLiquidInfo(heldItem.LiquidType, out var info))
                {
                    texture = ModContent.Request<Texture2D>(info.TexturePath).Value;
                    if (ModContent.RequestIfExists<Texture2D>($"{info.TexturePath}_Glow", out var glowMask))
                    {
                        glow = glowMask.Value;
                    }
                }
                AequusHelpers.GetItemDrawData(Main.player[Projectile.owner].HeldItem.type, out frame);
            }
            var origin = frame.Size() / 2f;
            var center = Main.GetPlayerArmPosition(Projectile);
            var effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            var player = Main.player[Projectile.owner];
            origin.X -= Projectile.spriteDirection * 8f;
            origin.Y += 8f;
            center += Projectile.velocity;
            center = center.Floor();
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            if (glow != null)
                Main.spriteBatch.Draw(glow, center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0f);

            return false;
        }
    }
}