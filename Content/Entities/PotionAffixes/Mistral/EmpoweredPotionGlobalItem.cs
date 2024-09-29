using Aequus.Common.DataSets;
using Aequus.Common.Entities.Players;
using Aequus.Common.Items;
using Aequus.Content.ItemPrefixes.Potions;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Entities.PotionAffixes.Mistral;

public class EmpoweredPotionGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsPotion.Contains(entity.type);
    }

    public override bool CanUseItem(Item item, Player player) {
        if (PlayerHooks.QuickBuff && item.buffType > 0 && item.buffTime > 0 && player.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer)) {
            return PrefixLoader.GetPrefix(item.prefix) is not EmpoweredPrefix && potionPlayer.empowered == 0;
        }

        return true;
    }

    public override bool? UseItem(Item item, Player player) {
        if (!player.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer)) {
            return null;
        }

        if (PrefixLoader.GetPrefix(item.prefix) is EmpoweredPrefix) {
            potionPlayer.empowered = item.buffType;

            // Cap buff time so you can't abuse Stuffed potions.
            int buffIndex = player.FindBuffIndex(item.buffType);
            if (buffIndex != -1) {
                player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], item.buffTime);
            }
        }
        else if (item.buffType == potionPlayer.empowered) {
            potionPlayer.empowered = 0;
        }

        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (PrefixLoader.GetPrefix(item.prefix) is EmpoweredPrefix) {
            if (Instance<EmpoweredPotionGlobalBuff>().CustomTooltip.TryGetValue(item.buffType, out LocalizedText tooltip)) {
                tooltips.AddTooltip(new TooltipLine(Mod, "TooltipEmpowered", tooltip.Value));
            }
        }
    }
}
