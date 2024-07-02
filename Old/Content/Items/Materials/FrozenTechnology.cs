using Aequus.Content.Systems;
using Aequus.DataSets;
using Aequus.DataSets.Structures.Enums;

namespace Aequus.Old.Content.Items.Materials;

public class FrozenTechnology : ModItem {
    public override void SetStaticDefaults() {
        //ItemSets.SortingPriorityMaterials[Type] = ???;
        Item.ResearchUnlockCount = 25;
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Frozen, Type, minStack: 1, maxStack: 2, chanceDemoninator: 3);
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 80);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override void AddRecipes() {
        RadarRecipe(); /* Radar */
        TravellingMerchantRecipes(); /* DPS Meter, Stopwatch, Lifeform Analyzer */
        CavernDropRecipes(); /* Depth Meter, Compass, Metal Detector */
        TallyCounterRecipe(); /* Tally Counter */
        /* AnglerRecipes(); Fisherman's Pocket Guide, Weather Radio, Sextant */
    }

    private void RadarRecipe() {
        Recipe.Create(ItemID.Radar)
            .AddRecipeGroup(RecipeSystem.AnyCopperBar, 15)
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);
    }

    private void CavernDropRecipes() {
        Recipe.Create(ItemID.DepthMeter)
            .AddRecipeGroup(RecipeSystem.AnyCopperBar, 10)
            .AddRecipeGroup(RecipeSystem.AnySilverBar, 8)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 6)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.Compass)
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddRecipeGroup(RecipeSystem.AnySilverBar, 6)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 4)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.MetalDetector)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 25)
            .AddIngredient(ItemID.FlinxFur, 3)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);
    }

    private void TravellingMerchantRecipes() {
        Recipe.Create(ItemID.DPSMeter)
            .AddRecipeGroup(RecipeSystem.AnyCopperBar, 10)
            .AddRecipeGroup(RecipeSystem.AnySilverBar, 8)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 6)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.Stopwatch)
            .AddRecipeGroup(RecipeSystem.AnySilverBar, 25)
            .AddRecipeGroup(RecipeGroupID.PressurePlate)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.LifeformAnalyzer)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 25)
            .AddIngredient(ItemID.AntlionMandible, 3)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);
    }

    private void TallyCounterRecipe() {
        // Tally counter is normally locked to post-Skeletron,
        // but this makes it obtainable earlier
        Recipe.Create(ItemID.TallyCounter)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 25)
            .AddIngredient<GlowLichen>(3)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);
    }

    private void AnglerRecipes() {
        Recipe.Create(ItemID.FishermansGuide)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.WeatherRadio)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);

        Recipe.Create(ItemID.Sextant)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 25)
            .AddIngredient(ItemID.Lens, 3)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient(Type)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PlatinumWatch);
    }
}