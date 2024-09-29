using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Entities.PotionAffixes.Stuffed;

[LegacyName("DoubledTimePrefix")]
public class StuffedPrefix : UnifiedPotionAffix {
    LocalizedText? AbilityText;

    public override void Load() {
        base.Load();

        AbilityText = this.GetLocalization("Ability");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MoonflowerPollen");
    }

    public override void Apply(Item item) {
        item.buffTime *= 2;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.1f;
    }

    public override bool CanRoll(Item item) {
        return AequusItem.IsPotion(item) && !Main.persistentBuff[item.buffType];
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        Item defaultItem = ContentSamples.ItemsByType[item.type];

        if (item.buffTime != defaultItem.buffTime) {
            yield return new TooltipLine(Mod, "PrefixStuffed", AbilityText!.Format(ALanguage.Percent(item.buffTime / (double)defaultItem.buffTime))) { IsModifier = true, IsModifierBad = false };
        }
    }
}