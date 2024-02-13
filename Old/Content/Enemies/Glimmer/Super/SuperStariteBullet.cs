﻿using Aequus.Content.DataSets;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Enemies.Glimmer.Super;

public class SuperStariteBullet : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
        ProjectileMetadata.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 300;
        Projectile.extraUpdates = 5;
    }

    public override Color? GetAlpha(Color lightColor) {
        return GlimmerColors.CosmicEnergy;
    }

    public override void AI() {
        if (Projectile.ai[0] <= 0f) {
            Projectile.ai[0] = Projectile.ai[0] * -1 + 1f;
            Projectile.velocity *= 0.2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        switch ((int)Projectile.ai[0]) {
            case 2:
                Projectile.velocity *= 0.99f;
                if (Projectile.velocity.Length() < 0.25f) {
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 2);
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        for (int i = 0; i < 5; i++) {

                        }
                    }
                }
                break;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
        var color = Projectile.GetAlpha(lightColor);
        int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
        var origin = texture.Size() / 2f;
        var bloomFrame = new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height);
        var bloomColor = new Color(40, 20, 255, color.A) * 0.4f;
        for (int i = 0; i < trailLength; i++) {
            float p = 1f - i / (float)trailLength;
            Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset, bloomFrame, Color.Lerp(bloomColor * 10f, bloomColor, 1f - p) * p, Projectile.oldRot[i], origin, Projectile.scale * 1.2f * p, SpriteEffects.None, 0f);
        }
        Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, Projectile.position + offset, bloomFrame, color * 0.5f, Projectile.rotation, origin, Projectile.scale + 0.5f, SpriteEffects.None, 0f);

        var bloom = AequusTextures.Bloom;
        Main.spriteBatch.Draw(bloom, Projectile.position + offset, null, bloomColor, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.6f, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) {
        SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.35f, Pitch = 0.25f, PitchVariance = 0.1f }, Projectile.Center);
        for (int i = 0; i < 20; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: GlimmerColors.CosmicEnergy);
            d.color = new Color(d.color.R + Main.rand.Next(-100, 0), d.color.G + Main.rand.Next(-100, 0), d.color.B, d.color.A);
            d.velocity *= Main.rand.NextFloat(0.4f, 1.5f);
            d.velocity += -Projectile.oldVelocity * Main.rand.NextFloat(0.5f, 2f);
            d.scale *= Main.rand.NextFloat(0.8f, 1.5f);
        }
    }
}