using Aequus;
using Aequus.NPCs.BossMonsters.Crabson.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Crabson.Projectiles;

public class CrabsonSlamProj : ModProjectile {

    public override string Texture => AequusTextures.None.Path;

    public float[,] effectLookup;

    public override void SetDefaults() {
        Projectile.width = 4000;
        Projectile.height = 1760;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.alpha = 255;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
    }

    public override void AI() {

        int slamEffectTime = 20;
        Projectile.velocity = Vector2.Zero;
        if (Projectile.alpha > 0 && Projectile.timeLeft > slamEffectTime) {
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        var tileCoordinates = Projectile.position.ToTileCoordinates();
        int x = tileCoordinates.X;
        int y = tileCoordinates.Y;
        if (effectLookup == null) {
            int width = Projectile.width / 16;
            int height = Projectile.height / 16;
            effectLookup = new float[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int k = x + i;
                    int l = y + j;
                    if (!WorldGen.InWorld(k, l, 40)) {
                        continue;
                    }

                    if ((Main.tile[k, l].IsFullySolid() || Main.tile[k, l].SolidTopType()) && !Main.tile[k, l - 1].IsFullySolid()) {
                        effectLookup[i, j] = 1f - (Math.Abs(i - width / 2f) / width + Math.Abs(j - height / 2f) / height);
                    }
                }
            }
            return;
        }

        if (Projectile.ai[0] > 0f) {
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, slamEffectTime - 1);
            Projectile.alpha += byte.MaxValue / slamEffectTime;
            int width = Projectile.width / 16;
            int chunk = (int)(width / (float)slamEffectTime * (slamEffectTime - Projectile.timeLeft)) / 2;
            int length = effectLookup.GetLength(0);
            int halfLength = length / 2;
            int start = Math.Max(halfLength - chunk, 0);
            int end = Math.Min(halfLength + chunk, length);
            int height = effectLookup.GetLength(1);
            for (int i = start; i < end; i++) {
                for (int j = 0; j < height; j++) {

                    if (effectLookup[i, j] <= 0f) {
                        continue;
                    }

                    float opacity = Math.Abs(((i - start) / (float)(end - start) - 0.5f) * 2f);
                    var d = Dust.NewDustDirect(new Vector2((i + tileCoordinates.X) * 16f, (j + tileCoordinates.Y) * 16f - 4f), 16, 8, DustID.GemSapphire, 0f, Main.rand.NextFloat(-4f, 0f),
                        Scale: Main.rand.NextFloat(opacity * 3f));
                    d.noGravity = true;
                    d.velocity *= Main.rand.NextFloat() * opacity * 1.5f;
                }
            }
            if (end < length && start > 0) {
                for (int j = 0; j < height; j++) {

                    if (effectLookup[start, j] > 0f) {
                        CrabsonSlamParticle.New(start + tileCoordinates.X, j + tileCoordinates.Y, effectLookup[start, j]);
                        //Helper.DebugDustLine(Main.LocalPlayer.Center, new Vector2(start + tileCoordinates.X, j + tileCoordinates.Y) * 16f, 100);
                    }
                    if (effectLookup[end, j] > 0f) {
                        CrabsonSlamParticle.New(end + tileCoordinates.X, j + tileCoordinates.Y, effectLookup[start, j]);
                    }
                }
            }
        }

        Vector3 tileClr = Color.Blue.ToVector3() * 0.5f * Projectile.Opacity;
        int effectLookupWidth = effectLookup.GetLength(0);
        int effectLookupHeight = effectLookup.GetLength(1);
        for (int i = 0; i < effectLookupWidth; i++) {
            for (int j = 0; j < effectLookupHeight; j++) {
                int k = x + i;
                int l = y + j;
                if (!WorldGen.InWorld(k, l, 40) || ScreenCulling.OnScreenWorld(k, l)) {
                    continue;
                }

                float opacity = effectLookup[i, j];
                Lighting.AddLight(k, l - 1, tileClr.X * opacity, tileClr.Y * opacity, tileClr.Z * opacity);
            }
        }
    }

    public override bool ShouldUpdatePosition() {
        return false;
    }

    public override bool? CanDamage() {
        return Projectile.ai[0] > 0f;
    }
    // Cannot hit NPCs, since it has extremely high range
    public override bool? CanHitNPC(NPC target) {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        if (effectLookup == null || !projHitbox.Intersects(targetHitbox)) {
            return false;
        }

        int myX = (int)Projectile.position.X / 16;
        int myY = (int)Projectile.position.Y / 16;
        int x = Math.Max(targetHitbox.X / 16, myX);
        int y = Math.Max(targetHitbox.Y / 16, myY);
        int width = Math.Max(targetHitbox.Width / 16, 1);
        int height = Math.Max(targetHitbox.Height / 16, 1) + 2;
        int startX = x - myX;
        int startY = y - myY;
        for (int i = x - myX; i < width + startX; i++) {
            if (i > effectLookup.GetLength(0)) {
                break;
            }
            for (int j = y - myY; j < height + startY; j++) {
                if (j > effectLookup.GetLength(1)) {
                    break;
                }

                if (effectLookup[i, j] > 0f) {
                    return true;
                }
            }
        }

        return false;
    }

    private void GetAuraParameters(out Texture2D texture, out Color bloomColor, out Color secondaryBloomColor, out Rectangle frame) {
        bloomColor = Color.Lerp(Color.Cyan, Color.Blue, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.4f, 0.6f)) with { A = 0 } * Projectile.Opacity;
        secondaryBloomColor = Color.Blue with { A = 0 } * 0.5f * Projectile.Opacity;
        frame = new(AequusTextures.Bloom0.Width / 2, 0, 1, AequusTextures.Bloom0.Height / 2);
        texture = AequusTextures.Bloom0.Value;
    }

    private void DrawTileAura_Solid(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame, Vector2 bloomSize, Vector2 largeBloomSize, Vector2 drawCoordinates) {

        if (Main.tile[i, j].IsHalfBlock) {
            drawCoordinates.Y += 8f;
        }

        spriteBatch.Draw(
            texture,
            drawCoordinates + new Vector2(0f, -frame.Height * largeBloomSize.Y + 4f),
            frame,
            secondaryBloomColor * opacity,
            0f,
            Vector2.Zero,
            largeBloomSize,
            SpriteEffects.None, 0f);
        spriteBatch.Draw(
            texture,
            drawCoordinates + new Vector2(0f, -frame.Height * bloomSize.Y + 4f),
            frame,
            bloomColor * opacity,
            0f,
            Vector2.Zero,
            bloomSize,
            SpriteEffects.None, 0f);
    }
    private void DrawTileAura_Sloped(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame, Vector2 bloomSize, Vector2 largeBloomSize, Vector2 drawCoordinates, float dirX, float dirY) {
        for (int k = 0; k < 8; k++) {
            spriteBatch.Draw(
                texture,
                drawCoordinates + new Vector2(k * 2f * dirX, -frame.Height * largeBloomSize.Y + 4f + k * 2f * dirY),
                frame,
                secondaryBloomColor * opacity,
                0f,
                Vector2.Zero,
                largeBloomSize with { X = 2f, },
                SpriteEffects.None, 0f);
            spriteBatch.Draw(
                texture,
                drawCoordinates + new Vector2(k * 2f * dirX, -frame.Height * bloomSize.Y + 4f + k * 2f * dirY),
                frame,
                bloomColor * opacity,
                0f,
                Vector2.Zero,
                bloomSize with { X = 2f, },
                SpriteEffects.None, 0f);
        }
    }

    private void DrawTileAura(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame) {

        int dir = 1;
        if (i > Projectile.Center.X / 16) {
            dir = -1;
        }
        opacity *= Helper.Wave((i + j) * 0.3f + Main.GlobalTimeWrappedHourly * 10f * dir, 1f, 1.5f);
        Vector2 bloomSize = new(16f, opacity * 0.2f);
        Vector2 largeBloomSize = bloomSize with { Y = bloomSize.Y * 1.5f };
        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
        switch (Main.tile[i, j].Slope) {
            case SlopeType.SlopeUpLeft:
            case SlopeType.SlopeUpRight:
            case SlopeType.Solid: {
                    DrawTileAura_Solid(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates);
                    break;
                }

            case SlopeType.SlopeDownLeft: {
                    DrawTileAura_Sloped(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates,
                        1f, 1f);
                    break;
                }
            case SlopeType.SlopeDownRight: {
                    DrawTileAura_Sloped(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates + new Vector2(14f, 0f),
                        -1f, 1f);
                    break;
                }
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        if (effectLookup == null) {
            return false;
        }

        var tileCoordinates = Projectile.position.ToTileCoordinates();
        int x = tileCoordinates.X;
        int y = tileCoordinates.Y;
        int width = effectLookup.GetLength(0);
        int height = effectLookup.GetLength(1);

        GetAuraParameters(out var texture, out var bloomColor, out var secondaryBloomColor, out var frame);
        ScreenCulling.Prepare(16);
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int k = x + i;
                int l = y + j;
                if (!WorldGen.InWorld(k, l, 40) || !ScreenCulling.OnScreenWorld(k, l) || effectLookup[i, j] <= 0f) {
                    continue;
                }

                DrawTileAura(Main.spriteBatch, texture, k, l, effectLookup[i, j], bloomColor, secondaryBloomColor, frame);
            }
        }
        return false;
    }
}