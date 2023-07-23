using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Global {
    public class WIPItems : GlobalItem {
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
}