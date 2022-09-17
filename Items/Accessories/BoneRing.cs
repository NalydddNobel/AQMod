using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BoneRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            if (aequus.accBoneRing > 0)
            {
                aequus.accBoneRing = Math.Max(aequus.instaShieldCooldown - 1, 1);
            }
            else
            {
                aequus.accBoneRing = 4;
            }
        }
    }
}