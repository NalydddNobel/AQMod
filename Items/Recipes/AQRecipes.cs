using AQMod.Content.World.Events;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Recipes
{
    public static class AQRecipes
    {
        internal static void VanillaRecipeAddons(AQMod aQMod)
        {
            var r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.Cloud, 20);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.SandBlock, 40);
            r.SetResult(ItemID.SandstorminaBottle);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.WaterWalkingPotion);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddIngredient(ModContent.ItemType<CrabShell>(), 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.WaterWalkingBoots);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.GreenThread);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();

            if (ModContent.GetInstance<AQConfigServer>().demonSiegeDowngrades)
            {
                foreach (var u in DemonSiege.Upgrades)
                {
                    r = new ModRecipe(aQMod);
                    r.AddIngredient(u.rewardItem);
                    r.AddIngredient(ModContent.ItemType<DemonicEnergy>());
                    r.AddTile(ModContent.TileType<GlimmeringStatueTile>());
                    r.SetResult(u.baseItem);
                    r.AddRecipe();
                }
            }
        }
    }
}