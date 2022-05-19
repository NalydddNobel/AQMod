using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Aequus.Items.Accessories
{
    public class FoolsGoldRing : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CoinRing;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 20);
            Item.color = new Color(255, 200, 200, 255);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accFoolsGoldRing = true;
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