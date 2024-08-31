using ReLogic.Utilities;
using System.Collections.Generic;

namespace Aequus.Common.Items.Global;

public class WIPItems : GlobalItem {
#if DEBUG
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif

    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return item.ModItem != null && item.ModItem.GetType().GetAttribute<WorkInProgressAttribute>() != null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.Insert(
            tooltips.GetIndex("OneDropLogo"),
            new TooltipLine(Mod, "WorkInProgress", TextHelper.GetTextValue("Items.WorkInProgress")) { OverrideColor = Color.LightCoral * 1.33f, }
        );
    }
}