using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class FoolsGoldRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accFoolsGold = true;
            long coinAmt = 0L;
            for (int i = 0; i < Main.InventoryCoinSlotsCount; i++)
            {
                var item = player.inventory[Main.InventoryCoinSlotsStart + i];
                if (item.IsAir)
                {
                    continue;
                }
                coinAmt += item.value * (long)item.stack;
            }

            coinAmt = Math.Min(coinAmt, Item.platinum * 50L);

            player.GetDamage(DamageClass.Generic) += (int)(coinAmt / (double)Item.platinum) / 100f;
            player.statDefense += (int)(coinAmt / (double)Item.platinum);
        }
    }
}