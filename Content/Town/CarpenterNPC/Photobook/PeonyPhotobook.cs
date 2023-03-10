﻿using Aequus.Content.Town.CarpenterNPC.Photobook.UI;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Town.CarpenterNPC.Photobook
{
    public class PeonyPhotobook : PhotobookItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(32);
        }

        public override void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            base.UpdateBank(player, aequus, slot, bank);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(32);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PhotobookItem>()
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}