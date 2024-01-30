using System;
using Terraria.UI.Chat;

namespace Aequus.Common.Items.Components;
public interface IHaveDownsideTip {
}

internal sealed class DownsideTooltips : GlobalItem {
    public const String Identifier = "{v}";

    public override Boolean AppliesToEntity(Item item, Boolean lateInstantiation) {
        return item.ModItem != null && item.ModItem is IHaveDownsideTip;
    }

    public override Boolean PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref Int32 yOffset) {
        if (!line.Name.StartsWith("Tooltip") || !line.Text.StartsWith(Identifier)) {
            return true;
        }

        String realText = line.Text[Identifier.Length..];
        Single intensity = Helper.Oscillate(Math.Max((Main.mouseTextColor - 220) / 35f, 0f) * MathHelper.PiOver2, 1f);
        var lineColor = Color.Lerp(line.OverrideColor ?? line.Color, Color.Red, intensity * 0.5f);
        var shadowColor = Color.Lerp(Color.Black, Color.Red, intensity * 0.3f);
        var drawCoordinates = new Vector2(line.X, line.Y) + Main.rand.NextVector2Square(-2f, 2f) * Math.Max(intensity - 0.5f, 0f) * line.BaseScale;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, realText, drawCoordinates, lineColor, shadowColor, line.Rotation, line.Origin, line.BaseScale);
        return false;
    }
}