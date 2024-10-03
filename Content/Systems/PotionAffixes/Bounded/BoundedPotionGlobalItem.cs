using Aequus.Common.DataSets;
using System;

namespace Aequus.Content.Systems.PotionAffixes.Bounded;

internal class BoundedPotionGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsPotion.Contains(entity.type);
    }

    public override bool? UseItem(Item item, Player player) {
        if (!player.TryGetModPlayer(out BoundedPotionPlayer potionPlayer)) {
            return null;
        }

        if (PrefixLoader.GetPrefix(item.prefix) is BoundedPrefix && !Main.persistentBuff[item.buffType]) {
            potionPlayer.bounded.Add(item.buffType);

            // Cap buff time so you can't abuse Stuffed potions.
            int buffIndex = player.FindBuffIndex(item.buffType);
            if (buffIndex != -1) {
                player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], item.buffTime);
            }
        }
        else {
            potionPlayer.bounded.Remove(item.buffType);
        }

        return null;
    }
}
