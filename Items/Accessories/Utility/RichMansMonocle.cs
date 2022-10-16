﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class RichMansMonocle : ModItem, ItemHooks.IUpdateVoidBag
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
        }

        public override void UpdateInventory(Player player)
        {
            player.Aequus().accPriceMonocle = true;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.accPriceMonocle = true;
        }
    }
}