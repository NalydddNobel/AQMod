using Aequus.Old.Content.Items.Potions;
using Aequus.Old.Content.Items.Potions.Prefixes;
using System;
using System.Collections.Generic;

namespace Aequus.Old.Content.Items.Potions.Prefixes.EmpoweredPotions;

public class EmpoweredPrefix : PotionPrefix {
    public override void Load() {
        base.Load();
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MistralPollen");
    }

    public override bool CanRoll(Item item) {
        return base.CanRoll(item) && !EmpoweredBuffs.Blacklist.Contains(item.buffType);
    }

    public override void Apply(Item item) {
        item.buffTime = ContentSamples.ItemsByType[item.type].buffTime / 2;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 5f;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        float percent = 1f;
        if (EmpoweredBuffs.Override.TryGetValue(item.buffType, out var buffOverride)) {
            percent = buffOverride.Percent;
        }

        yield return new TooltipLine(Mod, "PrefixEmpowered", this.GetLocalization("Ability")
            .Format(ExtendLanguage.Percent(percent + 1f) + "%")) { IsModifier = true, IsModifierBad = false, };
        yield return new TooltipLine(Mod, "PrefixEmpoweredDownside", this.GetLocalization("Downside")
            .Format("50%")) { IsModifier = true, IsModifierBad = true, };
    }

    public static void CheckUseItem(Item item, Player player, PotionsPlayer potionPlayer) {
        if (PrefixLoader.GetPrefix(item.prefix) is EmpoweredPrefix) {
            potionPlayer.empoweredPotionId = item.buffType;

            // Cap buff time so you can't abuse Stuffed potions.
            int buffIndex = player.FindBuffIndex(item.buffType);
            if (buffIndex != -1) {
                player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], item.buffTime);
            }
        }
        else if (item.buffType == potionPlayer.empoweredPotionId) {
            potionPlayer.empoweredPotionId = 0;
        }
    }
}