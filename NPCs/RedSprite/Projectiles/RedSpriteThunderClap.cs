using Aequus;
using Aequus.Common.DataSets;
using Aequus.Common.Effects;
using Aequus.Common.Preferences;
using Aequus.Particles.Dusts;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.NPCs.RedSprite.Projectiles;
public class RedSpriteThunderClap : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 4;

        ProjectileSets.DealsHeatDamage.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 24;
        Projectile.hide = true;
    }

    public override void AI() {
        var center = Projectile.Center;
        float minimumLength = 128f;
        byte closest = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
        Projectile.alpha += 10;
        if (Main.player[closest].dead || !Main.player[closest].active) {
            return;
        }
        if (Main.player[closest].position.Y + Main.player[closest].height > center.Y + minimumLength) {
            minimumLength = Main.player[closest].position.Y + Main.player[closest].height - center.Y;
        }
        Projectile.ai[0] = Math.Max(minimumLength, Projectile.ai[0]);
        float maxLength = Projectile.ai[0] + 96f;
        if ((Main.player[Main.myPlayer].Center - new Vector2(center.X, center.Y + Projectile.ai[0])).Length() < 1000f) {
            if ((int)Projectile.localAI[0] == 0) {
                ScreenFlash.Flash.Set(Projectile.Center + new Vector2(0f, Projectile.ai[0]), 0.5f);
                ScreenShake.SetShake(12f, 0.9f);
                for (int i = 0; i < 20; i++) {
                    var d = Dust.NewDustPerfect(center + new Vector2(0f, Projectile.ai[0]), ModContent.DustType<MonoDust>(),
                        Main.rand.NextVector2Unit() * Main.rand.NextFloat(12f) + new Vector2(0f, 8f), newColor: Color.Orange.UseA(0), Scale: Main.rand.NextFloat(0.8f, 3f));
                    d.noGravity = true;
                }
            }
            else if ((int)Projectile.localAI[0] == 1) {
                ScreenFlash.Flash.Set(Projectile.Center + new Vector2(0f, Projectile.ai[0]), 0.75f);
                ScreenShake.SetShake(32f, 0.8f);
                for (int i = 0; i < 50; i++) {
                    var d = Dust.NewDustPerfect(center + new Vector2(0f, Projectile.ai[0]), ModContent.DustType<MonoDust>(),
                        Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(24f), newColor: Color.Orange.UseA(0), Scale: Main.rand.NextFloat(0.8f, 3.5f));
                    d.noGravity = true;
                }
            }
        }
        for (; Projectile.ai[0] < Math.Min(maxLength, 2500f); Projectile.ai[0] += 4f) {
            if (!Collision.CanHit(center, 1, 1, new Vector2(center.X, center.Y + Projectile.ai[0]), 1, 1)) {
                Projectile.ai[0] -= 4f;
                Projectile.localAI[0]++;
                break;
            }
        }

        DelegateMethods.v3_1 = new Vector3(0.8f, 0.5f, 0.1f);
        Utils.PlotTileLine(center, center + new Vector2(0f, Projectile.ai[0]), 1f, DelegateMethods.CastLight);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        var center = Projectile.Center;
        float point = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center,
            center + new Vector2(0f, Projectile.ai[0]), 22, ref point);
    }

    public override Color? GetAlpha(Color drawColor) {
        return new Color(255, 255, 255, 255);
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        behindNPCsAndTiles.Add(index);
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var drawPosition = Projectile.Center + new Vector2(0f, 8f);
        var scale = new Vector2(Projectile.scale, Projectile.scale);
        //scale.X -= 1f - Projectile.timeLeft / 32f;
        lightColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, 0);
        var orig = new Vector2(texture.Width / 2f, frame.Height - 4f);
        //float electric = 2f + ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) + 1f) * 2f;

        int separation = frame.Height - 4;
        var glow = AequusTextures.Bloom4;
        var glowScale = new Vector2(1f, scale.X * 2f);
        var thunderGlowOrig = new Vector2(glow.Width() / 2f, glow.Height() / 2f);
        var glowBright = new Color(200, 140, 30) * (float)Math.Pow(Projectile.Opacity, 3f);
        var glowDark = new Color(80, 20, 2, 0) * (float)Math.Pow(Projectile.Opacity, 3f);

        var rand = LegacyEffects.EffectRand;
        int r = rand.SetRand((int)Projectile.position.X);

        if (Projectile.localAI[0] > 0f)
            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, new Rectangle(0, 0, glow.Width() / 2, glow.Height()), new Color(100, 20, 2, 0), Projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, new Vector2(glowScale.X, glowScale.Y * 1.5f), SpriteEffects.None, 0f);

        if (ClientConfig.Instance.HighQuality) {
            var clr = new Color(255, 100, 0, 20) * (float)Math.Pow(Projectile.Opacity, 16f);
            for (int i = 0; i < 8; i++) {
                rand.SetRand((int)Projectile.position.X);
                float length2 = Projectile.ai[0];
                var off = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                off.X *= scale.X;
                while (true) {
                    var position = drawPosition + new Vector2(0f, length2 - separation) + off;
                    length2 -= separation;
                    if (length2 < separation) {
                        frame.Y = 1 * frame.Height;
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                        frame.Y = 0;
                        Main.spriteBatch.Draw(texture, drawPosition + off - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                        frame.Y = frame.Height * 3;
                        Main.spriteBatch.Draw(texture, drawPosition + off + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                        break;
                    }
                    else {
                        frame.Y = (rand.Rand() / 128 + 1) * frame.Height;
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, clr, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            rand.SetRand((int)Projectile.position.X);
        }

        float length = Projectile.ai[0];

        var glow2 = AequusTextures.Bloom0;
        var glow2Orig = glow2.Size() / 2f;
        Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, -frame.Height / 2f) - Main.screenPosition, null, glowBright, Projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);

        while (true) {
            var position = drawPosition + new Vector2(0f, length - separation);
            length -= separation;
            if (length < separation) {
                frame.Y = 1 * frame.Height;
                Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                frame.Y = 0;
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                frame.Y = frame.Height * 3;
                Main.spriteBatch.Draw(texture, drawPosition + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
                if (Projectile.localAI[0] > 0f) {
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, Projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowDark, Projectile.rotation, glow2Orig, scale * 3f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(glow2, drawPosition + new Vector2(0f, Projectile.ai[0] - frame.Height / 2f) - Main.screenPosition, null, glowBright, Projectile.rotation, glow2Orig, scale * 2f, SpriteEffects.None, 0f);
                }
                break;
            }
            else {
                frame.Y = (rand.Rand() / 128 + 1) * frame.Height;
                Main.spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, orig, scale, SpriteEffects.None, 0f);
            }
        }
        if (Projectile.localAI[0] > 0f)
            Main.spriteBatch.Draw(glow, drawPosition + new Vector2(0f, Projectile.ai[0]) - Main.screenPosition, new Rectangle(0, 0, glow.Width() / 2, glow.Height()), new Color(255, 220, 20, 0), Projectile.rotation + MathHelper.PiOver2, thunderGlowOrig, glowScale, SpriteEffects.None, 0f);

        rand.SetRand(r);
        return false;
    }
}