using Aequus.Common.Preferences;
using Aequus.Content.ItemPrefixes.Armor;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Placeable.Nature.Moss;
using Aequus.Tiles.CraftingStation;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusRecipes : ModSystem
    {
        /// <summary>
        /// A condition which locks a recipe behind the <see cref="AequusWorld.downedOmegaStarite"/> flag.
        /// </summary>
        public static Recipe.Condition ConditionOmegaStarite { get; private set; }

        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ItemID.Ectoplasm"/> and <see cref="Hexoplasm"/>.
        /// </summary>
        public static RecipeGroup AnyEctoplasm { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="EvilPlantArgon"/>, <see cref="EvilPlantKrypton"/>, and <see cref="EvilPlantXenon"/>.
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
            ConditionOmegaStarite = new Recipe.Condition(NetworkText.FromKey("Mods.Aequus.RecipeCondition.OmegaStarite"), (r) => AequusWorld.downedOmegaStarite);

            AnyEctoplasm = NewGroup("AnyEctoplasm",
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
            AnyMosshrooms = NewGroup("AnyMosshroom",
                ModContent.ItemType<EvilPlantArgon>(), ModContent.ItemType<EvilPlantKrypton>(), ModContent.ItemType<EvilPlantXenon>());
            AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
            AnyFruit = NewGroup("AnyFruit", AequusItem.FruitIDs.ToArray());
        }

        private static RecipeGroup NewGroup(string name, params int[] items)
        {
            var group = new RecipeGroup(() => TextHelper.GetTextValue("RecipeGroup." + name), items);
            RecipeGroup.RegisterGroup("Aequus:" + name, group);
            return group;
        }

        public override void AddRecipes()
        {
            CreatePrefixRecipes<ArgonPrefix>((r) => r.AddIngredient(ItemID.ArgonMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<KryptonPrefix>((r) => r.AddIngredient(ItemID.KryptonMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<NeonPrefix>((r) => r.AddIngredient(ItemID.PurpleMoss, 25).AddTile<ArmorSynthesizerTile>());
            CreatePrefixRecipes<XenonPrefix>((r) => r.AddIngredient(ItemID.XenonMoss, 25).AddTile<ArmorSynthesizerTile>());
        }

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

        public static void CreatePrefixRecipes<T>(Action<Recipe> createRecipe) where T : ModPrefix
        {
            var prefix = ModContent.GetInstance<T>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = AequusItem.SetDefaults(i);
                if (prefix.CanRoll(item))
                {
                    var r = Recipe.Create(i);
                    r.createItem.Prefix(prefix.Type);
                    r.AddIngredient(i);
                    createRecipe(r);
                    r.TryRegisterAfter(i);
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

        public static void CreateShimmerTransmutation(RecipeGroup ingredient, int result, Recipe.Condition condition = null)
        {
            Recipe.Create(result)
                .AddRecipeGroup(ingredient)
                .AddIngredient(condition != null ? ModContent.ItemType<CosmicEnergy>() : ItemID.FallenStar, condition != null ? 1 : 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
        public static void CreateShimmerTransmutation(int ingredient, int result, Recipe.Condition condition = null)
        {
            Recipe.Create(result)
                .AddIngredient(ingredient)
                .AddIngredient(condition != null ? ModContent.ItemType<CosmicEnergy>() : ItemID.FallenStar, condition != null ? 1 : 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}