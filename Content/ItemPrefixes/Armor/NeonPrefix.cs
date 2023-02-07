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
            AddPrefixLine(tooltips, new TooltipLine(Mod, "NeonPrefixEffect", $"+{item.defense * 4} health from potions") { IsModifier = true, IsModifierBad = false, });
            AddPrefixLine(tooltips, new TooltipLine(Mod, "NeonPrefixEffect", $"+{item.defense} seconds of potion sickness") { IsModifier = true, IsModifierBad = true, });
        }

        public override void Apply(Item item)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.potionDelayTime += item.defense;
            player.Aequus().extraHealingPotion += item.defense * 4;
        }
    }
}