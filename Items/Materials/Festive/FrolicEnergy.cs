using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Festive
{
    [LegacyName("XmasEnergy")]
    public class FrolicEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0f, 0.7f), Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f + MathHelper.Pi, 0f, 0.5f), 0.1f);
        public override int Rarity => ItemRarityID.Yellow;

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.FestiveWings)
                .AddIngredient<FrolicEnergy>(3)
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChristmasHook)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChristmasTreeSword)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.Razorpine)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.EldMelter)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChainGun)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.NorthPole)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SnowmanCannon)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BlizzardStaff)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BabyGrinchMischiefWhistle)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ReindeerBells)
                .AddIngredient<FrolicEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            Recipe.Create(ItemID.CandyCaneHook)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.StarAnise, 33)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.CandyCaneSword)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.CnadyCanePickaxe)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.FruitcakeChakram)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.RedRyder)
                .AddIngredient(ItemID.FlintlockPistol)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.HandWarmer)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.Toolbox)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ReindeerAntlers)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.MrsClauseHat)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.MrsClauseShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.MrsClauseHeels)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.ParkaHood)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.ParkaCoat)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.ParkaPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.TreeMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.TreeShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.TreeTrunks)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.SnowHat)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.UglySweater)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.Holly)
                .AddIngredient<FrolicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.DogWhistle)
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.SnowGlobe)
                .AddIngredient<FrolicEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}