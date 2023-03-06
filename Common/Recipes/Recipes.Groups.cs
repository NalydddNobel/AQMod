using Aequus.Content.Elites.Items;
using Aequus.Items;
using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes
{
    public partial class AequusRecipes : ModSystem
    {
        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ItemID.Ectoplasm"/> and <see cref="Hexoplasm"/>.
        /// </summary>
        public static RecipeGroup AnyEctoplasm { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ElitePlantArgon"/>, <see cref="ElitePlantKrypton"/>, and <see cref="ElitePlantXenon"/>.
        /// </summary>
        [Obsolete()]
        public static RecipeGroup AnyMosshrooms { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for all IDs in <see cref="Main.anglerQuestItemNetIDs"/>.
        /// </summary>
        public static RecipeGroup AnyQuestFish { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for all IDs in <see cref="AequusItem.FruitIDs"/>.
        /// </summary>
        public static RecipeGroup AnyFruit { get; private set; }

        public override void AddRecipeGroups()
        {
            AnyEctoplasm = NewGroup("AnyEctoplasm",
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
            AnyMosshrooms = NewGroup("AnyMosshroom",
                ModContent.ItemType<ElitePlantArgon>(), ModContent.ItemType<ElitePlantKrypton>(), ModContent.ItemType<ElitePlantXenon>());
            AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
            AnyFruit = NewGroup("AnyFruit", AequusItem.FruitIDs.ToArray());
        }

        private static RecipeGroup NewGroup(string name, params int[] items)
        {
            var group = new RecipeGroup(() => TextHelper.GetTextValue("RecipeGroup." + name), items);
            RecipeGroup.RegisterGroup("Aequus:" + name, group);
            return group;
        }
    }
}