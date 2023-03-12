using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    // TODO: Check if this can accidentally break Calamity Hardmode progression rework via shimmer
    public class AdamantiteChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AdamantiteChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.GoldChest, 5)
                .AddIngredient(ItemID.AdamantiteBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.GoldChest, 5)
                .AddIngredient(ItemID.TitaniumBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}