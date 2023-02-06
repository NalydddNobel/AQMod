using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor
{
    public class NeonPrefix : MossArmorPrefixBase
    {
        public override int MossItem => ItemID.XenonMoss;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            AddPrefixLine(tooltips, new TooltipLine(Mod, "NeonPrefixEffect", $"+{item.defense * 2} health from potions") { IsModifier = true, IsModifierBad = false, });
            AddPrefixLine(tooltips, new TooltipLine(Mod, "NeonPrefixEffect", $"+{item.defense} seconds of potion sickness") { IsModifier = true, IsModifierBad = true, });
        }
    }
}