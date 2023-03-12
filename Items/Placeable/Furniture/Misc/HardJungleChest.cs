using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    public class HardJungleChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardJungleChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.GraniteChest, 5)
                .AddIngredient(ItemID.TurtleShell)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}