using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class HighSteaks : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.BloodMimicItemValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.bloodDiceDamage = Math.Max(aequus.bloodDiceDamage, 0.25f) + 0.25f;
            if (aequus.bloodDiceMoney > 0)
            {
                aequus.bloodDiceMoney = Math.Max(aequus.bloodDiceMoney / 2, 1);
            }
            else
            {
                aequus.bloodDiceMoney = Item.buyPrice(silver: 25);
            }
        }
    }
}