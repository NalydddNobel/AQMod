using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBlood {
        public static Dictionary<int, List<TooltipInfo>> UnnecessaryTooltips = new();

        public record struct TooltipInfo(string Mod, string Name) {
            public TooltipInfo(string Name) : this("Terraria", Name) { }
        }

        private void LoadUnnecessaryTooltips() {

        }

        public static bool ExtractTooltip(Item item, out string tooltip) {
            tooltip = "";
            int lines = item.ToolTip.Lines;
            List<string> text = new();
            for (int i = 0; i < lines; i++) {
                text.Add(item.ToolTip.GetLine(i));
            }

            int numTooltips = text.Count;
            var names = new string[numTooltips];
            var textArray = text.ToArray();
            var modifier = new bool[numTooltips];
            var badModifier = new bool[numTooltips];
            int oneDropLogo = -1;
            for (int i = 0; i < numTooltips; i++) {
                names[i] = "Tooltip" + i;
                modifier[i] = false;
                badModifier[i] = false;
            }
            var tooltipLines = ItemLoader.ModifyTooltips(item, ref numTooltips, names, ref textArray, ref modifier, ref badModifier, ref oneDropLogo, out var overrideColor);

            if (UnnecessaryTooltips.TryGetValue(item.netID, out var unnecessaryLines)) {
                foreach (var l in unnecessaryLines) {
                    for (int i = 0; i < tooltipLines.Count; i++) {
                        if (tooltipLines[i].Name == l.Name && tooltipLines[i].Mod == l.Mod) {
                            break;
                        }
                    }
                }
            }

            if (tooltipLines.Count <= 0) {
                return false;
            }

            foreach (var t in tooltipLines) {
                if (!t.Name.Contains("Tooltip"))
                    continue;

                if (!string.IsNullOrEmpty(tooltip)) {
                    tooltip += "\n";
                }

                tooltip += t.Text;
            }
            return true;
        }
    }
}
