﻿using Aequus.Items.GlobalItems;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class IronLotus : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.Spears[Type] = true;
            TooltipsGlobal.Dedicated[Type] = new TooltipsGlobal.ItemDedication(new Color(110, 60, 30, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<IronLotusProj>(24);
            Item.SetWeaponValues(210, 2f, 0);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 10);
            Item.autoReuse = true;
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}