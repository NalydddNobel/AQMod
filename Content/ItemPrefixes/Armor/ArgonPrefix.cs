using Microsoft.Xna.Framework;
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

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            AddPrefixLine(tooltips, new TooltipLine(Mod, "ArgonPrefixEffect", $"+{(item.defense < 30 ? 100 : Math.Floor((1f - (item.defense - 30f) / item.defense) * 100f))}% defense") { IsModifier = true, IsModifierBad = false,});
            AddPrefixLine(tooltips, new TooltipLine(Mod, "ArgonPrefixEffect", $"-{MathHelper.Clamp(item.defense, 0, 30)}% damage") { IsModifier = true, IsModifierBad = true, });
        }
    }
}