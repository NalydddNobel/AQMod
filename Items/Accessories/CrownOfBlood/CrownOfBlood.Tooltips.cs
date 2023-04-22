using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBloodItem {
        public static LocalizedText GetBoostTooltip(Item item) {
            return item.ModItem != null
                ? Language.GetText("Mods." + item.ModItem.Mod.Name + ".Items." + item.ModItem.Name + ".BoostTooltip")
                : Language.GetText("Mods.Aequus.BoostTooltips." + ItemID.Search.GetName(item.type));
        }

        public static bool GetCrownOfBloodTooltip(Item item, out string tooltip) {
            tooltip = "";
            if (!item.accessory || item.vanity) {
                return false;
            }

            List<string> text = new();
            if (item.wingSlot > 0) {
                text.Add(AequusLocalization.Wings);
            }
            if (item.defense > 0) {
                text.Add("+" + item.defense + " defense");
            }
            if (NoBoost.Contains(item.type)) {
                goto ModifyTooltipsLabel;
            }

            var boostTooltip = GetBoostTooltip(item);
            if (boostTooltip.Key == boostTooltip.Value) {
                int lines = item.ToolTip.Lines;
                string wingsLine = Language.GetTextValue("CommonItemTooltip.FlightAndSlowfall");

                for (int i = 0; i < lines; i++) {
                    string line = item.ToolTip.GetLine(i);
                    if (line == wingsLine || line.StartsWith('\'')) {
                        continue;
                    }

                    text.Add(line);
                }
            }
            else {
                text.Add(boostTooltip.Value);
            }

        ModifyTooltipsLabel:
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

        internal static void ModifyEquipTooltip(Item item, List<TooltipLine> tooltips) {
            if (!item.accessory || item.vanity || item.createTile > -1 || item.type == ModContent.ItemType<CrownOfBloodItem>() || Helper.iterations != 0 || Main.LocalPlayer.Aequus().accCrownOfBlood == null) {
                return;
            }

            Helper.iterations++;
            string text = GetCrownOfBloodTooltip(item, out string tooltip)
                ? tooltip
                : AequusLocalization.NoItem;
            tooltips.Add(new(Aequus.Instance, "CrownOfBlood", Lang.GetItemName(ModContent.ItemType<CrownOfBloodItem>()) + ": " + text) {
                OverrideColor = Color.PaleVioletRed,
            });
            Helper.iterations--;
        }
    }
}