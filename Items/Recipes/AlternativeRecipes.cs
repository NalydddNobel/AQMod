using Aequus.Items.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Recipes
{
    public static class AlternativeRecipes
    {
        public static void AddRecipes()
        {
            List<Recipe> register = new List<Recipe>();
            foreach (var r in Main.recipe)
            {
                CheckHexoplasmAlt(r, register);
            }

            foreach (var r in register)
            {
                r.Register();
            }
        }
        public static void CheckHexoplasmAlt(Recipe r, List<Recipe> register)
        {
            if (r.createItem.type == ItemID.PumpkinMoonMedallion || r.createItem.type == ItemID.NaughtyPresent)
            {
                r.ReplaceItemWithRecipeGroup(ItemID.Ectoplasm, AequusRecipes.Groups.AnyEctoplasm);
                //register.Add(
                //    CreateRecipe(r)
                //    .ReplaceItem(ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>()));
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
            return Aequus.Instance.CreateRecipe(result, stack);
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
        public static void ReplaceItemWithRecipeGroup(this Recipe r, int item, string recipeGroup)
        {
            var itemList = new List<Item>(r.requiredItem);
            r.requiredItem.Clear();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].type == item)
                {
                    r.AddRecipeGroup(recipeGroup, itemList[i].stack);
                }
                else
                {
                    r.AddIngredient(itemList[i].type, itemList[i].stack);
                }
            }
        }
    }
}