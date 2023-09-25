using Aequus.Common.Items.Components;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusItem : GlobalItem {
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        var player = Main.LocalPlayer;
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (item.ModItem is ICooldownItem cooldownItem) {
            tooltips.AddTooltip(new(Mod, "CooldownTip", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Cooldown", TextHelper.Seconds(item.GetCooldownTime()))));
        }
        Tooltip_Monocle(item, tooltips, player, aequusPlayer);
    }
}