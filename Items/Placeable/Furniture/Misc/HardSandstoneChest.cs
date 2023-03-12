using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Misc
{
    public class HardSandstoneChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardSandstoneChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ItemID.DesertChest, 5)
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}