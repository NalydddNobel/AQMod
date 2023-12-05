using Aequus.Common.Items.Components;
using Aequus.Content.ItemPrefixes;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.ItemPrefixes;

[Autoload(false)]
public class CooldownPrefix : InstancedPrefix {
    public float cooldownMultiplier;

    public CooldownPrefix(string name, float priceMultiplier, float cooldownMultiplier, StatModifiers statModifiers) : base(name, priceMultiplier, statModifiers) {
        this.cooldownMultiplier = cooldownMultiplier;
    }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override void Load() {
        AequusPrefixes.RegisteredCooldownPrefixes.Add(this);
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