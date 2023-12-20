using System;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Weapons.Magic.SpamMail;

public class SpamMailProj : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.alpha = 255;
    }

    public override void OnSpawn(IEntitySource source) {
        var player = Main.player[Projectile.owner];
        if (player.GetWeaponCrit(player.HeldItem) > Main.rand.Next(100)) {
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 8;
        height = 8;
        return true;
    }

    public override void AI() {
        switch ((int)Projectile.ai[0]) {
            case 2:
                if ((int)Projectile.ai[1] == 10f) {
                    if (Main.myPlayer == Projectile.owner) {
                        for (int i = 0; i < 3; i++) {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 8f, ModContent.ProjectileType<SpamMailCritEffect>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                else if (Projectile.ai[1] > 10f) {
                    Projectile.alpha += 5;
                }
                else {
                    Projectile.alpha = 0;
                }

                if (Projectile.frame <= Main.projFrames[Type] - 1 && Projectile.frameCounter++ > 3) {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                Projectile.ai[1]++;
                break;

            case 1:
            case 0:
                Projectile.frame = (int)Projectile.ai[0];
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.alpha > 0) {
                    Projectile.alpha -= 30;
                    if (Projectile.alpha < 0) {
                        Projectile.alpha = 0;
                    }
                }
                break;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (Projectile.ai[0] > 0f) {
            modifiers.SetCrit();
        }
        else {
            modifiers.DisableCrit();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (hit.Crit) {
            Projectile.ai[0] = 2f;
            Projectile.penetrate = -1;
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = 0f;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.netUpdate = true;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        origin.Y += 6f;
        var drawCoordinates = Projectile.position + offset - Main.screenPosition;
        var drawColor = Color.Lerp(lightColor, Color.White, 0.1f) * Projectile.Opacity;

        float rotation = Projectile.rotation;
        var spriteEffects = SpriteEffects.None;
        float scale = Projectile.scale * Projectile.Opacity;
        if (Math.Abs(MathHelper.WrapAngle(Projectile.rotation)) >= MathHelper.PiOver2) {
            spriteEffects = SpriteEffects.FlipHorizontally;
            rotation -= MathHelper.Pi;
        }
        for (int i = 0; i < 3; i++) {
            Main.EntitySpriteDraw(texture, drawCoordinates - Projectile.velocity * i * 2f, frame, drawColor * 0.1f, rotation, origin, scale, spriteEffects, 0f);
        }
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, rotation, origin, scale, spriteEffects, 0f);
        return false;
    }
}