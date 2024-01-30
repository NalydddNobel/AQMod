using Aequus.Common.Items.Components;
using System;
using System.Collections.Generic;

namespace Aequus.Common.ItemPrefixes;

[Autoload(false)]
public class CooldownPrefix : InstancedPrefix {
    public Single cooldownMultiplier;

    public CooldownPrefix(String name, Single priceMultiplier, Single cooldownMultiplier, StatModifiers statModifiers) : base(name, priceMultiplier, statModifiers) {
        this.cooldownMultiplier = cooldownMultiplier;
    }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override void Load() {
        AequusPrefixes.RegisteredCooldownPrefixes.Add(this);
    }

    public override Boolean CanRoll(Item item) {
        return item.damage > 0 && item.ModItem is ICooldownItem ? base.CanRoll(item) : false;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        String cooldownTime = (cooldownMultiplier > 1f ? "+" : "") + -(Int32)Math.Round((1f - cooldownMultiplier) * 100f);
        return new TooltipLine[] {
            new(Mod, "PrefixCooldown", this.GetCategoryText("CooldownTip").Format(cooldownTime)) { IsModifier = true, IsModifierBad = cooldownMultiplier > 1f }
        };
    }
}