using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture;

public abstract class TrappedChestBaseItem<TTrappedChestTile, TChestTile, TItem> : ModItem where TTrappedChestTile : TrappedChestBaseTile<TChestTile, TItem> where TItem : ModItem where TChestTile : BaseChest<TItem> {
    public override string Texture => ModContent.GetInstance<TItem>().Texture;

    public override void SetStaticDefaults() {
        ItemID.Sets.TrapSigned[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ModContent.ItemType<TItem>());
        Item.createTile = ModContent.TileType<TTrappedChestTile>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<TItem>())
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
