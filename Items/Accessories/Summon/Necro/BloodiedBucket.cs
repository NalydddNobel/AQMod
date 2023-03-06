using Aequus.Common.Recipes;
using Aequus.Items.Weapons.Ranged.Bow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public class BloodiedBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueBloodMoon * 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostLifespan += 3600;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<CrusadersCrossbow>());
        }
    }
}