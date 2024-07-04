using System.Collections.Generic;

namespace AequusRemake;

public partial class AequusItem : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        ModifyTooltipsInner(item, tooltips);
    }
}
