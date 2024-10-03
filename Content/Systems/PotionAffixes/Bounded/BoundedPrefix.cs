using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Systems.PotionAffixes.Bounded;

public class BoundedPrefix : UnifiedPotionAffix {
    LocalizedText? Ability;

    public override void Load() {
        base.Load();

        Ability = this.GetLocalization("Ability");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "ManaclePollen");
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.1f;
    }

    public override bool CanRoll(Item item) {
        return AequusItem.IsPotion(item) && !Main.persistentBuff[item.buffType];
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return new TooltipLine(Mod, "PrefixBounded", Ability!.Value) { IsModifier = true, IsModifierBad = false, };
    }
}