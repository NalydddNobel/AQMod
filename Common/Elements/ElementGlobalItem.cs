using System.Collections.Generic;

namespace Aequus.Common.Elements;

public class ElementGlobalItem : GlobalItem {
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.damage <= 0) {
            return;
        }

        string text = "";
        foreach (Element e in Main.LocalPlayer.GetModPlayer<ElementalPlayer>().visibleElements) {
            if (!e.ContainsItem(item.type)) {
                continue;
            }
            if (!string.IsNullOrEmpty(text)) {
                text += ", ";
            }

            text += ChatTagWriter.Color(Colors.AlphaDarken(e.Color), e.DisplayName.Value);
        }

        if (!string.IsNullOrEmpty(text)) {
            tooltips.Add(new TooltipLine(Mod, "Elements", $"Elements: {text}"));
        }
    }
}
