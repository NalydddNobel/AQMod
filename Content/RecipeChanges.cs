using Aequus.Common;
using Aequus.Items;
using Aequus.Items.Misc.Energies;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public class RecipeChanges : IPostAddRecipes
    {
        void ILoadable.Load(Mod mod)
        {
        }

        void IPostAddRecipes.PostAddRecipes(Aequus aequus)
        {
            var register = new List<Recipe>();
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                switch (r.createItem?.type)
                {
                    case ItemID.PumpkinMoonMedallion:
                    case ItemID.NaughtyPresent:
                        {
                            r.ReplaceItemWith(ItemID.Ectoplasm,
                                (r, i) => r.AddRecipeGroup(AequusRecipes.AnyEctoplasm, i.stack));
                        }
                        break;

                    case ItemID.VoidLens:
                    case ItemID.VoidVault:
                        {
                            if (!GameplayConfig.Instance.VoidBagRecipe)
                                continue;

                            for (int j = 0; j < r.requiredItem.Count; j++)
                            {
                                if (r.requiredItem[j].type == ItemID.JungleSpores)
                                {
                                    r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                                    r.requiredItem[j].stack = 1;
                                }
                            }
                        }
                        break;
                }
            }

            foreach (var r in register)
            {
                r.Register();
            }
        }

        void ILoadable.Unload()
        {
        }
    }
}