using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Content.Tiles.HangingPots;
using Aequus.CrossMod;

namespace Aequus.Items.Potions.Pollen;

public class ManaclePollen : ModItem {
    public override void Load() {
        Mod.AddContent(new InstancedHangingPot(this, "ManaclePot", AequusTextures.ManaclePot.FullPath));
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PixieDust);
        Item.rare = ItemRarityID.Green;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 200);
    }

    public override void AddRecipes() {
        var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<BoundedPrefix>());
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            if (prefix.CanRoll(ContentSamples.ItemsByType[i])) {
                var r = Recipe.Create(i, 1)
                    .ResultPrefix<BoundedPrefix>()
                    .AddIngredient(i)
                    .AddIngredient(Type)
                    .DisableDecraft()
                    .TryRegisterAfter(i);
                MagicStorage.AddBlacklistedItemData(i, prefix.Type);
            }
        }
    }
}