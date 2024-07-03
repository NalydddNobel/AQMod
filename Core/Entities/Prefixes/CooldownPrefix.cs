using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Entities.Items.Components;
using System;
using System.Collections.Generic;

namespace Aequu2.Core.Entities.Prefixes;

[Autoload(false)]
internal class CooldownPrefix : InstancedModPrefix {
    public float cooldownMultiplier;

    public CooldownPrefix(string name, float priceMultiplier, float cooldownMultiplier, StatModifiers statModifiers) : base(name, priceMultiplier, statModifiers) {
        this.cooldownMultiplier = cooldownMultiplier;
    }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override void Load() {
        Aequu2Prefixes.RegisteredCooldownPrefixes.Add(this);
    }

    public override bool CanRoll(Item item) {
        return item.damage > 0 && item.ModItem is ICooldownItem ? base.CanRoll(item) : false;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        string cooldownTime = (cooldownMultiplier > 1f ? "+" : "") + -(int)Math.Round((1f - cooldownMultiplier) * 100f);
        return new TooltipLine[] {
            new(Mod, "PrefixCooldown", this.GetCategoryText("CooldownTip").Format(cooldownTime)) { IsModifier = true, IsModifierBad = cooldownMultiplier > 1f }
        };
    }
}