using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Common.Items.Tooltips;

public partial class KeywordGlobalItem : GlobalItem {
    private void CheckLinesInit(Item item) {
        // Init lines if hovering over a new item type
        if (KeywordSystem.LastHoveredItemID != item.type || (Main.mouseLeft && Main.mouseLeftRelease) || (Main.mouseRight && Main.mouseRightRelease)) {
            KeywordSystem.Tooltips.Clear(); // Clear line cache
            KeywordSystem.AddItemKeywords(item);
            KeywordSystem.AddPlayerSpecificKeywords(Main.LocalPlayer, item);
        }
        KeywordSystem.HoveredItemID = item.type;
    }

    private int GetVanillaTooltipBoxWidth(IEnumerable<TooltipLine> lines, DynamicSpriteFont font) {
        int max = 0;
        foreach (var l in lines) {
            max = Math.Max(max, (int)ChatManager.GetStringSize(font, l.Text, Vector2.One).X);
        }
        return max;
    }

    private void DrawKeyword(SpriteBatch spriteBatch, DynamicSpriteFont font, int i, int vanillaTooltipBoxY, int vanillaTooltipBoxWidth, ref int lineStartX, ref int lineStartY, ref int lineDirX, ref int lineDirY, ref int largestBoxWidth, ref int previousBoxHeight) {
        var keyword = KeywordSystem.Tooltips[i];
        // Recalculate tooltip box if needed
        if (keyword.recalculate) {
            keyword.Recalculate(font);
        }

        int boxHeight = keyword.lineTotalHeight + 40;
        if (i > 0) {
            if (lineDirY != -1) {
                if (lineStartY + previousBoxHeight + boxHeight > Main.screenHeight) {
                    lineDirY = -1;
                    lineStartY = vanillaTooltipBoxY - boxHeight - 4;
                }
                else {
                    lineStartY += previousBoxHeight + 4;
                }
            }
            else if (lineStartY - boxHeight < 0) {
                lineDirY = 1;
                lineStartX += lineDirX * (largestBoxWidth + 24);
                lineStartY = vanillaTooltipBoxY;
            }
            else {
                lineStartY -= boxHeight + 4;
            }
        }
        previousBoxHeight = boxHeight;

        int lineX = keyword.lineX(lineStartX, vanillaTooltipBoxWidth);

        // Header values for proper placement
        float headerHalfMeasurementX = keyword.headerHalfMeasurementX(font);
        float headerMinX = headerHalfMeasurementX + 6f;

        int boxWidth = keyword.boxWidth(headerHalfMeasurementX);
        largestBoxWidth = Math.Max(boxWidth, largestBoxWidth);
        // Swap box direction to the other side if we're trying to draw outside of the screen
        if (lineX + boxWidth > Main.screenWidth) {
            lineDirX = -1;
        }
        if (lineDirX == -1) {
            lineX = lineStartX - boxWidth - 26;
        }

        // Draw tooltip box
        if (Main.SettingsEnabled_OpaqueBoxBehindTooltips) {
            Utils.DrawInvBG(spriteBatch, new(lineX - 10, lineStartY - 9, boxWidth + 20, keyword.lineTotalHeight + 40), new Color(23, 25, 81, 255) * 0.925f);
        }

        // Draw item icon, if there is one
        int itemIconId = keyword.itemIconId;
        if (itemIconId > 0) {
            // offset the header's minimum X position
            headerMinX += 32f;

            Main.GetItemDrawFrame(itemIconId, out var texture, out var frame);
            float scale = 1f;
            int largestSide = Math.Max(texture.Width, texture.Height);
            if (largestSide > 32f) {
                scale = 32f / largestSide;
            }
            spriteBatch.Draw(texture, new Vector2(lineX - 2f, lineStartY - 2f), frame, Main.inventoryBack * 0.8f, 0f, Vector2.Zero, scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
        }

        // Draw header
        ChatManager.DrawColorCodedStringWithShadow(
            spriteBatch,
            font,
            keyword.header,
            new Vector2(lineX + Math.Max(boxWidth / 2f, headerMinX), lineStartY),
            keyword.textColor * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 1.5f, 2f),
            0f,
            new Vector2(headerHalfMeasurementX, 0f),
            Vector2.One
        );

        // Draw lines
        int textLineY = lineStartY + 32;
        for (int j = 0; j < keyword.tooltipLines.Count; j++) {
            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                font,
                keyword.tooltipLines[j],
                new Vector2(lineX, textLineY),
                keyword.textColor,
                0f,
                Vector2.Zero,
                Vector2.One
            );
            textLineY += keyword.lineHeights[j];
        }
    }

    public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
        CheckLinesInit(item);

        // Exit if there are no lines to render, or if the player is holding up (this hopefully prevents conflicts with SLR's keyword system)
        if (KeywordSystem.Tooltips.Count == 0 || Main.LocalPlayer.controlUp) {
            return true;
        }

        var spriteBatch = Main.spriteBatch;
        var font = FontAssets.MouseText.Value;

        int vanillaTooltipBoxWidth = GetVanillaTooltipBoxWidth(lines, font);

        int lineStartX = x;
        int lineStartY = y;
        int lineDirX = 1;
        int lineDirY = 1;
        int largestBoxWidth = 0;
        int previousBoxHeight = 0;
        for (int i = 0; i < KeywordSystem.Tooltips.Count; i++) {
            var keyword = KeywordSystem.Tooltips[i];
            // Swap box direction to the other side if we're trying to draw outside of the screen
            if (keyword.lineX(lineStartX, vanillaTooltipBoxWidth) + keyword.boxWidth(keyword.headerHalfMeasurementX(font)) > Main.screenWidth) {
                lineDirX = -1;
            }
        }
        for (int i = 0; i < KeywordSystem.Tooltips.Count; i++) {
            DrawKeyword(spriteBatch, font, i, y, vanillaTooltipBoxWidth, ref lineStartX, ref lineStartY, ref lineDirX, ref lineDirY, ref largestBoxWidth, ref previousBoxHeight);
        }
        return true;
    }
}