using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Common.Items.Tooltips;

public class Keyword {
    public String header;
    public List<String> tooltipLines;
    public Color textColor;
    public Int32 itemIconId;

    public Boolean recalculate;
    public Int32 lineMaxWidth;
    public Int32 lineTotalHeight;
    public Int32[] lineHeights;

    public Keyword(String header, Color? textColor = null, Int32 itemIconId = 0) {
        this.header = header;
        this.textColor = textColor ?? Color.White;
        tooltipLines = new();

        recalculate = true;
        lineMaxWidth = 0;
        lineTotalHeight = 0;
        lineHeights = null;
        this.itemIconId = itemIconId;
    }

    public void AddLine(String line) {
        tooltipLines.Add(line);
        recalculate = true;
    }

    public void Recalculate(DynamicSpriteFont font) {
        recalculate = false;
        lineMaxWidth = 0;
        lineTotalHeight = 0;
        lineHeights = new Int32[tooltipLines.Count];
        for (Int32 i = 0; i < tooltipLines.Count; i++) {
            var measurement = ChatManager.GetStringSize(FontAssets.MouseText.Value, tooltipLines[i], Vector2.One);
            lineMaxWidth = Math.Max((Int32)measurement.X, lineMaxWidth);
            lineTotalHeight += (Int32)measurement.Y;
            lineHeights[i] = (Int32)measurement.Y;
        }
    }
}