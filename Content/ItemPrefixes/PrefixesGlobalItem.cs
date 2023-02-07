using Aequus.Content.ItemPrefixes;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items;

public partial class AequusItem : GlobalItem
{
    public void UpdateEquip_Prefixes(Item item, Player player)
    {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix aequusPrefix)
        {
            aequusPrefix.UpdateEquip(item, player);
        }
    }

    public void UpdateAccessory_Prefixes(Item item, Player player, bool hideVisual)
    {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix aequusPrefix)
        {
            aequusPrefix.UpdateAccessory(item, player, hideVisual);
        }
    }

    public void ModifyTooltips_Prefixes(Item item, List<TooltipLine> tooltips)
    {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix aequusPrefix)
        {
            aequusPrefix.ModifyTooltips(item, tooltips);
        }
    }
}