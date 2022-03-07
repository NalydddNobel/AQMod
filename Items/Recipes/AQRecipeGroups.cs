using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
using AQMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Recipes
{
    public static class AQRecipeGroups
    {
        public const string AnyNobleMushroom = "AQMod:AnyNobleMushroom";
        public const string AnyEnergy = "AQMod:AnyEnergy";
        public const string CopperOrTin = "AQMod:CopperOrTin";
        public const string DemoniteBarOrCrimtaneBar = "AQMod:DemoniteBarOrCrimtaneBar";
        public const string ShadowScaleOrTissueSample = "AQMod:ShadowScaleOrTissueSample";
        public const string CascadeOrHelfire = "AQMod:CascadeOrHelfire";
        public const string AnyEel = "AQMod:AnyEel";
        public const string EvilAccessories = "AQMod:EvilAccessories";

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
            RecipeGroup.RegisterGroup(EvilAccessories,
                new RecipeGroup(() => Language.GetTextValue("Mods.AQMod.RecipeGroup.EvilAccessories"),
                ItemID.BandofStarpower,
                ItemID.PanicNecklace));
        }
    }
}