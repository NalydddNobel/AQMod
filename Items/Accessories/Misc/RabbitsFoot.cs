﻿using Aequus.Biomes.DemonSiege;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc
{
    public class RabbitsFoot : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.Bunny, Type, UpgradeProgressionType.PreHardmode) { Hide = true, });
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().luckRerolls += 1f;
        }
    }
}