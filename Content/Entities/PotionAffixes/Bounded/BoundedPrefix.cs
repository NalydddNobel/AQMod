using System.Collections.Generic;

namespace Aequus.Content.Entities.PotionAffixes.Bounded;

public class BoundedPrefix : UnifiedPotionAffix {
    public override void Load() {
        base.Load();

        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "ManaclePollen");
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.1f;
    }

    public override bool CanRoll(Item item) {
        return AequusItem.IsPotion(item) && !Main.persistentBuff[item.buffType];
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return new TooltipLine(Aequus.Instance, "PrefixBounded", TextHelper.GetTextValue("Prefixes.BoundedPrefix.Ability")) { IsModifier = true, IsModifierBad = false, };
    }
}