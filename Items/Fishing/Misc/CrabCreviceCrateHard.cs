﻿using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Ranged;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Fishing.Misc
{
    public class CrabCreviceCrateHard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 10;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var l = new ItemLoot(ItemID.OceanCrateHard, Main.ItemDropsDB).Get(includeGlobalDrops: false);
            foreach (var loot in l)
            {
                if (loot is AlwaysAtleastOneSuccessDropRule oneFromOptions)
                {
                    itemLoot.Add(ItemDropRule.OneFromOptions(1,
                        ModContent.ItemType<StarPhish>(), ModContent.ItemType<DavyJonesAnchor>(), ModContent.ItemType<ArmFloaties>(), ModContent.ItemType<LiquidGun>()));
                    continue;
                }
                itemLoot.Add(loot);
            }
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.OceanCrateHard);
            Item.createTile = ModContent.TileType<FishingCrates>();
            Item.placeStyle = FishingCrates.CrabCreviceCrateHard;
        }

        public override bool CanRightClick()
        {
            return true;
        }
    }
}