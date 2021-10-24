using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Amulets
{
    public class WyvernAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<AQPlayer>().wyvernAmuletHeld = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.wyvernAmulet = true;
            aQPlayer.wyvernAmuletHeld = true;
        }
    }
}