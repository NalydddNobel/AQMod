using Aequus.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Movement.FlashwayShield;

public class FlashwayShield : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 10);
        Item.defense = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.dashType != 0 || player.dashDelay != 0) {
            return;
        }

        player.dashType = -1;
        player.GetModPlayer<AequusPlayer>().SetDashData<FlashwayShieldDashData>();
    }
}