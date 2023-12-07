using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Common.Items.Tooltips; 

public class Keyword {
    public string header;
    public List<string> tooltipLines;
    public Color textColor;
    public int itemIconId;

    public bool recalculate;
    public int lineMaxWidth;
    public int lineTotalHeight;
    public int[] lineHeights;

    public Keyword(string header, Color? textColor = null, int itemIconId = 0) {
        this.header = header;
        this.textColor = textColor ?? Color.White;
        tooltipLines = new();

        recalculate = true;
        lineMaxWidth = 0;
        lineTotalHeight = 0;
        lineHeights = null;
        this.itemIconId = itemIconId;
    }

    public void AddLine(string line) {
        tooltipLines.Add(line);
        recalculate = true;
    }

    public void Recalculate(DynamicSpriteFont font) {
        recalculate = false;
        lineMaxWidth = 0;
        lineTotalHeight = 0;
        lineHeights = new int[tooltipLines.Count];
        for (int i = 0; i < tooltipLines.Count; i++) {
            tooltipLines[i] = Lang.SupportGlyphs(tooltipLines[i]);
            var measurement = ChatManager.GetStringSize(FontAssets.MouseText.Value, tooltipLines[i], Vector2.One);
            lineMaxWidth = Math.Max((int)measurement.X, lineMaxWidth);
            lineTotalHeight += (int)measurement.Y;
            lineHeights[i] = (int)measurement.Y;
        }
    }

    public int lineX(int lineStartX, int vanillaTooltipBoxWidth) {
        return lineStartX + vanillaTooltipBoxWidth + 26;
    }

    public float headerHalfMeasurementX(DynamicSpriteFont font) {
        return ChatManager.GetStringSize(font, header, Vector2.One).X / 2f;
    }

    public int boxWidth(float headerHalfMeasurementX) {
        return Math.Max(lineMaxWidth, (int)headerHalfMeasurementX * 2 + 10 + (itemIconId > 0 ? 32 : 0));
    }
}