using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Enemies.DemonSiege.LavaLegs;

public class LeggedLavaProj : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileSets.PushableByTypeId.Add((ProjectileEntry)Type);
    }

    public override void SetDefaults() {
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(180, 180, 180, 60);
    }

    public override void AI() {
        if (Main.rand.NextBool(4)) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            d.scale *= 0.6f;
            d.fadeIn = d.scale + 0.4f;
            d.velocity *= 0.33f;
            d.noGravity = true;
        }
        if (Projectile.lavaWet) {
            var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            if (Projectile.Center.Y < player.Center.Y) {
                Projectile.velocity.Y += 0.25f;
                if (Projectile.velocity.Y > 16f) {
                    Projectile.velocity.Y = 16f;
                }
            }
            else {
                Projectile.velocity.Y -= 0.25f;
                if (Projectile.velocity.Y < -16f) {
                    Projectile.velocity.Y = -16f;
                }
            }
        }
        else {
            Projectile.velocity.Y += 0.25f;
            if (Projectile.velocity.Y > 8f)
                Projectile.velocity.Y = 8f;
        }
        if (Math.Abs(Projectile.velocity.X) < 2f) {
            Projectile.velocity.X *= 0.98f;
            if (Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }
    }

    public override Boolean TileCollideStyle(ref Int32 width, ref Int32 height, ref Boolean fallThrough, ref Vector2 hitboxCenterFrac) {
        fallThrough = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].position.Y
            > Projectile.position.Y + Projectile.height;
        return true;
    }

    private void CollisionEffects(Vector2 velocity) {
        Vector2 spawnPos = Projectile.position + velocity;
        for (Int32 i = 0; i < 5; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            d.velocity = new Vector2(0f, 2f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            d.scale *= 0.6f;
            d.fadeIn = d.scale + 0.3f;
            d.noGravity = true;
        }
    }

    public override Boolean OnTileCollide(Vector2 oldVelocity) {
        Boolean doEffects = false;
        if (oldVelocity.X != Projectile.velocity.X && Math.Abs(oldVelocity.Y) > 2f) {
            doEffects = true;
            Projectile.position.X += Projectile.velocity.X * 0.9f;
            Projectile.velocity.X = -oldVelocity.X * 0.8f;
        }
        if (oldVelocity.Y != Projectile.velocity.Y && Math.Abs(oldVelocity.Y) > 2f) {
            doEffects = true;
            Projectile.position.Y += Projectile.velocity.Y * 0.9f;
            Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
        }
        if (doEffects) {
            CollisionEffects(Projectile.velocity);
        }
        else {
            Projectile.velocity *= 0.95f;
        }
        return false;
    }

    public override void OnKill(Int32 timeLeft) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Item85 with { Pitch = 1f }, Projectile.Center);
        }

        for (Int32 i = 0; i < 18; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            d.velocity = new Vector2(0f, 3f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            d.scale *= 0.6f;
            d.fadeIn = d.scale + 0.8f;
            d.noGravity = true;
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out Int32 _);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}