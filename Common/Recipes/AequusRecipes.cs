using Aequus.Common.Preferences;
using Aequus.Content.ItemPrefixes.Armor;
using Aequus.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Tiles.CraftingStation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes
{
    public partial class AequusRecipes : ModSystem
    {
        public static HashSet<int> PrefixedRecipeResultOverride { get; private set; }
        public static Dictionary<int, Func<bool>> ShimmerConditionOverride { get; private set; }

        public override void Load()
        {
            PrefixedRecipeResultOverride = new HashSet<int>();
            ShimmerConditionOverride = new Dictionary<int, Func<bool>>();
            On_Item.CanHavePrefixes += Item_CanHavePrefixes;
        }

        public override void Unload()
        {
            ShimmerConditionOverride?.Clear();
            ShimmerConditionOverride = null;
            PrefixedRecipeResultOverride?.Clear();
            PrefixedRecipeResultOverride = null;
        }

        private static void CreatePrefixRecipes<T>(Action<Recipe> createRecipe) where T : ModPrefix
        {
            var prefix = ModContent.GetInstance<T>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = AequusItem.SetDefaults(i);
                if (prefix.CanRoll(item))
                {
                    var r = Recipe.Create(i)
                        .ResultPrefix<T>()
                        .AddIngredient(i);
                    createRecipe(r);
                    r.TryRegisterAfter(i);
                }
            }
        }

        public override void AddRecipes()
        {
            CreatePrefixRecipes<ArgonPrefix>((r) => r.AddIngredient(ItemID.ArgonMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<KryptonPrefix>((r) => r.AddIngredient(ItemID.KryptonMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<NeonPrefix>((r) => r.AddIngredient(ItemID.PurpleMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<XenonPrefix>((r) => r.AddIngredient(ItemID.XenonMoss, 25).AddTile<ArmorSynthesizerTile>());
        }

        public static void CreateShimmerTransmutation(int ingredient, int result, Func<bool> condition = null)
        {
            if (condition != null)
            {
                ShimmerConditionOverride[ingredient] = condition;
            }
            ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }
        public static void CreateShimmerTransmutation(RecipeGroup ingredient, int result, Func<bool> condition = null)
        {
            foreach (var i in ingredient.ValidItems)
            {
                CreateShimmerTransmutation(i, result, condition);
            }
        }

        #region Detours
        private bool Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self)
        {
            return PrefixedRecipeResultOverride.Contains(self.type) || orig(self);
        }
        #endregion
    }
}