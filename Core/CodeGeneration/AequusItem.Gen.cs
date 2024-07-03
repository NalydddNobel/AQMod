using System.Collections.Generic;

namespace Aequu2;

public partial class AequusItem : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
#if !DEBUG
        UseItemInner(item, player, player.GetModPlayer<AequusPlayer>());
#endif
        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        ModifyTooltipsInner(item, tooltips);
    }
}
