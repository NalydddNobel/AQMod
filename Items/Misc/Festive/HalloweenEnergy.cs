using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc.Festive
{
    public class HalloweenEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(0.8f, 0.4f, 0.2f);
        public override int Rarity => ItemRarityID.Yellow;

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.SpookyTwig)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BlackFairyDust)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SpookyHook)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.StakeLauncher)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.NecromanticScroll)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.CandyCornRifle)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.JackOLanternLauncher)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.TheHorsemansBlade)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BatScepter)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.RavenStaff)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ScytheWhip)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.CursedSapling)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SpiderEgg)
                .AddIngredient<HalloweenEnergy>(3)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            Recipe.Create(ItemID.BatHook)
                .AddIngredient<HalloweenEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.RottenEgg, 33)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.CatEars)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.CatMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.CatShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.CatPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.CreeperMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.CreeperShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.CreeperPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.GhostMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.GhostShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.LeprechaunHat)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.LeprechaunShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.LeprechaunPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.PixieShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.PixiePants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.PrincessHat)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.PrincessDress)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.PumpkinMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.PumpkinShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.PumpkinPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.RobotMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.RobotShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.RobotPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.UnicornMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.UnicornShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.UnicornPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.VampireMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.VampireShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.VampirePants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.WitchHat)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.WitchDress)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.WitchBoots)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.BrideofFrankensteinMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.BrideofFrankensteinDress)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.KarateTortoiseMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.KarateTortoiseShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.KarateTortoisePants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.ReaperHood)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.ReaperRobe)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.FoxMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.FoxShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.FoxPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.SpaceCreatureMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.SpaceCreatureShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.SpaceCreaturePants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.WolfMask)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.WolfShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.WolfPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.TreasureHunterShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.TreasureHunterPants)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<HalloweenEnergy>()
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.UnluckyYarn)
                .AddIngredient<HalloweenEnergy>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}