using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class ForgedCard : ModItem, ItemHooks.IUpdateBank
    {
        public int Flat => Item.buyPrice(gold: 2, silver: 50);

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateInventory(Player player)
        {
            var aequus = player.Aequus();
            aequus.flatScamDiscount = Math.Max(aequus.flatScamDiscount, Item.buyPrice(gold: 1, silver: 50)) + Item.buyPrice(gold: 1);
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.flatScamDiscount += Flat;
        }
    }
}