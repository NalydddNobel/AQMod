using Aequus.Common;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Recipes
{
    public class AequusRecipes : ModSystem
    {
        private static RecipeGroup anyEctoplasm;
        public static RecipeGroup AnyEctoplasm { get => anyEctoplasm; }

        public override void AddRecipeGroups()
        {
            NewGroup("AnyEctoplasm", ref anyEctoplasm,
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
        }

        private static RecipeGroup NewGroup(string name, ref RecipeGroup group, params int[] items)
        {
            group = new RecipeGroup(
                () => AequusText.GetText("RecipeGroup." + name),
                items);
            RecipeGroup.RegisterGroup("Aequus:" + name, group);
            return group;
        }

        public override void PostAddRecipes()
        {
            var register = new List<Recipe>();
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                switch (r.createItem?.type)
                {
                    case ItemID.PumpkinMoonMedallion:
                    case ItemID.NaughtyPresent:
                        {
                            r.ReplaceItemWith(ItemID.Ectoplasm,
                                (r, i) => r.AddRecipeGroup(AnyEctoplasm, i.stack));
                        }
                        break;

                    case ItemID.VoidLens:
                    case ItemID.VoidVault:
                        {
                            if (!GameplayConfig.Instance.VoidBagRecipe)
                                continue;

                            for (int j = 0; j < r.requiredItem.Count; j++)
                            {
                                if (r.requiredItem[j].type == ItemID.JungleSpores)
                                {
                                    r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                                    r.requiredItem[j].stack = 1;
                                }
                            }
                        }
                        break;
                }
            }

            foreach (var r in register)
            {
                r.Register();
            }
        }

        public static Recipe CreateRecipe(Recipe parent)
        {
            var r = CreateRecipe(parent.createItem.type, parent.createItem.stack);

            for (int i = 0; i < parent.requiredItem.Count; i++)
            {
                r.requiredItem.Add(parent.requiredItem[i].Clone());
            }
            for (int i = 0; i < parent.requiredTile.Count; i++)
            {
                r.requiredTile.Add(parent.requiredTile[i]);
            }
            for (int i = 0; i < parent.acceptedGroups.Count; i++)
            {
                r.acceptedGroups.Add(parent.acceptedGroups[i]);
            }
            for (int i = 0; i < parent.Conditions.Count; i++)
            {
                r.Conditions.Add(parent.Conditions[i]); // Same object reference, but conditions shouldn't be carrying instanced data which gets changed so it shouldn't be a problem, I think
            }
            return r;
        }
        public static Recipe CreateRecipe(int result, int stack = 1)
        {
            return Recipe.Create(result, stack);
        }

        public static Recipe SpaceSquidRecipe(ModItem modItem, int original, bool sort = true)
        {
            return modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient(ModContent.ItemType<FrozenTear>(), 12)
                .AddTile(TileID.Anvils)
                .UnsafeSortRegister((r) =>
                    {
                        if (sort)
                        {
                            r.SortAfterFirstRecipesOf(ItemID.RainbowRod);
                        }
                    });
        }
        public static void RedSpriteRecipe(ModItem modItem, int original, int amt = 1, bool sort = true)
        {
            modItem.CreateRecipe()
                .AddIngredient(original, amt)
                .AddIngredient(ModContent.ItemType<Fluorescence>(), 12)
                .AddTile(TileID.Anvils)
                .UnsafeSortRegister((r) =>
                    {
                        if (sort)
                        {
                            r.SortAfterFirstRecipesOf(ItemID.RainbowRod);
                        }
                    });
        }
    }
}