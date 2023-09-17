using Aequus.Common.Items.Components;
using Aequus.Core.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.ItemModifiers.Cooldowns;

public abstract class CooldownPrefixBase : ModPrefix {
    public abstract float CooldownMultiplier { get; }

    public override PrefixCategory Category => PrefixCategory.Custom;

    public override bool CanRoll(Item item) {
        return item.damage > 0 && item.ModItem is ICooldownItem ? base.CanRoll(item) : false;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
        string cooldownTime = (CooldownMultiplier > 1f ? "+" : "") + - (int)Math.Round((1f - CooldownMultiplier) * 100f);
        return new TooltipLine[] {
            new(Mod, "PrefixCooldown", this.GetCategoryText("CooldownTip").Format(cooldownTime)) { IsModifier = true, IsModifierBad = CooldownMultiplier > 1f }
        };
    }
}