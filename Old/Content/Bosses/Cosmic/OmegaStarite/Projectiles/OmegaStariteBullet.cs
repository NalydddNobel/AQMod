﻿using Aequus.Content.DataSets;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Particles;
using System;
using Terraria.GameContent;

namespace Aequus.Old.Content.Bosses.Cosmic.OmegaStarite.Projectiles;

public class OmegaStariteBullet : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileMetadata.PushableByAI.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 360;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI() {
        if (Projectile.ai[0] < 0f) {
            byte plr = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[plr].Center - Projectile.Center) * Projectile.ai[1], 0.015f);
            Projectile.ai[0]++;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Main.rand.NextBool(12)) {
            int d = Dust.NewDust(Projectile.Center + new Vector2(5f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + Projectile.velocity.ToRotation()), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerColors.CosmicEnergy, 0.75f);
            Main.dust[d].velocity = Projectile.velocity * 0.1f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Projectile.type].Value;
        var orig = texture.Size() / 2f;
        var drawPos = Projectile.Center - Main.screenPosition;
        var drawColor = GlimmerColors.CosmicEnergy;
        drawColor.A = 0;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);

        DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.Trail2, Projectile.oldPos, Projectile.oldRot,
            (p) => GlimmerColors.CosmicEnergy with { A = 0 } * (1f - p),
            (p) => Projectile.width - p * Projectile.width,
            -Main.screenPosition + Projectile.Size / 2f);

        float intensity = 0f;
        float playerDistance = (Main.player[Main.myPlayer].Center - Projectile.Center).Length();
        if (playerDistance < 480f) {
            intensity = 1f - playerDistance / 480f;
        }

        if (intensity > 0f) {
            var spotlight = AequusTextures.BloomStrong;
            var spotlightOrig = spotlight.Size() / 2f;
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.25f, Projectile.rotation, spotlightOrig, Projectile.scale * intensity, SpriteEffects.None, 0f);
            spotlight = AequusTextures.Bloom;
            spotlightOrig = spotlight.Size() / 2f;
            var crossScale = new Vector2(0.04f * intensity, (3f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 16f) * 0.2f) * intensity);
            var spotlightDrawColor = drawColor * 0.2f;
            spotlightDrawColor = Color.Lerp(spotlightDrawColor, new Color(128, 128, 128, 0), 0.3f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            crossScale.X *= 2f;
            crossScale.Y *= 1.5f;
            Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, 0f, spotlightOrig, crossScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightDrawColor * 0.25f, MathHelper.PiOver2, spotlightOrig, crossScale, SpriteEffects.None, 0f);
        }
        Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 255), Main.GameUpdateCount / 240f, orig, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) {
        float veloRot = Projectile.velocity.ToRotation();
        var velo = Projectile.velocity * 0.5f;
        for (int i = 0; i < 25; i++) {
            int d = Dust.NewDust(Projectile.Center + new Vector2(6f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + veloRot), 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerColors.CosmicEnergy, 0.75f);
            Main.dust[d].velocity = velo;
        }
    }
}