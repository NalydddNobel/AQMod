using Aequus.Common.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Anchor;

public class AnchorNecklace : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
    }
}