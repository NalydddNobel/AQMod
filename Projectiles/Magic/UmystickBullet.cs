using Aequus.Common.Graphics.Primitives;
using Aequus.Content;
using Aequus.Particles.Dusts;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Projectiles.Magic;
public class UmystickBullet : ModProjectile {
    public static SoundStyle UmystickDestroyed => AequusSounds.UmystickBreak;

    protected TrailRenderer prim;
    protected Color _glowColorCache;

    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 3;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        PushableEntities.AddProj(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.extraUpdates = 2;
        Projectile.scale = 0.85f;
        Projectile.timeLeft = 120;
    }

    public override void AI() {
        if ((int)Projectile.ai[0] == 0) {
            Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
            Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            _glowColorCache = new Color(128, 70, 70, 0);
            if (Projectile.frame == 1)
                _glowColorCache = new Color(90, 128, 50, 0);
            else if (Projectile.frame == 2)
                _glowColorCache = new Color(70, 70, 128, 0);

            for (int i = 0; i < 10; i++) {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, _glowColorCache * 2f, 1f);
            }
        }
        if ((int)Projectile.ai[0] < 40f) {
            Projectile.ai[0]++;
            if (Projectile.ai[1] > 40f) {
                Projectile.ai[0] = 0;
            }

            int target = Projectile.FindTargetWithLineOfSight(450f);
            if (target != -1) {
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, (Main.npc[target].Center - Projectile.Center) * 0.1f, 0.015f)) * Projectile.velocity.Length();
            }
        }
        Projectile.ShimmerReflection();
        if (Main.rand.NextBool(9)) {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, _glowColorCache * 2f, 1f);
        }
        Projectile.rotation += Projectile.velocity.Length() * Main.rand.NextFloat(0.01f, 0.0157f);
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
        var origin = frame.Size() / 2f;
        var center = Projectile.Center;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        if (prim == null) {
            prim = new TrailRenderer(TrailTextures.Trail[1].Value, TrailRenderer.DefaultPass,
                (p) => new Vector2((float)Math.Pow(1f - p, 2f) * 16f) * Projectile.scale, GetTrailColor,
                drawOffset: Projectile.Size / 2f);
        }
        new TrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass,
            (p) => new Vector2((1f - p) * 12f) * Projectile.scale, (p) => GetTrailColor(p) * 4f,
            drawOffset: Projectile.Size / 2f).Draw(Projectile.oldPos);
        prim.Draw(Projectile.oldPos);
        //Main.spriteBatch.Draw(TextureCache.Bloom[0].Value, center - Main.screenPosition, null, _glowClr, Projectile.rotation, TextureCache.Bloom[0].Value.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(AequusTextures.Bloom0, center - Main.screenPosition, null, Color.White, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.3f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(AequusTextures.Bloom0, center - Main.screenPosition, null, _glowColorCache, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 250, 250, 160), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }

    public Color GetTrailColor(float progress) {
        return (_glowColorCache * 2f).UseA(20).HueAdd((Projectile.frame != 0 ? progress : -progress) * 0.33f) * (1f - progress);
    }

    public override void OnKill(int timeLeft) {
        var center = Projectile.Center;
        float size = Projectile.width / 2f;
        if (Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(UmystickDestroyed, Projectile.Center);
        }
        for (int i = 0; i < 30; i++) {
            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>());
            var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
            Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
            Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
            Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
            Main.dust[d].color = _glowColorCache * Main.rand.NextFloat(0.8f, 2f);
        }
    }
}