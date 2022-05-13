using System.Collections.Generic;
using Terraria;
using Terraria.ID;

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
            }
        }
    }
}