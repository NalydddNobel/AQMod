using AequusRemake.Core.ContentGeneration;

namespace AequusRemake.Content.Items.Potions;

public class HoneyPotions : ModSystem {
    public ModItem Greater = new Potion("Greater", AequusTextures.GreaterHoneyPotion.FullPath, healLife: 120, buffTime: 1500, value: Item.sellPrice(silver: 20), rarity: ItemRarityID.Orange);
    public ModItem Super = new Potion("Super", AequusTextures.SuperHoneyPotion.FullPath, healLife: 160, buffTime: 1800, value: Item.sellPrice(silver: 60), rarity: ItemRarityID.Lime);

    public override void Load() {
        Mod.AddContent(Greater);
        Mod.AddContent(Super);
    }

    public override void AddRecipes() {
        GreaterRecipe();
        SuperRecipe();

        void GreaterRecipe() {
            Greater.CreateRecipe(3)
                .AddIngredient(ItemID.BottledHoney, 3)
                .AddIngredient(ItemID.PixieDust, 3)
                .AddIngredient(ItemID.CrystalShard)
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

    internal class Potion(string name, string texture, int healLife, int buffTime, int value, int rarity) : InstancedModItem($"{name}HoneyPotion", texture) {
        public override void Load() {
            ModTypeLookup<ModItem>.RegisterLegacyNames(this, $"{name}Honey");
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
            Item.buffType = BuffID.Honey;
            Item.buffTime = buffTime;
            Item.healLife = healLife;
            Item.value = value;
            Item.rare = rarity;
        }
    }
}
