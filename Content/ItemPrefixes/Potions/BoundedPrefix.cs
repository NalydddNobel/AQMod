using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions
{
    public class BoundedPrefix : AequusPrefix
    {
        public override void Apply(Item item)
        {
            item.Aequus().prefixPotionsBounded = true;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item)
        {
            return AequusItem.IsPotion(item) && !Main.persistentBuff[item.buffType];
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, "PrefixBounded", TextHelper.GetTextValue("Prefixes.BoundedPotion"))
            { IsModifier = true, IsModifierBad = false, });
        }
    }
}