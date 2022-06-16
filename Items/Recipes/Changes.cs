using Aequus.Common;
using Aequus.Items.Misc.Energies;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Recipes
{
    public class Changes : GlobalRecipe, IPostAddRecipes
    {
        void IPostAddRecipes.PostAddRecipes(Aequus aequus)
        {
            var register = new List<Recipe>();
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                if (r.createItem.type == ItemID.PhoenixBlaster)
                {
                    r.AddIngredient(ModContent.ItemType<DemonicEnergy>());
                }
                else if (r.createItem.type == ItemID.MinecartMech)
                {
                    r.AddIngredient(ItemID.Minecart);
                }
                else if (r.createItem.type == ItemID.PumpkinMoonMedallion || r.createItem.type == ItemID.NaughtyPresent)
                {
                    r.ReplaceItemWith(ItemID.Ectoplasm,
                        (r, i) => r.AddRecipeGroup(AequusRecipes.AnyEctoplasm, i.stack));
                }
                else if (r.createItem.type == ItemID.VoidLens && GameplayConfig.Instance.VoidBagRecipe)
                {
                    for (int j = 0; j < r.requiredItem.Count; j++)
                    {
                        if (j == 0)
                        {
                            r.requiredItem[j].SetDefaults(ItemID.MoneyTrough);
                        }
                        if (j == 1)
                        {
                            r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                            r.requiredItem[j].stack = 1;
                        }
                    }
                }
                else if (r.createItem.type == ItemID.VoidVault && GameplayConfig.Instance.VoidBagRecipe)
                {
                    for (int j = 0; j < r.requiredItem.Count; j++)
                    {
                        if (j == 0)
                        {
                            r.requiredItem[j].SetDefaults(ItemID.Safe);
                        }
                        if (j == 1)
                        {
                            r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                            r.requiredItem[j].stack = 1;
                        }
                    }
                }
            }

            foreach (var r in register)
            {
                r.Register();
            }
        }
    }
}