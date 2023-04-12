using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Defense
{
    public class FlashwayNecklace : ModItem
    {
        /// <summary>
        /// Default Value: 300 (5 seconds)
        /// </summary>
        public static int InstaShieldCooldown = 300;
        /// <summary>
        /// Default Value: 5
        /// </summary>
        public static int InstaShieldFrames = 5;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.instaShieldFrames += InstaShieldFrames;
            if (aequus.instaShieldCooldown > 0)
            {
                aequus.instaShieldCooldown = Math.Max(aequus.instaShieldCooldown / 2, 1);
            }
            else
            {
                aequus.instaShieldCooldown = InstaShieldCooldown;
            }
        }
    }
}