using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class BoundedPrefix : AequusPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

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
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, "PrefixBounded", AequusText.GetText("Prefixes.BoundedPotion"))
            { IsModifier = true, IsModifierBad = false, });
        }
    }
}