using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes {
    public partial class AequusRecipes : ModSystem {

        public partial class Overrides {
            public static readonly HashSet<int> PrefixedRecipeResult = new();
            public static readonly Dictionary<int, Condition> ShimmerCondition = new();
        }

        public override void Load() {
            On_Item.CanHavePrefixes += Item_CanHavePrefixes;
        }

        public override void Unload() {
            Overrides.PrefixedRecipeResult.Clear();
            Overrides.ShimmerCondition.Clear();
        }

        public static void AddShimmerCraft(int ingredient, int result, Condition condition = null) {
            if (condition != null) {
                Overrides.ShimmerCondition[ingredient] = condition;
            }
            ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }
        public static void AddShimmerCraft(RecipeGroup ingredient, int result, Condition condition = null) {
            foreach (var i in ingredient.ValidItems) {
                AddShimmerCraft(i, result, condition);
            }
        }

        #region Detours
        private bool Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
            return Overrides.PrefixedRecipeResult.Contains(self.type) || orig(self);
        }
        #endregion
    }
}