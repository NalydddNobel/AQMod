using Aequus.Common.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory;

[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : ModItem {
    public static int SlotAmount = 10;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SlotAmount);

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().extraInventorySlots += SlotAmount;
    }
}