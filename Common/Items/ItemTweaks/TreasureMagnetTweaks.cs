using Aequus.Content.Configuration;

namespace Aequus.Common.Items.ItemTweaks;

public class TreasureMagnetTweaks : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveTreasureMagnet;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.TreasureMagnet;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[ItemID.TreasureMagnet] = ItemID.CelestialMagnet;
        ItemID.Sets.ShimmerTransformToItem[ItemID.CelestialMagnet] = ItemID.TreasureMagnet;
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.ObsidianLockbox) {
            itemLoot.RemoveItemId(ItemID.TreasureMagnet);
        }
    }
}