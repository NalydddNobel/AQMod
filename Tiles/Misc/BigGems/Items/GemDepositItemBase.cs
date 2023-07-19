using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Misc.BigGems.Items;

public abstract class GemDepositItemBase : ModItem {
    public const int AmtForRecipe = 10;
    public abstract int Style { get; }
    public abstract int BaseGem { get; }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(BaseGem);
        Item.createTile = ModContent.TileType<BigGemsTile>();
        Item.placeStyle = Style;
        Item.alpha = 0;
        Item.value *= 10;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(BaseGem, AmtForRecipe)
            .AddTile(TileID.Solidifier)
            .Register();
    }
}