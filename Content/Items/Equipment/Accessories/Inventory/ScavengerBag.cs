using Aequus.Common.Items;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory;

[WorkInProgress]
public class ScavengerBag : ModItem {
    public override string Texture => AequusTextures.Item(ItemID.HerbBag);

    public static int SlotAmount = 10;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SlotAmount);

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.color = Color.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().extraInventorySlots += SlotAmount;
    }
}