using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Systems.PotionAffixes.Empowered;

public class EmpoweredPrefix : UnifiedPotionAffix {
    LocalizedText? AbilityText;
    LocalizedText? DownsideText;

    public override void Load() {
        base.Load();

        AbilityText = this.GetLocalization("Ability");
        DownsideText = this.GetLocalization("Downside").WithFormatArgs(ALanguage.Percent(0.5f));
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MistralPollen");
    }

    public override void Apply(Item item) {
        item.buffTime /= 2;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 2f;
    }

    public override bool CanRoll(Item item) {
        return Instance<EmpoweredDatabase>().Info.ContainsKey(item.buffType);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        if (Instance<EmpoweredDatabase>().Info.TryGetValue(item.buffType, out var info)) {
            yield return new TooltipLine(Mod, "PrefixEmpowered", AbilityText!.Format(ALanguage.Percent(info.Percent))) { IsModifier = true, IsModifierBad = false };
            yield return new TooltipLine(Mod, "PrefixEmpoweredBuffDuration", DownsideText!.Value) { IsModifier = true, IsModifierBad = true };
        }
    }
}
