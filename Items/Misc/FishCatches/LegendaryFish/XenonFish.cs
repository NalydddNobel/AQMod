using Aequus.Common.DataSets;
using Aequus.Items.Equipment.Accessories.Misc.Fishing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.FishCatches.LegendaryFish {
    public class XenonFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
            ItemSets.LegendaryFish.Add(Type);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add<RegrowingBait>(chance: 1, stack: 1)
                .Add(ItemID.XenonMoss, chance: 1, stack: (10, 25));
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Batfish);
            Item.questItem = false;
        }
    }
}