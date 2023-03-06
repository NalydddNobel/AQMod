using Aequus.Common.Recipes;
using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    public class HardMushroomChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardMushroomChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ItemID.MushroomChest);
        }
    }
}