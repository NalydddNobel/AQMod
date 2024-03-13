using System;
using System.Collections.Generic;

namespace Aequus.Old.Content.Potions.Prefixes.BoundedPotions;

public class BoundedPrefix : PotionPrefix {
    private TooltipLine _tooltipLine;

    public override void Load() {
        base.Load();
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "ManaclePollen");
    }

    public override bool CanRoll(Item item) {
        return base.CanRoll(item) && !Main.persistentBuff[item.buffType];
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.5f;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return _tooltipLine ??=
            new TooltipLine(Mod, "PrefixBounded", this.GetLocalizedValue("Ability")) { IsModifier = true, IsModifierBad = false, };
    }

    public static void CheckUseItem(Item item, Player player, PotionsPlayer potionPlayer) {
        if (PrefixLoader.GetPrefix(item.prefix) is BoundedPrefix && !Main.persistentBuff[item.buffType]) {
            potionPlayer.BoundedPotionIds.Add(item.buffType);

            // Cap buff time so you can't abuse Stuffed potions.
            int buffIndex = player.FindBuffIndex(item.buffType);
            if (buffIndex != -1) {
                player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], item.buffTime);
            }
        }
        else {
            potionPlayer.BoundedPotionIds.Remove(item.buffType);
        }
    }
}