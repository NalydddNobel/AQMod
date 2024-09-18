#if !CRAB_CREVICE_DISABLE
using Aequus.Common.DataSets;
using Aequus.Items.Tools.FishingPoles;

namespace Aequus.Items.Misc.FishCatches.LegendaryFish;

public class CrabDaughter : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 2;
        ItemSets.LegendaryFish.Add(Type);
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        this.CreateLoot(itemLoot)
            .Add<CrabRod>(chance: 1, stack: 1);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Batfish);
        Item.questItem = false;
    }
}
#endif