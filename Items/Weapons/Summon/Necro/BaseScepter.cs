using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public abstract class BaseScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            SacrificeTotal = 1;
        }

        public override bool AllowPrefix(int pre)
        {
            return !AequusItem.CritOnlyModifier.Contains(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                tooltips.RemoveCritChanceModifier();
            }
            catch
            {
            }
        }
    }
}
