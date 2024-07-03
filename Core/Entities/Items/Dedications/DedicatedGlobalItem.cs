using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequu2.Core.Entities.Items.Dedications;

public sealed class DedicatedGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return DedicationRegistry.TryGet(item.type, out _);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!DedicationRegistry.TryGet(item.type, out IDedicationInfo info)) {
            return;
        }

        tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "DedicatedItem", info.GetDedicatedLine().Value) { OverrideColor = info.TextColor });
    }

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
        if (line.Name != "DedicatedItem") {
            return true;
        }

        DrawDedicatedTooltip(line);
        return false;
    }

    public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line) {
        if (line.Mod == "Terraria" && line.Name == "ItemName") {
            DrawDedicatedItemName(line);
        }
    }

    public static void DrawDedicatedItemName(DrawableTooltipLine line) {
        const int SparklesFrameCount = 5;

        ulong seed = (ulong)Math.Abs(Main.LocalPlayer.name.GetHashCode());
        var coords = new Vector2(line.X, line.Y);
        var measurement = line.Font.MeasureString(line.Text) * line.BaseScale;

        var bloomTexture = AequusTextures.Bloom.Value;
        var bloomOrigin = bloomTexture.Size() / 2f;
        Vector2 bloomScale = new Vector2(measurement.X / bloomTexture.Width, (measurement.Y / bloomTexture.Height + 0.1f) / 5f);
        float bloomPulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f);
        Color bloomColor = (line.Color.SaturationSet(1f).HueShift(0.1f)) with { A = 0 } * 0.23f;
        Vector2 textCenter = coords - line.Origin / 2f + measurement / 2f + new Vector2(0f, -4f);
        Main.spriteBatch.Draw(bloomTexture, textCenter, null, bloomColor, 0f, bloomOrigin, bloomScale + new Vector2(0.8f, 0.4f), SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(bloomTexture, textCenter, null, bloomColor, 0f, bloomOrigin, bloomScale + new Vector2(2.5f, 0f), SpriteEffects.None, 0f);

        Texture2D texture = AequusTextures.Sparkles;
        Vector2 origin = new Vector2(6f, 5f);
        int sparkleCount = (int)measurement.X / 2;
        for (int i = 0; i < sparkleCount; i++) {
            Color sparkleColor = line.Color.HueAdd(Helper.Oscillate(i, -0.05f, 0.05f)) with { A = 0 } * 1.2f;
            float timer = i / measurement.X * 8f + Main.GlobalTimeWrappedHourly * 0.25f;
            float timerWrapped = timer % 2f;
            SpriteEffects spriteEffects = Utils.RandomInt(ref seed, 2) == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle frame = texture.Frame(verticalFrames: SparklesFrameCount, frameY: Utils.RandomInt(ref seed, 4));
            Vector2 sparklePosition = coords + new Vector2(Utils.RandomInt(ref seed, (int)measurement.X), Utils.RandomInt(ref seed, (int)measurement.Y / 2) + 4f);

            if (timerWrapped > 1f) {
                continue;
            }

            if (timerWrapped < 0.5f) {
                timerWrapped = MathF.Pow(timerWrapped * 2f, 1.5f) / 2f;
            }
            frame.Height -= 2;
            float scale = MathF.Sin(timerWrapped * MathHelper.Pi);
            Main.spriteBatch.Draw(
                texture,
                sparklePosition + new Vector2(Helper.Oscillate(timer * 11f, -4f, 3f), MathF.Pow(timerWrapped * 2f, 2f) * -12f + 12f),
                frame,
                sparkleColor * MathF.Pow(scale, 30f),
                0f,
                origin,
                scale * Helper.Oscillate(timer * 20f, 0.5f, 1.1f),
                spriteEffects,
                0f
            );
        }
    }

    public static void DrawDedicatedTooltip(DrawableTooltipLine line) {
        string text = line.Text;
        var color = line.OverrideColor.GetValueOrDefault(line.Color);
        float rotation = line.Rotation;
        var origin = line.Origin;
        var baseScale = line.BaseScale;
        var coords = new Vector2(line.X, line.Y);

        float brightness = Main.mouseTextColor / 255f;
        float brightnessProgress = (Main.mouseTextColor - 190f) / (byte.MaxValue - 190f);
        color = Colors.AlphaDarken(color) with { A = 0 };
        var font = FontAssets.MouseText.Value;
        ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, coords, Color.Black, rotation, origin, baseScale);
        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f) {
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords + (f + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (brightnessProgress * 1f), color, rotation, origin, baseScale);
        }
    }
}