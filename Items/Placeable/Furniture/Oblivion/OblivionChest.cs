using Aequus.Content.Biomes.GoreNest.Tiles;
using Aequus.Tiles.Furniture.Oblivion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Oblivion
{
    public class OblivionChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OblivionChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.AshWoodChest)
                .AddTile<GoreNestTile>()
                .Register();
        }
    }
}