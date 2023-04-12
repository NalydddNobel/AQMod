using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes {
    public partial class AequusRecipes : ModSystem {
        /// <summary>
        /// A condition which locks a recipe behind the <see cref="AequusWorld.downedOmegaStarite"/> flag.
        /// </summary>
        public static Condition ShimmerConditionHackOmegaStarite { get; private set; }
        public static Condition ShimmerConditionHackHardmodeTier { get; private set; }

        public partial class Overrides {
            public static readonly HashSet<int> PrefixedRecipeResult = new();
            public static readonly Dictionary<int, Condition> ShimmerCondition = new();
        }

        public override void Load() {
            ShimmerConditionHackOmegaStarite = new(TextHelper.GetText("RecipeCondition.OmegaStarite"), () => AequusWorld.downedOmegaStarite);
            ShimmerConditionHackHardmodeTier = new(TextHelper.GetText("RecipeCondition.Hardmode"), () => Aequus.HardmodeTier);

            //On.Terraria.Item.CanHavePrefixes += Item_CanHavePrefixes;
        }

        public override void Unload() {
            Overrides.PrefixedRecipeResult.Clear();
            Overrides.ShimmerCondition.Clear();
        }

        public static void CreateShimmerTransmutation(int ingredient, int result, Condition condition = null) {
            if (condition != null) {
                Overrides.ShimmerCondition[ingredient] = condition;
            }
            ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }
        public static void CreateShimmerTransmutation(RecipeGroup ingredient, int result, Condition condition = null) {
            foreach (var i in ingredient.ValidItems) {
                CreateShimmerTransmutation(i, result, condition);
            }
        }

        #region Detours
        //private bool Item_CanHavePrefixes(On.Terraria.Item.orig_CanHavePrefixes orig, Item self)
        //{
        //    return PrefixedRecipeResultOverride.Contains(self.type) || orig(self);
        //}
        #endregion
    }
}