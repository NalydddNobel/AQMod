using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Materials.Energies {
    public class OrganicEnergy : EnergyItemBase {
        protected override Vector3 LightColor => new Vector3(0.2f, 0.5f, 0.3f);
        public override int Rarity => ItemRarityID.Lime;

        public override void AddRecipes() {
            Recipe.Create(ItemID.GrenadeLauncher)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddIngredient(ItemID.RocketI, 100)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.VenusMagnum)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.NettleBurst)
                .AddIngredient(ItemID.CrystalVileShard)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.LeafBlower)
                .AddIngredient(ItemID.JungleSpores, 10)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.WaspGun)
                .AddIngredient(ItemID.BeeGun)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.Seedler)
                .AddIngredient(ItemID.JungleSpores, 10)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.PygmyStaff)
                .AddIngredient(ItemID.JungleSpores, 10)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.ThornHook)
                .AddIngredient(ItemID.Hook)
                .AddIngredient(ItemID.ChlorophyteBar, 6)
                .AddIngredient(Type)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
        }
    }
}