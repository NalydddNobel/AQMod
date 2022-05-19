using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Aequus.Items.Misc.Money
{
    public class ForgedCard : ModItem, IUpdateBank
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.DiscountCard;

        public int Flat => Item.buyPrice(gold: 2, silver: 50);

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 20);
            Item.color = new Color(255, 200, 200, 255);
        }

        public override void UpdateInventory(Player player)
        {
            player.Aequus().flatScamDiscount += Flat;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.flatScamDiscount += Flat;
        }
    }
}