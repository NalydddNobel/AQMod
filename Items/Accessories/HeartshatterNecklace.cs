using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class HeartshatterNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.instaShieldTimeMax += 10;
            if (aequus.instaShieldCooldown > 0)
            {
                aequus.instaShieldCooldown = Math.Max(aequus.instaShieldCooldown / 2, 1);
            }
            else
            {
                aequus.instaShieldCooldown = 180;
            }
        }
    }
}