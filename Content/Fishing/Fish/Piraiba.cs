#if POLLUTED_OCEAN
namespace Aequus.Content.Fishing.Fish;

public class Piraiba : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Bass);
    }

    public override void AddRecipes() {
        Recipe.Create(ModContent.ItemType<Items.Consumable.Food.TaintedSeafood.TaintedSeafood>())
            .AddIngredient(Type)
            .AddTile(TileID.CookingPots)
            .Register()
            .DisableDecraft();
    }
}
#endif
