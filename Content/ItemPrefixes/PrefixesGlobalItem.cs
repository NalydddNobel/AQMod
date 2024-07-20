using Aequus.Content.ItemPrefixes;

namespace Aequus;

public partial class AequusItem : GlobalItem {
    private void UpdateEquip_Prefixes(Item item, Player player) {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix aequusPrefix) {
            aequusPrefix.UpdateEquip(item, player);
        }
    }

    private void UpdateAccessory_Prefixes(Item item, Player player, bool hideVisual) {
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix aequusPrefix) {
            aequusPrefix.UpdateAccessory(item, player, hideVisual);
        }
    }
}