using Aequus.Content.Biomes.Meadows.Tiles;

namespace Aequus.Content.Items.Materials.Paper;

public class PaperMaterial : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults() {
        Item.width = 14;
        Item.height = 14;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 2);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(Instance<MeadowWood>().Item.Type, 7)
            .AddTile(TileID.Sawmill)
            .Register();
    }
}
