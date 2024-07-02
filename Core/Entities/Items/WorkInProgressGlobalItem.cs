using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Core.Entities.Items;

public sealed class WorkInProgressGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return item.ModItem != null && item.ModItem.GetType().GetAttribute<WorkInProgressAttribute>() != null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "WorkInProgress", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.WorkInProgress")) { OverrideColor = Color.LightCoral * 1.33f, });
    }
}