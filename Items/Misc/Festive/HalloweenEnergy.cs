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
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BlackFairyDust)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SpookyHook)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.StakeLauncher)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.NecromanticScroll)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.CandyCornRifle)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.JackOLanternLauncher)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.TheHorsemansBlade)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BatScepter)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.RavenStaff)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ScytheWhip)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.CursedSapling)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SpiderEgg)
                .AddIngredient<HalloweenEnergy>(3)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}