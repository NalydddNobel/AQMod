using AQMod.Content.LegacyWorldEvents.DemonSiege;
using AQMod.Items.Accessories.FidgetSpinner;
using AQMod.Items.Fish;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Materials.NobleMushrooms;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod
{
    public static class AQRecipes
    {
        public static class RecipeGroups
        {
            /// <summary>
            /// The key for the Any Noble Mushrooms recipe group
            /// </summary>
            public const string AnyNobleMushroom = "AQMod:AnyNobleMushroom";
            /// <summary>
            /// The key for the Any Energy recipe group
            /// </summary>
            public const string AnyEnergy = "AQMod:AnyEnergy";

            internal static void Setup()
            {
                var r = new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.AnyNobleMushroom"),
                    ModContent.ItemType<ArgonMushroom>(),
                    ModContent.ItemType<KryptonMushroom>(),
                    ModContent.ItemType<XenonMushroom>());
                RecipeGroup.RegisterGroup(AnyNobleMushroom, r);
                r = new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.AnyEnergy"),
                    ModContent.ItemType<UltimateEnergy>(),
                    ModContent.ItemType<AquaticEnergy>(),
                    ModContent.ItemType<AtmosphericEnergy>(),
                    ModContent.ItemType<OrganicEnergy>(),
                    ModContent.ItemType<DemonicEnergy>(),
                    ModContent.ItemType<CosmicEnergy>());
                RecipeGroup.RegisterGroup(AnyEnergy, r);
            }
        }

        public class r_ContainerPotionRecipe : ModRecipe
        {
            private readonly PotionofContainersTag.ContainerTag _chestTagCache;

            public r_ContainerPotionRecipe(Mod mod, int chest) : base(mod)
            {
                _chestTagCache = new PotionofContainersTag.ContainerTag(chest);
            }

            public override void OnCraft(Item item)
            {
                ((PotionofContainersTag)item.modItem).chestTag = _chestTagCache;
            }

            internal static void ConstructRecipe(int chestItem, ModItem item)
            {
                var recipe = new r_ContainerPotionRecipe(item.mod, (ushort)chestItem);
                recipe.AddIngredient(ModContent.ItemType<PotionofContainers>());
                recipe.AddIngredient(chestItem);
                recipe.AddTile(TileID.DemonAltar);
                recipe.SetResult(item);
                ((PotionofContainersTag)recipe.createItem.modItem).chestTag = recipe._chestTagCache; // so that it shows in the crafting menu
                recipe.AddRecipe();
            }
        }

        public class r_MolitePotionRecipe : ModRecipe
        {
            private readonly MoliteTag.StarbytePotionTag _potion;

            public r_MolitePotionRecipe(Mod mod, ushort potionType) : base(mod)
            {
                _potion = new MoliteTag.StarbytePotionTag(potionType);
            }

            public override void OnCraft(Item item)
            {
                ((MoliteTag)item.modItem).potion = _potion;
            }

            internal static void ConstructRecipe(int potionItem, ModItem item)
            {
                var recipe = new r_MolitePotionRecipe(item.mod, (ushort)potionItem);
                recipe.AddIngredient(ModContent.ItemType<Molite>());
                recipe.AddIngredient(potionItem);
                recipe.AddTile(TileID.DemonAltar);
                recipe.SetResult(item);
                var molite = (MoliteTag)recipe.createItem.modItem;
                molite.potion = recipe._potion;
                molite.SetupPotionStats();
                recipe.AddRecipe();
            }
        }

        public class r_FidgetSpinnerRecipe : ModRecipe
        {
            private readonly byte _clr;

            public r_FidgetSpinnerRecipe(Mod mod, byte clr) : base(mod)
            {
                _clr = clr;
            }

            public override void OnCraft(Item item)
            {
                ((FidgetSpinner)item.modItem).clr = _clr;
            }

            internal static void ConstructRecipe(int dyeItem, byte clr, ModItem item)
            {
                var recipe = new r_FidgetSpinnerRecipe(item.mod, clr);
                recipe.AddIngredient(ModContent.ItemType<FidgetSpinner>());
                recipe.AddIngredient(dyeItem);
                recipe.AddTile(TileID.DyeVat);
                recipe.SetResult(item);
                var spinner = (FidgetSpinner)recipe.createItem.modItem;
                spinner.clr = recipe._clr;
                recipe.AddRecipe();
            }
        }

        internal static void AddRecipes(AQMod aQMod)
        {
            var r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.Cloud, 20);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddIngredient(ItemID.SandBlock, 40);
            r.SetResult(ItemID.SandstorminaBottle);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.WaterWalkingPotion);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddIngredient(ModContent.ItemType<Items.Materials.CrabShell>(), 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.WaterWalkingBoots);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.GreenThread);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();

            if (ModContent.GetInstance<AQConfigServer>().demonSiegeDowngrades)
            {
                foreach (var u in DemonSiege._upgrades)
                {
                    r = new ModRecipe(aQMod);
                    r.AddIngredient(u.rewardItem);
                    r.AddIngredient(ModContent.ItemType<DemonicEnergy>());
                    r.AddTile(TileID.DemonAltar);
                    r.SetResult(u.baseItem);
                    r.AddRecipe();
                }
            }
        }
    }
}