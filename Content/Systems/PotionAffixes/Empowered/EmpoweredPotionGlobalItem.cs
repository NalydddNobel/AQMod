using Aequus.Common.DataSets;
using Aequus.Common.Entities.Players;
using Aequus.Common.Items;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Systems.PotionAffixes.Empowered;

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
            if (Instance<EmpoweredDatabase>().Info.TryGetValue(item.buffType, out var info)) {
                tooltips.AddTooltip(new TooltipLine(Mod, "PrefixEmpoweredDesc", info.Tooltip.Value) { IsModifier = true, IsModifierBad = false });
            }
        }
    }
}
