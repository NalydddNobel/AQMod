using Aequus.Core;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public class WorkInProgressGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item item, bool lateInstantiation) {
        return item.ModItem != null && item.ModItem.GetType().GetAttribute<WorkInProgressAttribute>() != null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        tooltips.Insert(tooltips.GetIndex("OneDropLogo"), new TooltipLine(Mod, "WorkInProgress", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.WorkInProgress")) { OverrideColor = Color.LightCoral * 1.33f, });
    }
}