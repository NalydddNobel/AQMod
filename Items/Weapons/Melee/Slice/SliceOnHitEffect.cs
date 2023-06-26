using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Slice;

public class SliceOnHitEffect : ModProjectile {
    public override string Texture => AequusTextures.Flare.Path;

    private bool _playedSound;

    public static int SpawnOnNPC(Projectile projectile, NPC target) {
        var v = Main.rand.NextVector2Unit();
        var size = target.Size;
        size.X = Math.Max(size.X, 80f);
        size.Y = Math.Max(size.Y, 80f);
        return Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center - v * size, v * size / 8f,
            ModContent.ProjectileType<SliceOnHitEffect>(), projectile.damage, projectile.knockBack, projectile.owner);
    }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 16;
    }

    public override void AI() {
        if (!_playedSound) {
            _playedSound = true;
            SoundEngine.PlaySound(AequusSounds.hit_Sword.Sound with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3, }, Projectile.Center);
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        float intensity = MathF.Pow(MathF.Sin(Projectile.timeLeft / 16f * MathHelper.Pi), 2f);
        var color = new Color(20, 200, 255, 0) * intensity;
        Main.EntitySpriteDraw(
            texture,
            Projectile.position + offset - Main.screenPosition,
            frame,
            color,
            Projectile.velocity.ToRotation() + MathHelper.PiOver2,
            origin,
            new Vector2(0.8f, 2f), SpriteEffects.None, 0
        );
        Main.EntitySpriteDraw(
            texture,
            Projectile.position + offset - Main.screenPosition,
            frame,
            color,
            Projectile.velocity.ToRotation() + MathHelper.PiOver2,
            origin,
            new Vector2(1f, 3f), SpriteEffects.None, 0
        );
        return false;
    }
}