using System;

namespace Aequus.Content.Items.Weapons.Magic.SpamMail;

public class SpamMailCritEffect : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 70;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.extraUpdates = 2;
        Projectile.penetrate = -1;
        ProjectileID.Sets.TrailCacheLength[Type] = 70;
    }

    public override void AI() {
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.timeLeft <= 60) {
            Projectile.alpha += 4;
            Projectile.velocity *= 0.9f;
            return;
        }

        var target = Projectile.FindTargetWithinRange(180f);
        if (target == null || Projectile.Distance(target.Center) < 20f) {
            Projectile.timeLeft = 60;
            return;
        }

        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 10f, 0.02f);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
        var drawCoordinates = Projectile.Center - Main.screenPosition;
        var drawColor = Color.Lerp(lightColor, Color.White, 0.1f) * Projectile.Opacity;

        float scale = Projectile.scale * Projectile.Opacity;

        Main.spriteBatch.End();
        Main.spriteBatch.BeginWorld(shader: true);

        DrawHelper.ApplyUVEffect(texture, new Vector2(30f, 1f), new Vector2(Main.GlobalTimeWrappedHourly % 1f, 0f));
        DrawHelper.VertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot,
            (p) => drawColor * (1f - p) * Projectile.Opacity,
            (p) => 6f * Projectile.Opacity * Projectile.scale,
            Projectile.Size / 2f - Main.screenPosition);
        DrawHelper.VertexStrip.DrawTrail();

        Main.spriteBatch.End();
        Main.spriteBatch.BeginWorld(shader: false);

        var spriteEffects = SpriteEffects.None;
        float rotation = Projectile.rotation;
        if (Math.Abs(MathHelper.WrapAngle(rotation)) >= MathHelper.PiOver2) {
            spriteEffects = SpriteEffects.FlipHorizontally;
            rotation -= MathHelper.Pi;
        }
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, rotation, origin, scale, spriteEffects, 0f);
        return false;
    }
}