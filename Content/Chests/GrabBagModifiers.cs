using Aequus.Content.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Chests;

public class GrabBagModifiers : GlobalItem {
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        switch (item.type) {
            case ItemID.ObsidianLockbox: {
                    if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
                        itemLoot.RemoveItemId(ItemID.TreasureMagnet);
                    }
                }
                break;
        }
    }
}