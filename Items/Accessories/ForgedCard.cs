using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class ForgedCard : ModItem, IUpdateBank
    {
        public int Flat => Item.buyPrice(gold: 2, silver: 50);

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 20);
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