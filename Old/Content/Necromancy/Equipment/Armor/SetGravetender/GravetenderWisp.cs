using Aequu2.Core.Entities.NPCs;
using Aequu2.Old.Common.Projectiles;
using System;

namespace Aequu2.Old.Content.Necromancy.Equipment.Armor.SetGravetender;

public class GravetenderWisp : LegacyMinionTemplate {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 30;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft *= 5;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 100);
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override void AI() {
        if (!Main.player[Projectile.owner].HasBuff<GravetenderMinionBuff>()) {
            return;
        }

        Projectile.timeLeft = 2;
        if (++Projectile.frameCounter > 7) {
            Projectile.frameCounter = 0;
            Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        Projectile.rotation = Projectile.velocity.X * 0.075f;
        var Aequu2 = Main.player[Projectile.owner].GetModPlayer<Aequu2Player>();
        var gotoPosition = Aequu2.gravetenderGhost > -1 ?
            Main.npc[Aequu2.gravetenderGhost].Center + new Vector2(0f, -Main.npc[Aequu2.gravetenderGhost].height - Projectile.height)
            : DefaultIdlePosition();
        var velocityMin = 4f;
        var diff = gotoPosition - Projectile.Center;
        if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 2000f) {
            Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.velocity *= 0.1f;
            Aequu2.gravetenderGhost = -1;
            diff = Vector2.UnitY;
        }
        var ovalDiff = new Vector2(diff.X, diff.Y * 3f);
        float ovalLength = ovalDiff.Length();
        if (ovalLength > 28f) {
            var velocity = diff / 50f;
            if (velocity.Length() < velocityMin) {
                velocity = Utils.SafeNormalize(velocity, Vector2.UnitY) * 4f;
            }
            if (Math.Sign(velocity.X) != Math.Sign(Projectile.velocity.X)) {
                Projectile.velocity.X *= 0.99f;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.01f);
        }
        else {
            Projectile.velocity *= 0.95f;
        }
        if (Aequu2.gravetenderGhost > -1) {
            Aequu2NPC Aequu2NPC = Main.npc[Aequu2.gravetenderGhost].GetGlobalNPC<Aequu2NPC>();
            if (Projectile.numUpdates == -1) {
                Projectile.position += (Aequu2.gravetenderGhost > -1 ? Main.npc[Aequu2.gravetenderGhost].velocity * new Vector2(Aequu2NPC.statSpeedX, Aequu2NPC.statSpeedY) : Main.player[Projectile.owner].velocity) * 0.97f;
            }
        }

        Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0f, 0.05f));

        if (Aequu2.gravetenderGhost != -1) {
            Projectile.spriteDirection = Math.Sign(Main.npc[Aequu2.gravetenderGhost].velocity.X);
        }
        else {
            Projectile.spriteDirection = Main.player[Projectile.owner].direction;
        }
    }

    public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count) {
        return base.IdlePosition(player, leader, minionPos, count) + new Vector2((-40f + player.width / 2f) * player.direction, -16f);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

        var c = Projectile.GetAlpha(lightColor) * Projectile.Opacity * Projectile.scale;

        off -= Main.screenPosition;
        origin.Y += 6f;
        var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].cHead, null);
        for (int i = 0; i < trailLength; i++) {
            float p = 1f - i / (float)trailLength;
            Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off, frame, c * 0.6f * p * p, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), effects, 0);
        }

        float f = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 4f);
        for (int i = 0; i < 4; i++) {
            Vector2 v = (i * MathHelper.PiOver2).ToRotationVector2();
            Main.EntitySpriteDraw(t, Projectile.position + off + v * (f - 2f), frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(t, Projectile.position + off + v * f, frame, c * 0.1f, Projectile.rotation, origin, Projectile.scale, effects, 0);
        }
        Main.EntitySpriteDraw(t, Projectile.position + off, frame, c, Projectile.rotation, origin, Projectile.scale, effects, 0);
        return false;
    }
}