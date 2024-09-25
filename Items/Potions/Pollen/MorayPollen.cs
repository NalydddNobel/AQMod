using Aequus.Content.ItemPrefixes.Potions;
using Aequus.CrossMod;

namespace Aequus.Items.Potions.Pollen;
public class MorayPollen : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PixieDust);
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes() {
        var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<SplashPrefix>());
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            if (prefix.CanRoll(ContentSamples.ItemsByType[i])) {
                var r = Recipe.Create(i, 1)
                    .ResultPrefix<SplashPrefix>()
                    .AddIngredient(i)
                    .AddIngredient(Type)
                    .DisableDecraft()
                    .TryRegisterAfter(i);
                MagicStorage.AddBlacklistedItemData(i, prefix.Type);
            }
        }
    }
}