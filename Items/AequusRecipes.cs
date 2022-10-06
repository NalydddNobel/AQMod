using Aequus.Common;
using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusRecipes : IAddRecipeGroups, IAddRecipes
    {
        private static RecipeGroup anyEctoplasm;
        public static RecipeGroup AnyEctoplasm { get => anyEctoplasm; }

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipeGroups.AddRecipeGroups(Aequus aequus)
        {
            NewGroup("AnyEctoplasm", ref anyEctoplasm,
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
        }

        void ILoadable.Unload()
        {
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