using Aequus.Common.Drawing;
using Aequus.Common.Utilities;
using Aequus.Common.Graphics;
using System;
using Terraria.GameContent;

namespace Aequus.Content.Monsters.PollutedOcean.BlackJellyfish;

public partial class BlackJellyfish : IDrawOntoLayer, RenderTargetRequests.IRenderTargetRequest {
    private const int LightningSegments = 36;
    private Vector2[] lightningDrawCoordinates;
    private float[] lightningDrawRotations;

    public Point Dimensions => new Point(150, 150);
    public RenderTarget2D RenderTarget { get; set; }
    public bool IsDisposed { get; set; }
    public bool Active { get; set; }

    public override Color? GetAlpha(Color drawColor) {
        return drawColor * GetLightMagnitude();
    }

    public override void DrawBehind(int index) {
        if (NPC.ai[2] > 0f) {
            Instance<RenderTargetRequests>().Request(this);
            DrawLayers.Instance.PostDrawLiquids += NPC;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var drawCoordinates = NPC.Center;
        float opacity = NPC.Opacity;
        var origin = NPC.frame.Size() / 2f;
        //origin.X += 1f;
        //origin.Y += 6f;
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        if (!NPC.IsABestiaryIconDummy) {
            if (NPC.ai[2] > ShockAttackLength) {
                return false;
            }
            else {
                opacity *= 1f - Math.Min(NPC.ai[2] / ShockAttackLength, 1f);
            }
        }
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates - screenPos, NPC.frame, NPC.GetAlpha(drawColor) * opacity * 0.92f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(AequusTextures.BlackJellyfish_Bag, drawCoordinates - screenPos, NPC.frame, drawColor * opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }

    public void DrawOntoLayer(SpriteBatch spriteBatch, DrawLayer layer) {
        var drawCoordinates = NPC.Center - Main.screenPosition;
        Vector2 origin = NPC.frame.Size() / 2f;
        float attackProgress = Math.Min(NPC.ai[2] / ShockAttackLength, 1f);
        float attackRange = MathF.Pow(attackProgress, 2f) * AttackRange;

        if (NPC.ai[2] > ShockAttackLength) {
            float deathAnimation = (NPC.ai[2] - ShockAttackLength) / 14f;
            if (deathAnimation < 0.5f) {
                attackRange += MathF.Sin(deathAnimation / 0.5f * MathHelper.Pi) * 16f;
            }
            else {
                attackRange *= 1f - (deathAnimation - 0.5f) / 0.5f;
            }
        }

        Color lightningColor = new Color(255, 200, 100, 10);
        float attackRangeNormalized = attackRange / AttackRange;

        if (NPC.ai[2] < ShockAttackLength) {
            Color color = Color.Black;
            if (NPC.ai[2] % 8 < 4) {
                color = lightningColor;
            }
            drawCoordinates += Main.rand.NextVector2Square(-attackProgress, attackProgress) * 4f;
            float npcScale = NPC.scale + attackProgress * 0.3f;
            for (int i = 0; i < 4; i++) {
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates + new Vector2(2f * NPC.scale, 0f).RotatedBy(i * MathHelper.PiOver2 + NPC.rotation), NPC.frame, lightningColor with { A = 60 } * attackProgress, NPC.rotation, origin, npcScale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, NPC.frame, color * attackProgress, NPC.rotation, origin, npcScale, SpriteEffects.None, 0f);
        }
        else {
            spriteBatch.Draw(AequusTextures.BloomStrong, drawCoordinates, null, lightningColor * attackRangeNormalized, 0f, AequusTextures.BloomStrong.Size() / 2f, MathF.Pow(attackRange / AttackRange, 1.5f), SpriteEffects.None, 0f);
            spriteBatch.Draw(AequusTextures.Bloom, drawCoordinates, null, lightningColor * attackRangeNormalized, 0f, AequusTextures.Bloom.Size() / 2f, MathF.Pow(attackRange / AttackRange, 3f) * 1.25f, SpriteEffects.None, 0f);
        }

        if (RenderTarget != null && !RenderTarget.IsDisposed && !RenderTarget.IsContentLost) {
            Color color = !Main.zenithWorld ? Color.White : Main.DiscoColor;
            drawCoordinates = NPC.Center - Main.screenPosition;
            origin = RenderTarget.Size() / 2f;
            spriteBatch.Draw(RenderTarget, drawCoordinates, null, color, Main.GlobalTimeWrappedHourly, origin, 0.9f, SpriteEffects.None, 0f);

            int rings = 5;
            for (int i = 0; i < rings; i++) {
                float pulseAnimation = ((Main.GlobalTimeWrappedHourly) * 3f + i / (float)rings) % 1f;
                spriteBatch.Draw(RenderTarget, drawCoordinates, null, color * MathF.Sin(pulseAnimation * MathHelper.Pi), Main.GlobalTimeWrappedHourly + i * 10f, origin, (1f - MathF.Pow(1f - pulseAnimation, 2f)) * 0.8f + 0.2f, SpriteEffects.None, 0f);
            }
        }
    }

    public void DrawOntoRenderTarget(GraphicsDevice graphics, SpriteBatch sb) {
        if (NPC == null || !NPC.active) {
            Active = false;
            return;
        }

        graphics.Clear(Color.Transparent);

        var drawCoordinates = RenderTarget.Size() / 2f;

        if (lightningDrawCoordinates == null) {
            lightningDrawCoordinates = new Vector2[LightningSegments];
            lightningDrawRotations = new float[LightningSegments];
        }

        float attackProgress = Math.Min(NPC.ai[2] / ShockAttackLength, 1f);
        float attackRange = MathF.Pow(attackProgress, 2f) * AttackRange;
        if (NPC.ai[2] > ShockAttackLength) {
            float deathAnimation = (NPC.ai[2] - ShockAttackLength) / 14f;
            if (deathAnimation < 0.5f) {
                attackRange += MathF.Sin(deathAnimation / 0.5f * MathHelper.Pi) * 16f;
            }
            else {
                attackRange *= 1f - (deathAnimation - 0.5f) / 0.5f;
            }
        }
        for (int i = 0; i < lightningDrawCoordinates.Length; i++) {
            float rotation = i * MathHelper.TwoPi / (lightningDrawCoordinates.Length - 1);
            lightningDrawCoordinates[i] = drawCoordinates + new Vector2(attackRange, 0f).RotatedBy(rotation) + Main.rand.NextVector2Square(-attackProgress, attackProgress) * 6f;
            lightningDrawRotations[i] = rotation - MathHelper.PiOver2;
        }

        Vector2 origin = NPC.frame.Size() / 2f;
        Color lightningColor = new Color(255, 200, 100, 10);
        float attackRangeNormalized = attackRange / AttackRange;

        DrawHelper.ApplyBasicEffect(DrawHelper.View, DrawHelper.Projection, AequusTextures.BlackJellyfishVertexStrip);
        DrawHelper.VertexStrip.PrepareStrip(lightningDrawCoordinates, lightningDrawRotations,
            p => lightningColor * attackRangeNormalized * NPC.Opacity,
            p => Math.Max(attackRangeNormalized < 1f ? attackRangeNormalized : MathF.Pow(attackRangeNormalized, 1.5f), 0.25f) * NPC.Opacity * 8f,
            Vector2.Zero, includeBacksides: true);
        DrawHelper.VertexStrip.DrawTrail();

        //DrawHelper.DrawBasicVertexLine(AequusRemakeTextures.BlackJellyfishVertexStrip, lightningDrawCoordinates, lightningDrawRotations,
        //    p => lightningColor * attackRangeNormalized * NPC.Opacity,
        //    p => Math.Max(attackRangeNormalized < 1f ? attackRangeNormalized : MathF.Pow(attackRangeNormalized, 1.5f), 0.25f) * NPC.Opacity * 8f
        //);
    }

    public void Dispose() {
        if (RenderTarget != null && !RenderTarget.IsDisposed) {
            RenderTarget.Dispose();
            RenderTarget = null;
        }

        IsDisposed = true;
        Active = false;

        GC.SuppressFinalize(this);
    }

    ~BlackJellyfish() {
        if (!IsDisposed) {
            Dispose();
        }
    }
}