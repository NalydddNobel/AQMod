using Aequu2.Content.Configuration;
using Aequu2.Core.Entities.Items.Components;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequu2.Content.Items.Potions.Healing.Restoration;

public class LesserRestorationPotion : ModItem, IApplyPotionDelay, IPostAddRecipes {
    public override string Texture => AequusTextures.Item(ItemID.LesserRestorationPotion);

    public bool ApplyPotionDelay(Player player) {
        player.potionDelay = player.restorationDelayTime;
        player.AddBuff(BuffID.PotionSickness, player.potionDelay);
        return true;
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 30;
    }

    public override void SetDefaults() {
        Item.UseSound = SoundID.Item3;
        Item.healLife = 45;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useTurn = true;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 14;
        Item.height = 24;
        Item.potion = true;
        Item.value = Item.sellPrice(silver: 1, copper: 50);
        Item.rare = ItemRarityID.White;
    }

    public override void AddRecipes() {
        CreateRecipe(3)
            .AddIngredient(ItemID.Mushroom)
            .AddIngredient(ItemID.PinkGel)
            .AddIngredient(ItemID.Bottle, 3)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.LesserHealingPotion)
            .DisableDecraft();

        if (VanillaChangesConfig.Instance.RestorationPotionRecipe) {
            Recipe.Create(ItemID.RestorationPotion, 1)
                .AddIngredient(Type, 2)
                .AddIngredient(ItemID.GlowingMushroom)
                .AddTile(TileID.Bottles)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.HealingPotion)
                .DisableDecraft();
        }
    }

    public void PostAddRecipes(Mod mod) {
        if (!VanillaChangesConfig.Instance.RestorationPotionRecipe) {
            return;
        }

        for (int i = 0; i < Recipe.numRecipes; i++) {
            var recipe = Main.recipe[i];
            if (recipe == null || recipe.createItem.type != ItemID.RestorationPotion) {
                continue;
            }

            // shrug
            if (recipe.requiredItem.Find((i) => i.type == ItemID.Bottle) != null) {
                recipe.DisableRecipe();
            }
        }
    }
}