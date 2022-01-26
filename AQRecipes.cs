using AQMod.Content.World.Events.DemonSiege;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.FidgetSpinner;
using AQMod.Items.Fish;
using AQMod.Items.Foods;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
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
            public const string AnyNobleMushroom = "AQMod:AnyNobleMushroom";
            public const string AnyEnergy = "AQMod:AnyEnergy";
            public const string CopperOrTin = "AQMod:CopperOrTin";
            public const string DemoniteBarOrCrimtaneBar = "AQMod:DemoniteBarOrCrimtaneBar";
            public const string ShadowScaleOrTissueSample = "AQMod:ShadowScaleOrTissueSample";
            public const string CascadeOrHelfire = "AQMod:CascadeOrHelfire";
            public const string AnyEel = "AQMod:AnyEel";
            public const string EvilBarb = "AQMod:EvilBarb";

            internal static void Setup()
            {
                RecipeGroup.RegisterGroup(AnyNobleMushroom,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.AnyNobleMushroom"),
                    ModContent.ItemType<ArgonMushroom>(),
                    ModContent.ItemType<KryptonMushroom>(),
                    ModContent.ItemType<XenonMushroom>()));
                RecipeGroup.RegisterGroup(AnyEnergy,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.AnyEnergy"),
                    ModContent.ItemType<UltimateEnergy>(),
                    ModContent.ItemType<AquaticEnergy>(),
                    ModContent.ItemType<AtmosphericEnergy>(),
                    ModContent.ItemType<OrganicEnergy>(),
                    ModContent.ItemType<DemonicEnergy>(),
                    ModContent.ItemType<CosmicEnergy>()));
                RecipeGroup.RegisterGroup(CopperOrTin,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.CopperOrTin"),
                    ItemID.CopperBar,
                    ItemID.TinBar));
                RecipeGroup.RegisterGroup(DemoniteBarOrCrimtaneBar,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.DemoniteBarOrCrimtaneBar"),
                    ItemID.DemoniteBar,
                    ItemID.CrimtaneBar));
                RecipeGroup.RegisterGroup(ShadowScaleOrTissueSample,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.ShadowScaleOrTissueSample"),
                    ItemID.ShadowScale,
                    ItemID.TissueSample));
                RecipeGroup.RegisterGroup(CascadeOrHelfire,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.CascadeOrHelfire"),
                    ItemID.Cascade,
                    ItemID.HelFire));
                RecipeGroup.RegisterGroup(AnyEel,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.AnyEel"),
                    ModContent.ItemType<UltraEel>(),
                    ModContent.ItemType<LarvaEel>()));
                RecipeGroup.RegisterGroup(EvilBarb,
                    new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.EvilBarb"),
                    ModContent.ItemType<DemoniteBarb>(),
                    ModContent.ItemType<CrimtaneBarb>()));
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

        public class r_GlowString : ModRecipe
        {
            private readonly byte _clr;

            public r_GlowString(Mod mod, byte clr) : base(mod)
            {
                _clr = clr;
            }

            public override void OnCraft(Item item)
            {
                ((GlowString)item.modItem).clr = _clr;
            }

            internal static void ConstructRecipe(int stringItem, int dyeItem, byte clr, ModItem item)
            {
                var r = new r_GlowString(item.mod, clr);
                r.AddIngredient(ModContent.ItemType<GlowString>());
                r.AddIngredient(dyeItem);
                r.AddTile(TileID.DyeVat);
                r.SetResult(item);
                var glowString = (GlowString)r.createItem.modItem;
                glowString.clr = r._clr;
                r.AddRecipe();
                r = new r_GlowString(item.mod, clr);
                r.AddIngredient(stringItem);
                r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
                r.AddTile(TileID.DyeVat);
                r.SetResult(item);
                glowString = (GlowString)r.createItem.modItem;
                glowString.clr = r._clr;
                r.AddRecipe();
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
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
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