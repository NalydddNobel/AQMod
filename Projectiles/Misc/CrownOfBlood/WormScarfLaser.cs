using Aequus.Content;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Projectiles.Misc.CrownOfBlood;
public class WormScarfLaser : ModProjectile {
    public override void SetStaticDefaults() {
        this.SetTrail(10);
        LegacyPushableEntities.AddProj(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.extraUpdates = 1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 50;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override bool? CanHitNPC(NPC target) {
        return (int)Projectile.ai[0] <= 0 || target.whoAmI == (int)Projectile.ai[0] - 1;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(180);
    }

    public override void AI() {
        if ((int)Projectile.localAI[0] == 0) {
            Projectile.localAI[0] = 1f;
            SoundEngine.PlaySound(SoundID.Item17 with { PitchVariance = 0.5f, MaxInstances = 10 }, Projectile.Center);
        }
        if ((int)Projectile.ai[0] > 0 && (int)Projectile.ai[0] < Main.maxNPCs + 1) {
            int npcTarget = (int)Projectile.ai[0] - 1;
            var npc = Main.npc[npcTarget];
            if (!npc.CanBeChasedBy()) {
                Projectile.ai[0] = 0f;
                return;
            }
            float speed = Projectile.velocity.Length();
            float lerpAmount = 0.33f + Projectile.ai[1] * 0.01f;
            if (Collision.SolidCollision(Projectile.Center - Projectile.Size, Projectile.width * 2, Projectile.height * 2)) {
                lerpAmount *= 2f;
            }
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center), Math.Max(lerpAmount, 1f))) * speed;
        }
        Projectile.ai[1]++;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 4;
        height = 4;
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<WormScarfHit>(),
                0,
                0f,
                Projectile.owner
            );
        }
    }

    public override void OnKill(int timeLeft) {
        SoundEngine.PlaySound(SoundID.Item10);
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
        var clr = Projectile.GetAlpha(lightColor);

        for (int i = 0; i < trailLength; i++) {
            float progress = Helper.CalcProgress(trailLength, i);
            Main.EntitySpriteDraw(
                texture,
                Projectile.oldPos[i] + offset - Main.screenPosition,
                frame,
                clr with { R = 20, G = 20, A = 100 } * progress,
                Projectile.oldRot[i],
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        Main.EntitySpriteDraw(
            texture,
            Projectile.position + offset - Main.screenPosition,
            frame,
            clr,
            Projectile.rotation,
            origin,
            Projectile.scale,
            SpriteEffects.None,
            0
        );
        return false;
    }
}

public class WormScarfHit : ModProjectile {
    public override string Texture => AequusTextures.Flare.FullPath;

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
        Projectile.timeLeft = 30;
    }

    public override void AI() {
        Projectile.localAI[0] *= 0.8f;
        Projectile.localAI[0] -= 0.1f;
        if ((int)Projectile.localAI[1] == 0) {
            Projectile.localAI[0] = 10f;
            Projectile.localAI[1] = 1f;
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        if (Projectile.localAI[0] > 0f) {
            Main.EntitySpriteDraw(
                texture,
                Projectile.position + offset - Main.screenPosition,
                frame,
                Color.BlueViolet.SaturationMultiply(0.33f) with { A = 100 } * Projectile.scale,
                MathHelper.PiOver2,
                origin,
                new Vector2(0.5f, Projectile.localAI[0] * 0.5f) * Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
        return false;
    }
}