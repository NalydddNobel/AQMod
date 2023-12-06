using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

// TODO -- Make inherit bullet effects ?
public class EtOmniaVanitasBullet : ModProjectile {
    public EtOmniaVanitasParticle.Spawner _particleSpawner;

    public override string Texture => AequusTextures.Projectile(ProjectileID.Bullet);

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = 0;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.timeLeft = 3600;
        Projectile.alpha = 255;
        _particleSpawner = new();
    }

    public override void AI() {
        if (Projectile.alpha == 255) {
            Projectile.extraUpdates += (int)(Projectile.velocity.Length() / 16f);
            Projectile.velocity /= Projectile.MaxUpdates;
        }
        int chance = Projectile.MaxUpdates;
        float speed = Projectile.velocity.Length() * Projectile.MaxUpdates;
        float chanceMultiplier = Math.Max(8f - MathF.Pow(speed / 20f, 2f), 2f);
        int finalChance = Math.Max((int)(Projectile.MaxUpdates * chanceMultiplier - Projectile.localAI[0]), 2);
        if (Main.rand.NextBool(finalChance) && Projectile.alpha < 220) {
            _particleSpawner.New(Projectile.Center, Main.rand.NextVector2Square(-speed, speed) * 0.06f);
            Projectile.localAI[0] = 0f;
        }
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 20;
            if (Projectile.alpha <= 0) { 
                Projectile.alpha = 0; 
            }
        }
        Projectile.localAI[0]++;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(int timeLeft) {
        _particleSpawner.New(Projectile.Center, Vector2.Normalize(Projectile.velocity));
        _particleSpawner.Clear();
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Extra[ExtrasID.BlackBolt].Value;
        Projectile.GetDrawInfo(out _, out var offset, out _, out _, out int trailLength);

        var drawCoordinates = Projectile.position + offset - Main.screenPosition;
        var scale = new Vector2(0.2f, 1f);
        var trailColor = new Color(50, 100, 140, 100) * Projectile.Opacity;
        for (int i = 0; i < trailLength; i++) {
            float progress = 1f - 1f / trailLength * i;
            Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2() * 2f, null, trailColor * 0.6f * progress, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale * progress, SpriteEffects.None, 0f);
        }
        for (int i = 0; i < 4; i++) {
            Main.EntitySpriteDraw(texture, drawCoordinates - Projectile.velocity + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2() * 2f, null, trailColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
        }
        Main.EntitySpriteDraw(texture, drawCoordinates, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, scale * 0.8f, SpriteEffects.None, 0f);
        return false;
    }
}