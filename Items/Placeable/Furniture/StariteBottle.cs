using Aequus.Items.Consumables.Critters;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture
{
    public class StariteBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<StariteBottleTile>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient<DwarfStarite>()
                .TryRegisterAfter(ItemID.GoldButterflyCage);
        }
    }
}