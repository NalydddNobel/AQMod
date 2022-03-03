using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Reforges
{
    public abstract class AQPrefix : ModPrefix
    {
        public virtual void ModifyTooltips(Item item, AQItem aQItem, AQPlayer aQPlayer, List<TooltipLine> tooltips)
        {
        }
    }
}