using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    [LegacyName("BloodyTearFragment")]
    public class BloodyTearstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.DemoniteOre;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.BloodMoonStarter)
                .AddIngredient(Type, 4)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.SuspiciousLookingEye)
                .DisableDecraft();
            Recipe.Create(ItemID.BloodMoonMonolith)
                .AddIngredient(Type, 8)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.VoidMonolith)
                .DisableDecraft();
            Recipe.Create(ItemID.BloodyMachete)
                .AddIngredient(Type, 8)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.BloodRainBow)
                .AddIngredient(Type, 8)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.VampireFrogStaff)
                .AddIngredient(Type, 8)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.BloodFishingRod)
                .AddIngredient(Type, 8)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.BloodHamaxe)
                .AddIngredient(Type, 12)
                .AddIngredient<AquaticEnergy>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.DripplerFlail)
                .AddIngredient(Type, 12)
                .AddIngredient<AquaticEnergy>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
            Recipe.Create(ItemID.SharpTears)
                .AddIngredient(Type, 12)
                .AddIngredient<AquaticEnergy>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register()
                .DisableDecraft();
        }
    }
}