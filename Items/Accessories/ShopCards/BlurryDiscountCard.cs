using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.ShopCards
{
    public class BlurryDiscountCard : ModItem, IUpdatePiggybank
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 1);
        }

        private void Update(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (!player.discount)
                aQPlayer.discountPercentage = 0.9f;
            player.discount = true;
        }

        public override void UpdateInventory(Player player)
        {
            Update(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Update(player);
        }

        void IUpdatePiggybank.UpdatePiggyBank(Player player, int i)
        {
            Update(player);
        }
    }
}