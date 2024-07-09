using AequusRemake.Systems.Synergy;

namespace AequusRemake.Content.Items.Materials;

[Replacement]
public class BloodyTearstone : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 2);
    }

    public override void AddRecipes() {
        Recipe.Create(ItemID.BloodMoonStarter)
            .AddIngredient(Type, 15)
            .AddTile(TileID.Anvils)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.SuspiciousLookingEye)
            .DisableDecraft();
    }
}