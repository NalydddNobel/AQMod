using Aequu2.DataSets;
using System.Collections.Generic;

namespace Aequu2.Old.Content.Items.Potions.Prefixes.StuffedPotions;

[LegacyName("DoubledTimePrefix")]
public class StuffedPrefix : PotionPrefix {
    public override void Load() {
        base.Load();
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MoonflowerPollen");
    }

    public override void Apply(Item item) {
        item.buffTime = ContentSamples.ItemsByType[item.type].buffTime * 2;
    }

    public override bool CanRoll(Item item) {
        return base.CanRoll(item) && !BuffDataSet.CannotChangeDuration.Contains(item.buffType);
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.5f;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        Item defaultItem = ContentSamples.ItemsByType[item.type];
        if (item.buffTime != defaultItem.buffTime || defaultItem.buffTime == 0) {
            float percent = item.buffTime / (float)defaultItem.buffTime;

            yield return new TooltipLine(Mod, "PrefixStuffed", this.GetLocalization("Ability")
                .Format(XLanguage.Percent(percent) + "%")) { IsModifier = true, IsModifierBad = item.buffTime < defaultItem.buffTime, };
        }
    }
}