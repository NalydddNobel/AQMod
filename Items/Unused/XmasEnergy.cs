using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Unused
{
    public class XmasEnergy : EnergyItemBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Item.ResearchUnlockCount = 0;
        }

        protected override Vector3 LightColor => new Vector3(Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0f, 0.7f), Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f + MathHelper.Pi, 0f, 0.5f), 0.1f);
        public override int Rarity => ItemRarityID.Gray;

        //public override void AddRecipes()
        //{
        //    Recipe.Create(ItemID.FestiveWings)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddIngredient(ItemID.SoulofFlight, 20)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.ChristmasHook)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.ChristmasTreeSword)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.Razorpine)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.EldMelter)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.ChainGun)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.NorthPole)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.SnowmanCannon)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.BlizzardStaff)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.BabyGrinchMischiefWhistle)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();
        //    Recipe.Create(ItemID.ReindeerBells)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
        //        .AddTile(TileID.MythrilAnvil)
        //        .Register();

        //    Recipe.Create(ItemID.CandyCaneHook)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.StarAnise, 33)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Anvils)
        //        .Register();

        //    Recipe.Create(ItemID.CandyCaneSword)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.CnadyCanePickaxe)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.FruitcakeChakram)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.RedRyder)
        //        .AddIngredient(ItemID.FlintlockPistol)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.HandWarmer)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //    Recipe.Create(ItemID.Toolbox)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();

        //    Recipe.Create(ItemID.ReindeerAntlers)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Anvils)
        //        .Register();

        //    Recipe.Create(ItemID.MrsClauseHat)
        //        .AddIngredient(ItemID.Silk, 4)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.MrsClauseShirt)
        //        .AddIngredient(ItemID.Silk, 10)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.MrsClauseHeels)
        //        .AddIngredient(ItemID.Silk, 6)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();

        //    Recipe.Create(ItemID.ParkaHood)
        //        .AddIngredient(ItemID.Silk, 4)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.ParkaCoat)
        //        .AddIngredient(ItemID.Silk, 10)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.ParkaPants)
        //        .AddIngredient(ItemID.Silk, 6)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();

        //    Recipe.Create(ItemID.TreeMask)
        //        .AddIngredient(ItemID.Silk, 4)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.TreeShirt)
        //        .AddIngredient(ItemID.Silk, 10)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();
        //    Recipe.Create(ItemID.TreeTrunks)
        //        .AddIngredient(ItemID.Silk, 6)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();

        //    Recipe.Create(ItemID.SnowHat)
        //        .AddIngredient(ItemID.Silk, 4)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();

        //    Recipe.Create(ItemID.UglySweater)
        //        .AddIngredient(ItemID.Silk, 10)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Loom)
        //        .Register();

        //    Recipe.Create(ItemID.Holly)
        //        .AddIngredient<XmasEnergy>()
        //        .AddTile(TileID.Anvils)
        //        .Register();

        //    Recipe.Create(ItemID.DogWhistle)
        //        .AddIngredient<XmasEnergy>(10)
        //        .AddTile(TileID.Anvils)
        //        .Register();

        //    Recipe.Create(ItemID.SnowGlobe)
        //        .AddIngredient<XmasEnergy>(3)
        //        .AddIngredient(ItemID.SoulofNight, 3)
        //        .AddTile(TileID.Anvils)
        //        .Register();
        //}
    }
}