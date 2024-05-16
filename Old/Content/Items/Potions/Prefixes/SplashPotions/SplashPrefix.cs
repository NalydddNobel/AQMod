using Aequus.Old.Content.Items.Potions.Prefixes;
using System.Collections.Generic;

namespace Aequus.Old.Content.Items.Potions.Prefixes.SplashPotions;

public class SplashPrefix : PotionPrefix {
    private TooltipLine _tooltipLine;

    public override void Load() {
        base.Load();
        ModTypeLookup<ModItem>.RegisterLegacyNames(Item, "MorayPollen");
    }

    public override void Apply(Item item) {
        item.useStyle = ItemUseStyleID.Swing;
        item.UseSound = SoundID.Item1;
        item.shoot = ModContent.ProjectileType<SplashPotionProj>();
        item.noUseGraphic = true;
    }

    public override bool CanRoll(Item item) {
        return base.CanRoll(item) && ContentSamples.ItemsByType[item.type].shoot == ProjectileID.None;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.1f;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        yield return _tooltipLine ??=
            new TooltipLine(Mod, "PrefixSplash", this.GetLocalizedValue("Ability")) { IsModifier = true, IsModifierBad = false, };
    }
}