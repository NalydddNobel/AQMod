using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusRecipes
    {
        public static class Groups
        {
            public const string AnyEctoplasm = "Aequus:AnyEctoplasm";

            internal static void AddRecipeGroups()
            {
                RecipeGroup.RegisterGroup(
                    "Aequus:AnyEctoplasm",
                    new RecipeGroup(
                        () => AequusText.GetText("RecipeGroup.AnyEctoplasm"),
                        ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>()
                    ));
            }
        }
    }
}