using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequu2.Old.Content.TownNPCs.OccultistNPC;

public class OccultistProj : ModProjectile {
    public override string Texture => Aequu2Textures.Projectile(ProjectileID.DemonScythe);

    public override void SetDefaults() {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.npcProj = true;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.alpha = 255;
        Projectile.scale = 0.8f;
        Projectile.timeLeft = 120;
    }

    public override void AI() {
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 25;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        if (Projectile.localAI[0] == 0) {
            Projectile.localAI[0] = 1f;
            SoundEngine.PlaySound(SoundID.Item8 with { Pitch = 0.6f, Volume = 0.5f }, Projectile.Center);
            for (int i = 0; i < 32; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.7f, 1.5f));
                d.noGravity = true;
            }
        }
        else if (Main.rand.NextBool()) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.6f, 1.1f));
            d.noGravity = true;
        }
        if (Projectile.ai[1] == 0f) {
            Projectile.ai[1] = Projectile.velocity.Length() * 3f;
        }
        if (Projectile.alpha <= 50) {
            var target = Projectile.FindTargetWithinRange(700f);
            if (target != null) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * Math.Max(Projectile.velocity.Length(), 5f), 0.05f + Projectile.ai[0] * 0.01f);
            }
        }
        if (Projectile.ai[0] < 10f) {
            Projectile.ai[0] += 0.25f;
        }
        else {
            if (Projectile.velocity.Length() < Projectile.ai[1]) {
                Projectile.velocity *= 1.1f;
            }
            Projectile.ai[0]++;
        }
        Projectile.rotation += 0.7f;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
    }

    public override void OnKill(int timeLeft) {
        SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        for (int i = 0; i < 32; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.7f, 1.5f));
            d.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        lightColor = new Color(255, 222, 211, 200) * Projectile.Opacity;
        Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
        Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}
