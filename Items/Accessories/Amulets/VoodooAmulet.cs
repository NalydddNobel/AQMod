using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Amulets
{
    public class VoodooAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<AQPlayer>().voodooAmuletHeld = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.voodooAmulet = true;
            aQPlayer.voodooAmuletHeld = true;
        }
    }
}