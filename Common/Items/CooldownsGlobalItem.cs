using Aequus.Common.Items.Components;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public sealed class CooldownsGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is ICooldownItem;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (item.ModItem is ICooldownItem cooldownItem) {
            tooltips.AddTooltip(new(Mod, "CooldownTip", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Cooldown", TextHelper.Seconds(item.GetCooldownTime()))));
        }
    }
}