using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life.Dodge {
    public class HeartshatterNecklace : ModItem {
        /// <summary>
        /// Default Value: 180 (3 seconds)
        /// </summary>
        public static int InstaShieldCooldown = 180;
        /// <summary>
        /// Default Value: 10
        /// </summary>
        public static int InstaShieldFrames = 10;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.instaShieldFrames += InstaShieldFrames;
            if (aequus.instaShieldCooldown > 0) {
                aequus.instaShieldCooldown = Math.Max(aequus.instaShieldCooldown / 2, 1);
            }
            else {
                aequus.instaShieldCooldown = InstaShieldCooldown;
            }
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient<FlashwayNecklace>()
                .AddIngredient<PossessedShard>(3)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterAfter(ItemID.SweetheartNecklace);
        }
    }
}