using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Aequus.Items.Placeable.Nature.MossMushrooms;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusRecipes : ModSystem
    {
        public static RecipeGroup AnyEctoplasm { get; private set; }
        public static RecipeGroup AnyMosshrooms { get; private set; }
        public static RecipeGroup AnyQuestFish { get; private set; }

        public override void AddRecipeGroups()
        {
            AnyEctoplasm = NewGroup("AnyEctoplasm",
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
            AnyMosshrooms = NewGroup("AnyMosshroom",
                ModContent.ItemType<ArgonMushroom>(), ModContent.ItemType<KryptonMushroom>(), ModContent.ItemType<XenonMushroom>());
            AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
        }

        private static RecipeGroup NewGroup(string name, params int[] items)
        {
            var group = new RecipeGroup(() => AequusText.GetText("RecipeGroup." + name), items);
            RecipeGroup.RegisterGroup("Aequus:" + name, group);
            return group;
        }

        // Recipe Edits
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                {
                    if (r.HasIngredient(ItemID.GravityGlobe) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
                }
                if (GameplayConfig.Instance.EarlyPortalGun)
                {
                    if (r.HasIngredient(ItemID.PortalGun) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
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
        }

        public static Recipe CreateRecipe(Recipe parent)
        {
            var r = CreateRecipe(parent.createItem.type, parent.createItem.stack);

            for (int i = 0; i < parent.requiredItem.Count; i++)
            {
                r.requiredItem.Add(parent.requiredItem[i].Clone());
            }
            for (int i = 0; i < parent.requiredTile.Count; i++)
            {
                r.requiredTile.Add(parent.requiredTile[i]);
            }
            for (int i = 0; i < parent.acceptedGroups.Count; i++)
            {
                r.acceptedGroups.Add(parent.acceptedGroups[i]);
            }
            for (int i = 0; i < parent.Conditions.Count; i++)
            {
                r.Conditions.Add(parent.Conditions[i]); // Same object reference, but conditions shouldn't be carrying instanced data which gets changed so it shouldn't be a problem, I think
            }
            return r;
        }
        public static Recipe CreateRecipe(int result, int stack = 1)
        {
            return Recipe.Create(result, stack);
        }
    }
}