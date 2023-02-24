using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Materials.Energies
{
    public class DemonicEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(0.8f, 0.2f, 0.2f);
        public override int Rarity => ItemRarityID.Orange;

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.DemonScythe)
                .AddIngredient(ItemID.Book)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Bookcases)
                .Register();
            Recipe.Create(ItemID.MagmaStone)
                .AddIngredient(ItemID.Obsidian, 20)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.ObsidianRose)
                .AddIngredient(ItemID.Obsidian, 20)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.LavaCharm)
                .AddIngredient(ItemID.Obsidian, 20)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.Sunfury)
                .AddIngredient(ItemID.BlueMoon)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.DarkLance)
                .AddIngredient(ItemID.Bone, 30)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.HellwingBow)
                .AddIngredient(ItemID.Bone, 30)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.Flamelash)
                .AddIngredient(ItemID.MagicMissile)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.FlowerofFire)
                .AddIngredient(ItemID.Bone, 30)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}