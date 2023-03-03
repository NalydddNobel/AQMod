using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Energies
{
    [LegacyName("LightMatter")]
    public class CosmicEnergy : EnergyItemBase
    {
        public override int Rarity => ItemRarityID.LightRed;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.AntiGravityHook)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.InfluxWaver)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.Xenopopper)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ElectrosphereLauncher)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChargedBlasterCannon)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.LaserMachinegun)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.XenoStaff)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.LaserDrill)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BrainScrambler)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.CosmicCarKey)
                .AddIngredient(ItemID.MartianConduitPlating, 50)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}