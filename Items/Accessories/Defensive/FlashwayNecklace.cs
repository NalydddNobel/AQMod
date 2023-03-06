using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Defensive
{
    public class FlashwayNecklace : ModItem
    {
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
            aequus.instaShieldTimeMax += 5;
            if (aequus.instaShieldCooldown > 0)
            {
                aequus.instaShieldCooldown = Math.Max(aequus.instaShieldCooldown / 2, 1);
            }
            else
            {
                aequus.instaShieldCooldown = 300;
            }
        }
    }
}