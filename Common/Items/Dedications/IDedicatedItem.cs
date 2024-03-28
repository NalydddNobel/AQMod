using System.Linq;
using Terraria.Localization;

namespace Aequus.Common.Items.Dedications;

public interface IDedicatedItem {
    string DedicateeName { get; }
    string DisplayedDedicateeName => DedicateeName;
    Color TextColor { get; }
    public Color FaelingColor => TextColor;

    public TooltipLine GetLine() {
        return new TooltipLine(Aequus.Instance, "DedicatedItem",
            Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.DedicatedItem", DedicateeName)) { OverrideColor = TextColor, };
    }
}