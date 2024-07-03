using Aequu2.Content.Fishing;
using Terraria.Localization;

namespace Aequu2.Content.Items.Potions.Buffs.Sentry;

public class SentryPotion : ModItem {
    public static int IncreaseSentryAmount { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IncreaseSentryAmount);

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 20;
        ItemSets.DrinkParticleColors[Type] = new Color[] { new Color(208, 101, 32, 0), new Color(241, 216, 109, 0), new Color(138, 76, 31, 0), };
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.SummoningPotion);
        Item.buffType = ModContent.BuffType<SentryBuff>();
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(FishInstantiator.Killifish)
            .AddIngredient(ItemID.Shiverthorn)
            .AddTile(TileID.Bottles)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.SummoningPotion);
    }
}
