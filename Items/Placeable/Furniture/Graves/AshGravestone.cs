using Aequus.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Graves
{
    public class AshGravestone : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AshTombstones>(), AshTombstones.Style_AshGravestone);
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gravestone)
                .AddIngredient(ItemID.HellstoneBrick, 5)
                .AddIngredient(ItemID.AshBlock, 10)
                .AddTile(TileID.Hellforge)
                .Register();
        }
    }
}