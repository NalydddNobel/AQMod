using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Global {
    public class UnusedItems : GlobalItem {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) {
            return item.ModItem != null && item.ModItem.GetType().GetAttribute<UnusedContentAttribute>() != null;
        }

        public override void SetDefaults(Item item) {
            item.rare = -1;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            tooltips.Insert(
                tooltips.GetIndex("OneDropLogo"),
                new TooltipLine(Mod, "Unused", TextHelper.GetTextValue("Items.Unused")) { OverrideColor = Color.LightCyan, }
            );
        }
    }
}