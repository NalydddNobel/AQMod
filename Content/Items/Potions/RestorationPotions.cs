using AequusRemake.Core.ContentGeneration;
using AequusRemake.Systems.Configuration;
using AequusRemake.Systems.Items;

namespace AequusRemake.Content.Items.Potions;

public class RestorationPotions : ModSystem {
    public ModItem Lesser = new Potion("Lesser", AequusTextures.Item(ItemID.LesserRestorationPotion), healLife: 45, value: Item.sellPrice(silver: 1, copper: 50), rarity: ItemRarityID.White);
    public ModItem Greater = new Potion("Greater", AequusTextures.GreaterRestorationPotion.FullPath, healLife: 135, value: Item.sellPrice(silver: 20), rarity: ItemRarityID.Orange);
    public ModItem Super = new Potion("Super", AequusTextures.SuperRestorationPotion.FullPath, healLife: 180, value: Item.sellPrice(silver: 60), rarity: ItemRarityID.Lime);

    public override void Load() {
        Mod.AddContent(Lesser);
        Mod.AddContent(Greater);
        Mod.AddContent(Super);
    }

    public override void AddRecipes() {
        LesserRecipe();
        GreaterRecipe();
        SuperRecipe();

        void LesserRecipe() {
            Lesser.CreateRecipe(3)
                .AddIngredient(ItemID.Mushroom)
                .AddIngredient(ItemID.PinkGel)
                .AddIngredient(ItemID.Bottle, 3)
                .AddTile(TileID.Bottles)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.LesserHealingPotion)
                .DisableDecraft();

            if (VanillaChangesConfig.Instance.RestorationPotionRecipe) {
                Recipe.Create(ItemID.RestorationPotion, 1)
                    .AddIngredient(Lesser.Type, 2)
                    .AddIngredient(ItemID.GlowingMushroom)
                    .AddTile(TileID.Bottles)
                    .Register()
                    .SortBeforeFirstRecipesOf(ItemID.HealingPotion)
                    .DisableDecraft();
            }
        }
        void GreaterRecipe() {
            Greater.CreateRecipe(3)
                .AddIngredient(ItemID.RestorationPotion, 3)
                .AddIngredient(ItemID.PixieDust, 3)
                .AddIngredient(ItemID.CrystalShard)
                .AddTile(TileID.Bottles)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GreaterHealingPotion)
                .DisableDecraft();
            Greater.CreateRecipe(3)
                .AddIngredient(ItemID.GreaterHealingPotion, 3)
                .AddIngredient(ItemID.PinkGel, 3)
                .AddTile(TileID.Bottles)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.GreaterHealingPotion)
                .DisableDecraft();
        }
        void SuperRecipe() {
            Super.CreateRecipe(4)
                .AddIngredient(Greater.Type, 4)
                .AddIngredient(ItemID.FragmentNebula)
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentStardust)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.Bottles)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.SuperHealingPotion)
                .DisableDecraft();
        }
    }

    public override void PostAddRecipes() {
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

    internal class Potion(string name, string texture, int healLife, int value, int rarity) : InstancedModItem($"{name}RestorationPotion", texture), IModifyPotionDelay {
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
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 14;
            Item.height = 24;
            Item.potion = true;
            Item.healLife = healLife;
            Item.value = value;
            Item.rare = rarity;
        }
    }
}
