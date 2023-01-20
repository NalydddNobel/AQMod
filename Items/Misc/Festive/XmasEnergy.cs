using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc.Festive
{
    public class XmasEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0f, 0.5f), AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f + MathHelper.PiOver2, 0f, 0.5f), 0.1f);
        public override int Rarity => ItemRarityID.Yellow;

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.FestiveWings)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChristmasHook)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChristmasTreeSword)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.Razorpine)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.EldMelter)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ChainGun)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.NorthPole)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SnowmanCannon)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BlizzardStaff)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.BabyGrinchMischiefWhistle)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.ReindeerBells)
                .AddIngredient<XmasEnergy>(3)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}