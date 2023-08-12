using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items.Misc.FishingBait;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.GrabBags.Crates {
    public class CrabCreviceCrate : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 10;
            CrateBait.BiomeCrates.Add(new CrateBait.BiomeCrateFishingInfo((f, p) => p.InModBiome<CrabCreviceBiome>(), Type, ModContent.ItemType<CrabCreviceCrateHard>()));
        }

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            var l = new ItemLoot(ItemID.OceanCrateHard, Main.ItemDropsDB).Get(includeGlobalDrops: false);
            foreach (var loot in l) {
                if (loot is AlwaysAtleastOneSuccessDropRule oneFromOptions) {
                    int[] options = Array.ConvertAll(CrabCreviceBiome.ChestPrimaryLoot, (l) => l.item);
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, options));
                    continue;
                }
                itemLoot.Add(loot);
            }
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.OceanCrate);
            Item.createTile = ModContent.TileType<FishingCratesTile>();
            Item.placeStyle = FishingCratesTile.CrabCreviceCrate;
        }

        public override bool CanRightClick() {
            return true;
        }
    }
}