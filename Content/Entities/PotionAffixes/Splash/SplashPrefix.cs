using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Entities.PotionAffixes.Splash;

public class SplashPrefix : UnifiedPotionAffix {
    LocalizedText? AbilityText;

    public override void Load() {
        base.Load();

        AbilityText = this.GetLocalization("Ability");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MorayPollen");
    }

    public override void Apply(Item item) {
        item.useStyle = ItemUseStyleID.Swing;
        item.UseSound = SoundID.Item1;
        item.shoot = ModContent.ProjectileType<SplashPotionProj>();
        item.noUseGraphic = true;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1f;
    }

    public override bool CanRoll(Item item) {
        return AequusItem.IsPotion(item);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return new TooltipLine(Mod, "PrefixSplash", AbilityText!.Value) { IsModifier = true, IsModifierBad = false };
    }
}