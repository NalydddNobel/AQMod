using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing.Rods
{
    public class CrabRod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 18;
            item.shootSpeed = 10f;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.Bobbers.CrabRod>();
        }


        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<CrabShell>(), 8);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ModContent.ItemType<ExoticCoral>(), 40);
            r.AddTile(ModContent.TileType<Tiles.FishingCraftingStation>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}