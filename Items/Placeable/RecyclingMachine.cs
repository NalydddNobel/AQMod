using Aequus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class RecyclingMachine : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RecyclingMachineTile>());
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar, 12)
                .AddRecipeGroup("IronBar", 2)
                .AddRecipeGroup("PresurePlate")
                .AddRecipeGroup("Sand", 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 12)
                .AddRecipeGroup("IronBar", 2)
                .AddRecipeGroup("PresurePlate")
                .AddRecipeGroup("Sand", 10)
                .AddTile(TileID.Anvils)
                .Register();

        }
    }
}