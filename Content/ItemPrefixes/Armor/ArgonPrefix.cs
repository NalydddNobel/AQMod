﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor
{
    public class ArgonPrefix : MossArmorPrefixBase
    {
        public override int MossItem => ItemID.ArgonMoss;

        public int maxDefenseIncrease = 30;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            AddPrefixLine(tooltips, new TooltipLine(Mod, "ArgonPrefixEffect", $"+{(item.defense < maxDefenseIncrease ? 100 : Math.Floor((1f - (item.defense - (float)maxDefenseIncrease) / item.defense) * 100f))}% defense") { IsModifier = true, IsModifierBad = false,});
            AddPrefixLine(tooltips, new TooltipLine(Mod, "ArgonPrefixEffect", $"-{MathHelper.Clamp(item.defense, 0, maxDefenseIncrease)}% damage") { IsModifier = true, IsModifierBad = true, });
        }

        public override void Apply(Item item)
        {
            item.Aequus().defenseChange = (int)MathHelper.Clamp(item.defense, 0, maxDefenseIncrease);
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage(DamageClass.Generic) *= 1f - MathHelper.Clamp(item.defense, 0, maxDefenseIncrease) / 100f;
        }
    }
}