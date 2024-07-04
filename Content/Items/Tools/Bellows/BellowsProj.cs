using AequusRemake.Core.Entities.Projectiles;
using System;
using Terraria.GameContent;

namespace AequusRemake.Content.Items.Tools.Bellows;

public class BellowsProj : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hide = true;
        Projectile.timeLeft = 68 * (1 + Projectile.extraUpdates);
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        if (player.itemAnimation >= player.itemAnimationMax * 0.75f && Main.netMode != NetmodeID.Server) {
            var v = Vector2.Normalize(Projectile.velocity);
            var spawnPos = player.MountedCenter + v * (player.width + 10);
            if (Main.rand.NextBool(4)) {
                var g = Gore.NewGoreDirect(player.GetSource_ItemUse(player.HeldItem), spawnPos,
                    v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f), GoreID.Smoke1 + Main.rand.Next(3));
                Main.instance.LoadGore(g.type);
                g.position -= TextureAssets.Gore[g.type].Size() / 2f;
                g.scale = Main.rand.NextFloat(0.5f, 1.1f);
                g.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            var d = Dust.NewDustDirect(spawnPos, 10, 10, DustID.Smoke);
            d.velocity *= 0.1f;
            d.velocity += v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f);
            d.scale = Main.rand.NextFloat(0.8f, 1.5f);
            d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        if (Projectile.numUpdates <= 0 && player.itemAnimation >= 45) {
            var v = Projectile.velocity.SafeNormalize(new Vector2(player.direction, 0f));
            if (v.Y > 0f) {
                v.Y *= player.gravity / 0.4f;
            }
            player.velocity -= v * GetPushForce(player, player.HeldItemFixed());
            if (player.velocity.X < 4f) {
                player.fallStart = (int)player.position.Y / 16;
            }
            //if (Math.Abs(player.velocity.X) > player.accRunSpeed) {
            //    player.velocity.X *= 0.9f;
            //}
            //if (player.velocity.Y < -player.jumpSpeedBoost) {
            //    player.velocity.Y *= 0.9f;
            //}
        }
        //Projectile.velocity = Vector2.Normalize();
        player.heldProj = Projectile.whoAmI;
        if (Projectile.numUpdates != -1) {
            return;
        }

        if (Projectile.timeLeft < 20 * (Projectile.extraUpdates + 1)) {
            if (Projectile.frame > 0) {
                if (Projectile.frameCounter++ > 2) {
                    Projectile.frameCounter = 0;
                    Projectile.frame--;
                }
            }
        }
        else if (Projectile.frame < 3) {
            if (Projectile.frameCounter++ > 2) {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
        }
    }
    public float GetPushForce(Player player, Item item) {
        float force = item.knockBack;
        if (player.mount != null && player.mount.Active && player.mount._data.usesHover) {
            force *= Bellows.MountPushForcePenalty;
        }
        force /= Math.Max(player.velocity.Length() / 8f, 1f);
        return float.IsNaN(force) ? 0f : force * 1.8f;
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var _, out var frame, out var origin, out int _);

        var player = Main.player[Projectile.owner];
        var difference = -Projectile.velocity;
        var dir = Vector2.Normalize(difference);
        var drawCoords = player.MountedCenter + dir * -20f;
        drawCoords.Y += 2f + Projectile.gfxOffY;
        float rotation = difference.ToRotation() + (player.direction == -1 ? 0f : MathHelper.Pi);
        var spriteEffects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, ExtendLight.Get(drawCoords),
             rotation, origin, 1f, spriteEffects, 0);
        return false;
    }
}