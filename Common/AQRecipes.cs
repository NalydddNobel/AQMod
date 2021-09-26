using AQMod.Items.Accessories.ShopCards;
using AQMod.Items.Misc;
using AQMod.Items.Misc.Energies;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public static class AQRecipes
    {
        public static void AddRecipes(AQMod aQMod)
        {
            var r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.WoodenSword);
            r.AddIngredient(ItemID.IceBlock, 20);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceBlade);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.WoodenBoomerang);
            r.AddIngredient(ItemID.IceBlock, 20);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceBoomerang);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.WoodenBoomerang);
            r.AddIngredient(ItemID.IceBlock, 20);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceBoomerang);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.IceBlade);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.EnchantedSword);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.MagicHat);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.WizardHat);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ItemID.Cloud, 20);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ItemID.RainCloud, 20);
            r.SetResult(ItemID.TsunamiInABottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
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
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Snowball, 100);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlurryBoots);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Goldfish, 5);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.SailfishBoots);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.StrangePlant1);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlowerBoots);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.StrangePlant2);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlowerBoots);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.StrangePlant3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlowerBoots);
            r.AddRecipe();
            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.StrangePlant4);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlowerBoots);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.FlipperPotion);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.Flipper);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Wood, 6);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.BreathingReed);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.Spear);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.Trident);
            r.AddRecipe();

            r = new ModRecipe(aQMod);
            r.AddIngredient(ItemID.SwiftnessPotion);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.Aglet);
            r.AddRecipe();

            ModRecipe recipe = new ModRecipe(aQMod);
            recipe.AddIngredient(ModContent.ItemType<BlurryDiscountCard>());
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.DiscountCard);
            recipe.AddRecipe();
        }
    }
}