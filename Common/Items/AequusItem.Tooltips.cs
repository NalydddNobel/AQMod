using Aequus.Common.Items.Components;
using Aequus.Core.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public partial class AequusItem : GlobalItem {
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is ICooldownItem cooldownItem) {
            tooltips.AddTooltip(new(Mod, "CooldownTip", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Cooldown", TextHelper.Seconds(item.GetCooldownTime()))));
        }
    }
}