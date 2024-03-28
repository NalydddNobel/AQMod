using Aequus.Common.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Dedicated.EtOmniaVanitas;

public class EtOmniaVanitasProj : HeldProjBase {
    public override string Texture => AequusTextures.EtOmniaVanitas_Muzzle.Path;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults() {
        Projectile.SetDefaultHeldProj();
        Projectile.timeLeft = 4;
        Projectile.hide = true;
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        player.heldProj = Projectile.whoAmI;
        player.itemTime = Math.Max(player.itemTime, 2);
        player.itemAnimation = Math.Max(player.itemAnimation, 2);
        Projectile.Center = player.Center;
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (!player.CCed && Projectile.ai[0] > 1) {
            Projectile.ai[0]--;
        }
        if (player.channel) {
            Projectile.timeLeft = 2;
        }

        if (Projectile.localAI[0] > 0f) {
            Projectile.localAI[0] *= 0.9f;
            Projectile.localAI[0]--;
            if (Projectile.localAI[0] < 0f) {
                Projectile.localAI[0] = 0f;
            }
        }

        if (Projectile.frame > 0) {
            if (Projectile.frameCounter++ > 1) {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        if (Projectile.ai[0] <= 1 && player.channel) {
            // Shoot
            var heldItem = player.HeldItemFixed();
            if (Main.myPlayer == Projectile.owner && player.HasAmmo(heldItem) && player.PickAmmo(heldItem, out int projToShoot, out float bulletSpeed, out int bulletDamage, out float bulletKb, out var usedAmmoItemId, dontConsume: Projectile.ai[0] == 0)) {
                var wantedVector = Projectile.DirectionTo(Main.MouseWorld);

                if (Projectile.velocity.X != wantedVector.X || Projectile.velocity.Y != wantedVector.Y) {
                    Projectile.netUpdate = true;
                    Projectile.velocity = Vector2.Normalize(wantedVector);
                }

                player.ChangeDir(Math.Sign(wantedVector.X));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, wantedVector * bulletSpeed, ModContent.ProjectileType<EtOmniaVanitasBullet>(), bulletDamage, bulletKb, Projectile.owner, ai0: projToShoot);
            }
            Projectile.frame = player.itemTimeMax < 7 && (int)Projectile.localAI[2] % 2 == 0 ? 2 : 1;
            Projectile.ai[0] = player.itemTimeMax;
            Projectile.localAI[0] = 11f;
            Projectile.localAI[1] = Main.rand.NextFloat(player.itemTimeMax * 0.15f, player.itemTimeMax * 0.5f);
            Projectile.localAI[2]++;
            SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);

            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - player.fullRotation;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Projectile.owner);
            NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, Projectile.owner);

            //armRotation = Projectile.velocity.ToRotation();
        }

        //SetArmRotation(player);
    }

    public override bool PreDraw(ref Color lightColor) {
        Main.GetItemDrawFrame(Main.player[Projectile.owner].HeldItemFixed().type, out var itemTexture, out var itemFrame);
        if (Projectile.frame > 0) {
            lightColor = Color.Lerp(lightColor, Color.White, (1f - (Projectile.frame - 1) * 0.5f) * 0.5f);
        }
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
        frame.Height -= 2;
        var drawCoordinates = Main.player[Projectile.owner].MountedCenter + Vector2.Normalize(Projectile.velocity) * 16f + new Vector2(0f, Main.player[Projectile.owner].gfxOffY - 2f) - Main.screenPosition;
        var gunDrawCoordinates = drawCoordinates;
        gunDrawCoordinates.Y += 6f;
        float gunRotation = Projectile.rotation;
        int flipDir = Math.Abs(Projectile.rotation) > MathHelper.PiOver2 ? -1 : 1;
        var spriteEffects = flipDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        float animationProgress = Projectile.localAI[0] / 11f;
        if (Projectile.localAI[0] < 9f) {
            gunDrawCoordinates += (Projectile.rotation + Projectile.localAI[0] * 0.1f * flipDir).ToRotationVector2() * -Projectile.localAI[0] * (Projectile.localAI[1] / 8f);
            gunRotation += 0.03f * Projectile.localAI[0] * -flipDir * (Projectile.localAI[1] / 8f);

            if (Projectile.localAI[1] < 1f) {
                gunDrawCoordinates += Main.rand.NextVector2Unit() * 2f;
            }
        }

        Main.EntitySpriteDraw(itemTexture, gunDrawCoordinates, itemFrame, lightColor, gunRotation, itemFrame.Size() / 2f, Projectile.scale, spriteEffects, 0f);
        var rotationVector = Projectile.rotation.ToRotationVector2();
        var muzzleCoordinates = drawCoordinates + rotationVector * (itemFrame.Width / 2f + 18f - Projectile.localAI[0]) + new Vector2(0f, -5f * flipDir).RotatedBy(Projectile.rotation);
        Main.EntitySpriteDraw(texture, muzzleCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale * animationProgress, spriteEffects, 0f);
        Main.EntitySpriteDraw(AequusTextures.BloomStrong, muzzleCoordinates - rotationVector * (1f - animationProgress) * 8f, null, Color.Blue with { A = 0 } * 0.4f * animationProgress, gunRotation, AequusTextures.BloomStrong.Size() / 2f, Projectile.scale * 0.4f, spriteEffects, 0f);
        return false;
    }
}
