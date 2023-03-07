using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Fishing.Bait;
using Aequus.Tiles.Furniture;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Misc
{
    public class CrabCreviceCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 10;
            CrateBait.BiomeCrates.Add(new CrateBait.BiomeCrateFishingInfo((f, p) => p.Aequus().ZoneCrabCrevice, Type, ModContent.ItemType<CrabCreviceCrateHard>()));
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var l = new ItemLoot(ItemID.OceanCrateHard, Main.ItemDropsDB).Get(includeGlobalDrops: false);
            foreach (var loot in l)
            {
                if (loot is AlwaysAtleastOneSuccessDropRule oneFromOptions)
                {
                    int[] options = Array.ConvertAll(CrabCreviceBiome.ChestPrimaryLoot, (l) => l.item);
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, options));
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