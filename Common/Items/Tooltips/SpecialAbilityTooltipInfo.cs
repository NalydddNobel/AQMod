﻿using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Items.Tooltips {
    public class SpecialAbilityTooltipInfo {
        public string header;
        public List<string> tooltipLines;
        public Color textColor;
        public int itemIconId;

        public bool recalculate;
        public int lineMaxWidth;
        public int lineTotalHeight;
        public int[] lineHeights;

        public SpecialAbilityTooltipInfo(string header, Color? textColor = null, int itemIconId = 0) {
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
                var measurement = font.MeasureString(tooltipLines[i]);
                lineMaxWidth = Math.Max((int)measurement.X, lineMaxWidth);
                lineTotalHeight += (int)measurement.Y;
                lineHeights[i] = (int)measurement.Y;
            }
        }
    }
}