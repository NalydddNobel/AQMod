using Aequus.Common.Preferences;
using Aequus.Content.Elites.Items;
using Aequus.Items;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes
{
    public partial class AequusRecipes : ModSystem
    {
        // Recipe Edits
        public override void PostAddRecipes()
        {
            var config = GameplayConfig.Instance;
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                if (config.EarlyGravityGlobe)
                {
                    if (r.HasIngredient(ItemID.GravityGlobe) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
                }
                if (config.EarlyPortalGun)
                {
                    if (r.HasIngredient(ItemID.PortalGun) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
                }

                if (r.createItem == null) // Weird
                    continue;

                if (r.createItem.type >= ItemID.Count)
                {
                    // To prevent some items being craftable because of early wires.
                    if (config.EarlyWiring
                        && r.createItem != null
                        && !r.createItem.mech /* Ignore items which show wires, although this may still allow some funky conflicts to slip through. */
                        && r.HasIngredient(ItemID.Wire) && !r.HasIngredient(ItemID.Bone)) // Only edit recipes which contain Wire, but not Bones.
                    {
                        int index = r.requiredItem.FindIndex((i) => i != null && i.stack > 0 && i.type == ItemID.Wire);
                        if (index > -1)
                        {
                            // Adds bones to the recipes, with a maximum of 30 bones incase a recipe requires like 9999 wire.
                            r.requiredItem.Insert(index, new Item(ItemID.Bone, Math.Min(r.requiredItem[index].stack, 30)));
                        }
                    }
                    continue;
                }

                switch (r.createItem?.type)
                {
                    case ItemID.PumpkinMoonMedallion:
                    case ItemID.NaughtyPresent:
                        {
                            r.ReplaceItemWith(ItemID.Ectoplasm,
                                (r, i) => r.AddRecipeGroup(AnyEctoplasm, i.stack));
                        }
                        break;

                    case ItemID.VoidLens:
                    case ItemID.VoidVault:
                        {
                            if (!config.VoidBagRecipe)
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
        }
    }
}