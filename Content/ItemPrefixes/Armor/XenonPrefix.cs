using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor
{
    public class XenonPrefix : MossArmorPrefixBase
    {
        public override int MossItem => ItemID.XenonMoss;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            AddPrefixLine(tooltips, new TooltipLine(Mod, "XenonPrefixEffect", "+1 minion slot") { IsModifier = true, IsModifierBad = false, });
            AddPrefixLine(tooltips, new TooltipLine(Mod, "XenonPrefixEffect", "-200% defense") { IsModifier = true, IsModifierBad = true, });
        }
    }
}