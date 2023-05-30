using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor {
    public class XenonPrefix : MossArmorPrefixBase
    {
        public override int MossItem => ItemID.XenonMoss;

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            return new[] {
                new TooltipLine(Mod, "XenonPrefixEffect", "+1 minion slot") 
                { IsModifier = true, IsModifierBad = false, },

                new TooltipLine(Mod, "XenonPrefixEffect2", "Defense is negative") 
                { IsModifier = true, IsModifierBad = true, }
            };
        }

        public override void Apply(Item item)
        {
            item.Aequus().defenseChange = -item.defense * 2;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.maxMinions++;
        }
    }
}