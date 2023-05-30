using Aequus.Content.Town.CarpenterNPC.Photobook.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Content.Town.CarpenterNPC.Photobook {
    public class PeonyPhotobook : PhotobookItem {
        public new const int PhotoStorage = 32;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PhotoStorage);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(PhotoStorage);
        }

        public override void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            base.UpdateBank(player, aequus, slot, bank);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(PhotoStorage);
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