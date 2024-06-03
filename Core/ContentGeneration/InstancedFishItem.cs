using Aequus.Core.ContentGeneration;
using System;

namespace Aequus.Content.Fishing;

internal class InstancedFishItem : InstancedModItem {
    private readonly int _rarity;
    private readonly int _value;
    private readonly Action<ModItem> _addRecipes;

    public InstancedFishItem(string name, int rarity, int value, Action<ModItem> Recipe) : base(name, $"{typeof(InstancedFishItem).NamespaceFilePath()}/Fish/{name}") {
        _rarity = rarity;
        _value = value;
        _addRecipes = Recipe;
    }

    public override string LocalizationCategory => "Fishing.Catches";

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = _rarity;
        Item.value = _value;
    }

    public override void AddRecipes() {
        _addRecipes?.Invoke(this);
    }

    public static void SeafoodDinnerRecipe(ModItem modItem) {
        Recipe.Create(ItemID.SeafoodDinner)
            .AddIngredient(modItem.Type, 2)
            .AddTile(TileID.CookingPots)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.SeafoodDinner)
            .DisableDecraft();
    }

    public static void CookedFishRecipe(ModItem modItem) {
        Recipe.Create(ItemID.CookedFish)
            .AddIngredient(modItem.Type, 1)
            .AddTile(TileID.CookingPots)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.CookedFish)
            .DisableDecraft();
    }
}
