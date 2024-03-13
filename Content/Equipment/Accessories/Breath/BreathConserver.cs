using Aequus.Common.Items;

namespace Aequus.Content.Equipment.Accessories.Breath;

[AutoloadEquip(EquipType.Back)]
public class BreathConserver : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accRestoreBreathOnKill++;
    }
}