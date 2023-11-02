using Aequus.Common.Items;
using Aequus.Core;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Anchor;

[WorkInProgress]
public class AnchorNecklace : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
    }
}