using Aequus.Old.Content.Particles;
using Terraria.GameContent;

namespace Aequus.Old.Content.Necromancy.Sceptres.Evil;

public class CrimsonSceptreProj : CorruptionSceptreProj {
    public override string Texture => AequusTextures.CorruptionSceptreProj.Path;

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 140, 100, 255 - Projectile.alpha);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            UpdateTrail();

            int trailCount = _trail.GetLength(0);

            for (int i = 0; i < trailCount; i++) {
                var p = ModContent.GetInstance<LegacyBloomParticle>().New();
                p.Location = _trail[i][0];
                p.Velocity = Projectile.velocity * -0.125f;
                p.Color = new Color(150, 30, 10, 10) * Projectile.Opacity;
                p.BloomColor = Color.Red with { A = 0 } * 0.015f * Projectile.Opacity;
                p.Scale = 1.5f;
                p.BloomScale = 0.5f;
            }
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        int target = Projectile.FindTargetWithLineOfSight(300f);
        if (target != -1) {
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.075f)) * speed;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(int timeLeft) {
        var center = Projectile.Center;
        for (int i = 0; i < 12; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(255, 210, 200, 150));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var origin = frame.Size() / 2f;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(255, 40, 10, 100), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
        return false;
    }
}