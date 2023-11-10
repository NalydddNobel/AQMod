using Aequus.Content.DedicatedContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Common.Items;

public sealed class DedicatedItemGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return item.ModItem is IDedicatedItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is not IDedicatedItem dedicatedItem) {
            return;
        }

        string name = dedicatedItem.DisplayedDedicateeName;
        string text = string.IsNullOrEmpty(name) ? Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.DedicatedItemAnonymous") : Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.DedicatedItem", name);
        tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "DedicatedItem", text) { OverrideColor = dedicatedItem.TextColor, });
    }

    public static void DrawDedicatedItemName(DrawableTooltipLine line) {
        const int SparklesFrameCount = 5;

        ulong seed = (ulong)Math.Abs(Main.LocalPlayer.name.GetHashCode());
        var coords = new Vector2(line.X, line.Y);
        var measurement = line.Font.MeasureString(line.Text) * line.BaseScale;

        var bloomTexture = AequusTextures.BloomStrong.Value;
        var bloomOrigin = bloomTexture.Size() / 2f;
        var bloomScale = new Vector2(measurement.X / bloomTexture.Width + 2f, (measurement.Y / bloomTexture.Height + 0.1f) / 3f);
        float bloomPulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f);
        var bloomColor = (line.Color.SaturationSet(1f).HueMultiply(0.8f)) with { A = 0 } * 0.15f;
        var textCenter = coords - line.Origin / 2f + measurement / 2f + new Vector2(0f, -4f);
        Main.spriteBatch.Draw(
            bloomTexture,
            textCenter,
            null,
            bloomColor,
            0f,
            bloomOrigin,
            bloomScale,
            SpriteEffects.None,
            0f
        );
        Main.spriteBatch.Draw(
            bloomTexture,
            textCenter,
            null,
            bloomColor * 0.5f * bloomPulse,
            Helper.Oscillate(Main.GlobalTimeWrappedHourly * 3.65f, -0.05f, 0.05f),
            bloomOrigin,
            bloomScale * bloomPulse * 1.2f,
            SpriteEffects.None,
            0f
        );

        var texture = AequusTextures.Sparkles;
        var origin = new Vector2(6f, 5f);
        int sparkleCount = (int)measurement.X / 5;
        for (int i = 0; i < sparkleCount; i++) {
            var sparkleColor = line.Color.HueAdd(Helper.Oscillate(i, -0.05f, 0.05f)) with { A = 0 } * 1.2f;
            float timer = (i / measurement.X * 8f + Main.GlobalTimeWrappedHourly * 0.2f);
            float timerWrapped = timer % 2f;
            var spriteEffects = Utils.RandomInt(ref seed, 2) == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var frame = texture.Frame(verticalFrames: SparklesFrameCount, frameY: Utils.RandomInt(ref seed, 4));
            var sparklePosition = coords + new Vector2(Utils.RandomInt(ref seed, (int)measurement.X), Utils.RandomInt(ref seed, (int)measurement.Y / 2) + 4f);

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
}