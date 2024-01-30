using System;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Weapons.Magic.TrashStaff;

public class TrashStaffProj : ModProjectile {
    public override LocalizedText DisplayName => ModContent.GetInstance<TrashStaff>().DisplayName;

    public Boolean Crit => (Int32)Projectile.ai[0] != 0;

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
        if (player.RollCrit(DamageClass.Magic)) {
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }
    }

    public override Boolean TileCollideStyle(ref Int32 width, ref Int32 height, ref Boolean fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 8;
        height = 8;
        return true;
    }

    public override void AI() {
        switch ((Int32)Projectile.ai[0]) {
            case 2:
                if ((Int32)Projectile.ai[1] == 10f) {
                    if (Main.myPlayer == Projectile.owner) {
                        for (Int32 i = 0; i < 3; i++) {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Unit() * 8f, ModContent.ProjectileType<TrashStaffCritEffect>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                else if (Projectile.ai[1] > 10f) {
                    Projectile.alpha += 16;
                }
                else {
                    Projectile.alpha = 0;
                }

                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.WrapAngle(Projectile.velocity.ToRotation() + MathHelper.PiOver2), 0.1f);
                Projectile.velocity *= 0.95f;

                if (Projectile.ai[1] >= 0f && Projectile.frame <= Main.projFrames[Type] - 1 && Projectile.frameCounter++ > 3) {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                Projectile.ai[1]++;
                break;

            case 1:
            case 0:
                Projectile.frame = (Int32)Projectile.ai[0];
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
        if (Crit) {
            modifiers.SetCrit();
        }
        else {
            modifiers.DisableCrit();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, Int32 damageDone) {
        if (hit.Crit) {
            Projectile.ai[0] = 2f;
            Projectile.ai[1] = -16f;
            Projectile.penetrate = -1;
            Projectile.velocity = -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * 0.3f;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.netUpdate = true;
        }
    }

    public override void OnKill(Int32 timeLeft) {
        if (Projectile.ai[0] > 1) {
            return;
        }

        var dustColor = Crit ? Color.Lerp(Color.Red, Color.White, 0.4f) : Color.White;

        for (Int32 i = 0; i < 6; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Alpha: 170, newColor: dustColor, Scale: 1.6f);
            d.velocity += Projectile.oldVelocity * 0.1f;
            d.velocity *= 2f;
            d.noGravity = true;
            d.noLight = true;
            d.fadeIn = d.scale + 0.2f;
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out Int32 _);
        origin.Y += 6f;
        var drawCoordinates = Projectile.position + offset - Main.screenPosition;
        var drawColor = Color.Lerp(lightColor, Color.White, Crit ? 0.6f : 0.1f) * Projectile.Opacity;

        Single rotation = Projectile.rotation;
        var spriteEffects = SpriteEffects.None;
        Single scale = Projectile.scale * Projectile.Opacity;
        if (Math.Abs(MathHelper.WrapAngle(Projectile.rotation)) >= MathHelper.PiOver2) {
            spriteEffects = SpriteEffects.FlipHorizontally;
            rotation -= MathHelper.Pi;
        }
        if (Projectile.timeLeft < 60) {
            Single animation = 1f - Projectile.timeLeft / 60f;
            scale *= 1f + animation * 1f;
            drawCoordinates += Main.rand.NextVector2Square(-animation, animation) * 12f;
        }

        for (Int32 i = 0; i < 3; i++) {
            Main.EntitySpriteDraw(texture, drawCoordinates - Projectile.velocity * i * 2f, frame, drawColor * 0.1f, rotation, origin, scale, spriteEffects, 0f);
        }
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, rotation, origin, scale, spriteEffects, 0f);
        return false;
    }
}