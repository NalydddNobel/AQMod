using Aequus.Items;
using Aequus.Items.Materials.Hexoplasm;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes {
    public partial class AequusRecipes : ModSystem {
        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ItemID.Ectoplasm"/> and <see cref="Hexoplasm"/>.
        /// </summary>
        public static RecipeGroup AnyEctoplasm { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for all IDs in <see cref="Main.anglerQuestItemNetIDs"/>.
        /// </summary>
        public static RecipeGroup AnyQuestFish { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for the Shellphone and its alternative modes.
        /// </summary>
        public static RecipeGroup Shellphone { get; private set; }

        public override void AddRecipeGroups() {
            AnyEctoplasm = NewGroup("AnyEctoplasm",
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
            AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
            Shellphone = NewGroup("Shellphone", ItemID.ShellphoneDummy, ItemID.Shellphone, ItemID.ShellphoneHell, ItemID.ShellphoneOcean, ItemID.ShellphoneSpawn);
        }

        private static RecipeGroup NewGroup(string name, params int[] items) {
            RecipeGroup group = new(() => Language.GetOrRegister("Mods.Aequus.RecipeGroup." + name).Value, items);
            RecipeGroup.RegisterGroup(name, group);
            return group;
        }
    }
}