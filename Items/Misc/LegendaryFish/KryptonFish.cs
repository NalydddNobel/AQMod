using Aequus.Items;
using Aequus.Items.Accessories.Misc.Fishing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.LegendaryFish {
    public class KryptonFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
            AequusItem.LegendaryFishIDs.Add(Type);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add<Ramishroom>(chance: 1, stack: 1)
                .Add(ItemID.KryptonMoss, chance: 1, stack: (10, 25));
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Batfish);
            Item.questItem = false;
        }
    }
}