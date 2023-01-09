using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes
{
    public abstract class AequusPrefix : ModPrefix
    {
        public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
        }
    }
}