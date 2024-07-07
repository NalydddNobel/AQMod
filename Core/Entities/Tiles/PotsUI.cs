using AequusRemake.Core.GUI;
using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace AequusRemake.Core.Entities.Tiles;

public class PotsUI : UILayer {
    public override bool OnUIUpdate(GameTime gameTime) {
        return PotsSystem.LootPreviews.Count > 0;
    }

    protected override bool DrawSelf() {
        lock (PotsSystem.LootPreviews) {
            foreach (var preview in PotsSystem.LootPreviews) {
                DrawPreview(preview.Key, preview.Value);
            }
        }
        return true;
    }

    private static void DrawPreview(Point tileCoordinates, PotsSystem.PotLootPreview preview) {
        var seed = Helper.TileSeed(tileCoordinates) % 10000f;

        float scale = Math.Min(preview.Opacity, sin(Main.GlobalTimeWrappedHourly * 5f + seed, 0.9f, 1f));
        float pulseScale = scale;

        var frame = preview.Frame ?? preview.Texture.Bounds;
        int largestSide = Math.Max(frame.Width, frame.Height);
        if (largestSide > 24f) {
            scale *= 24f / largestSide;
        }

        var drawCoordinates = new Vector2(tileCoordinates.X * 16f + 16f, tileCoordinates.Y * 16f + 20f) - Main.screenPosition;
        var itemWobbleOffset = new Vector2(sin(Main.GlobalTimeWrappedHourly * 3f + seed * 0.9f, -1f, 1f), sin(Main.GlobalTimeWrappedHourly * 1.2f + seed * 0.8f, -2f, 2f));
        float rotation = sin(Main.GlobalTimeWrappedHourly * 4.2f, -0.1f, 0.1f);
        float opacity = 1f;
        bool dangerView = preview.Dangerous && Main.LocalPlayer.dangerSense;
        if (dangerView) {
            opacity *= sin(Main.GlobalTimeWrappedHourly * 5f + seed, 0.3f, 1f);
        }
        Main.spriteBatch.Draw(AequusTextures.BloomStrong, drawCoordinates, null, Color.Black * opacity * (dangerView ? 0.33f : 0.75f) * preview.Opacity, 0f, AequusTextures.BloomStrong.Size() / 2f, 0.4f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(preview.Texture, drawCoordinates + itemWobbleOffset + new Vector2(2f) * scale, frame, Color.Black * 0.33f * opacity * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(preview.Texture, drawCoordinates + itemWobbleOffset, frame, Color.White * 0.75f * opacity * pulseScale * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        if (preview.NPCColor != Color.Transparent) {
            Main.spriteBatch.Draw(preview.Texture, drawCoordinates + itemWobbleOffset, frame, preview.NPCColor * 0.75f * opacity * pulseScale * preview.Opacity, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        int sparkleCount = HighQualityEffects ? 6 : 3;
        var sparkleColor = preview.Dangerous ? Color.Red : Color.Orange;
        for (int i = 0; i < sparkleCount; i++) {
            float timer = seed + (i * (seed + 500) + Main.GlobalTimeWrappedHourly * 1.1f) + i / (float)sparkleCount;
            var random = new FastRandom((int)timer);
            timer %= 1f;
            if (timer > 1f) {
                continue;
            }

            timer = MathF.Pow(timer, random.NextFloat(1f, 2.5f));
            random.NextSeed();
            var sparkleOffset = new Vector2(random.NextFloat(-12f, 12f), random.NextFloat(-12f, 12f) + 4f);
            var sparkleFrame = AequusTextures.BaseParticleTexture.Frame(verticalFrames: 3, frameY: random.Next(3));
            float sparkleFade = MathF.Sin(timer * MathHelper.Pi);
            Main.spriteBatch.Draw(AequusTextures.BaseParticleTexture, drawCoordinates + new Vector2(0f, -timer * 4f) + sparkleOffset, sparkleFrame, sparkleColor with { A = 0 } * sparkleFade * 0.45f * preview.Opacity, 0f, sparkleFrame.Size() / 2f, sparkleFade * random.NextFloat(1f, 1.5f), SpriteEffects.None, 0f);
        }

        if (preview.Stack > 1) {
            ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, preview.Stack.ToString(), drawCoordinates + new Vector2(-12f, -2f), Color.White * 0.66f * preview.Opacity, 0f, Vector2.Zero, Vector2.One * 0.66f);
        }
    }

    public PotsUI() : base("Angler Lamp", InterfaceLayerNames.EntityHealthBars_16, InterfaceScaleType.Game) { }
}