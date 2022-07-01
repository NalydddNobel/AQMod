using Aequus.Items.Misc;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Recipes
{
    public static class AequusRecipes
    {
        private static RecipeGroup anyEctoplasm;
        public static RecipeGroup AnyEctoplasm { get => anyEctoplasm; }

        internal static void AddRecipeGroups()
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

        public static Recipe ReplaceItem(this Recipe r, int item, int newItem, int newItemStack = -1)
        {
            for (int i = 0; i < r.requiredItem.Count; i++)
            {
                if (r.requiredItem[i].type == item)
                {
                    int stack = newItemStack <= 0 ? r.requiredItem[i].stack : newItemStack;
                    r.requiredItem[i].SetDefaults(newItem);
                    r.requiredItem[i].stack = stack;
                    break;
                }
            }
            return r;
        }

        public static void ReplaceItemWith(this Recipe r, int item, Action<Recipe, Item> replacementMethod)
        {
            var itemList = new List<Item>(r.requiredItem);
            r.requiredItem.Clear();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].type == item)
                {
                    replacementMethod(r, itemList[i]);
                }
                else
                {
                    r.AddIngredient(itemList[i].type, itemList[i].stack);
                }
            }
        }

        public static Recipe SpaceSquidRecipe(ModItem modItem, int original, bool sort = true)
        {
            return modItem.CreateRecipe()
                .AddIngredient(original)
                .AddIngredient(ModContent.ItemType<FrozenTear>(), 12)
                .AddTile(TileID.Anvils)
                .Register((r) =>
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
                .Register((r) =>
                    {
                        if (sort)
                        {
                            r.SortAfterFirstRecipesOf(ItemID.RainbowRod);
                        }
                    });
        }
    }
}