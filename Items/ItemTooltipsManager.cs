using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class ItemTooltipsManager : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ItemSets.Dedicated.TryGetValue(item.type, out var dedication))
            {
                tooltips.Add(new TooltipLine(Mod, "DedicatedItem", Language.GetTextValue("Mods.AQMod.Tooltips.DedicatedItem")) { overrideColor = dedication.color });
            }
        }
    }
}
