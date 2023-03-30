using Aequus.Content.ItemPrefixes.Armor;
using Aequus.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Gems;
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
        /// <summary>
        /// A condition which locks a recipe behind the <see cref="AequusWorld.downedOmegaStarite"/> flag.
        /// </summary>
        public static Recipe.Condition ShimmerConditionHackOmegaStarite { get; private set; }
        public static Recipe.Condition ShimmerConditionHackHardmodeTier { get; private set; }

        public static HashSet<int> PrefixedRecipeResultOverride { get; private set; }
        public static Dictionary<int, Func<bool>> ShimmerConditionOverride { get; private set; }

        public override void Load()
        {
            ShimmerConditionHackOmegaStarite = new Recipe.Condition(TextHelper.GetText("RecipeCondition.OmegaStarite").ToNetworkText(), (r) => AequusWorld.downedOmegaStarite);
            ShimmerConditionHackHardmodeTier = new Recipe.Condition(TextHelper.GetText("RecipeCondition.Hardmode").ToNetworkText(), (r) => Aequus.HardmodeTier);

            PrefixedRecipeResultOverride = new HashSet<int>();
            ShimmerConditionOverride = new Dictionary<int, Func<bool>>();
            //On.Terraria.Item.CanHavePrefixes += Item_CanHavePrefixes;
        }

        public override void Unload()
        {
            ShimmerConditionOverride?.Clear();
            ShimmerConditionOverride = null;
            PrefixedRecipeResultOverride?.Clear();
            PrefixedRecipeResultOverride = null;
        }

        public static void CreateShimmerTransmutation(int ingredient, int result, Recipe.Condition condition = null)
        {
            return;

            int ingredient2 = ModContent.ItemType<OmniGem>();
            int ingredient2Stack = 1;

            if (condition == ShimmerConditionHackOmegaStarite)
            {
                ingredient2 = ModContent.ItemType<CosmicEnergy>();
                ingredient2Stack = 1;
            }
            else if (condition == ShimmerConditionHackHardmodeTier)
            {
                ingredient2 = ItemID.SoulofLight;
                ingredient2Stack = 3;
            }

            Recipe.Create(result)
                .AddIngredient(ingredient)
                .AddIngredient(ingredient2, ingredient2Stack)
                .AddCondition()
                .AddTile(TileID.DemonAltar)
                .Register();

            //if (condition != null)
            //{
            //    ShimmerConditionOverride[ingredient] = condition;
            //}
            //ItemID.Sets.ShimmerTransformToItem[ingredient] = result;
        }
        public static void CreateShimmerTransmutation(RecipeGroup ingredient, int result, Recipe.Condition condition = null)
        {
            foreach (var i in ingredient.ValidItems)
            {
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