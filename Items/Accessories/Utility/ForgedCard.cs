using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class ForgedCard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().flatScamDiscount += Item.gold;
            if (Main.myPlayer == player.whoAmI)
            {
                Main.shopSellbackHelper.Clear();
            }
        }
    }
}