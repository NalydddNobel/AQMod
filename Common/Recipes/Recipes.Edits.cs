using Aequus.Common.Preferences;
using Aequus.Items;
using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes {
    public partial class AequusRecipes : ModSystem {
        public override void PostAddRecipes() {
            var config = GameplayConfig.Instance;
            for (int i = 0; i < Main.recipe.Length; i++) {
                Recipe r = Main.recipe[i];
                if (r.createItem == null)
                    continue;

                switch (r.createItem.type) {
                    case ItemID.PumpkinMoonMedallion:
                    case ItemID.NaughtyPresent: {
                            r.ReplaceItemWith(ItemID.Ectoplasm,
                                (r, i) => r.AddRecipeGroup(AnyEctoplasm, i.stack));
                        }
                        break;

                    case ItemID.VoidLens:
                    case ItemID.VoidVault: {
                            if (!config.VoidBagRecipe)
                                continue;

                            for (int j = 0; j < r.requiredItem.Count; j++) {
                                if (r.requiredItem[j].type == ItemID.JungleSpores) {
                                    r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                                    r.requiredItem[j].stack = 1;
                                }
                            }
                        }
                        break;
                }

                if (r.createItem.type <= ItemID.Count) {
                    continue;
                }

                if (config.EarlyGravityGlobe && r.HasIngredient(ItemID.GravityGlobe) && !r.HasIngredient(ItemID.LunarBar)) {
                    r.AddIngredient(ItemID.LunarBar, 5);
                }
                else if (config.EarlyPortalGun && r.HasIngredient(ItemID.PortalGun) && !r.HasIngredient(ItemID.LunarBar)) {
                    r.AddIngredient(ItemID.LunarBar, 5);
                }
                else if (config.EarlyWiring && r.HasIngredient(ItemID.Wrench) && !r.HasIngredient(ItemID.Bone)) {
                    r.AddIngredient(ItemID.Bone, 30);
                }
            }
        }
    }
}