using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Ranged;
using Aequus.Tiles;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags
{
    public class CrabCreviceCrate : ModItem
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
            Item.CloneDefaults(ItemID.OceanCrate);
            Item.createTile = ModContent.TileType<FishingCrates>();
            Item.placeStyle = FishingCrates.CrabCreviceCrate;
        }

        public override bool CanRightClick()
        {
            return true;
        }
    }
}