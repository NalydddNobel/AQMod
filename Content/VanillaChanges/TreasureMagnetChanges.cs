using AequusRemake.Content.Configuration;
using Terraria.WorldBuilding;

namespace AequusRemake.Content.VanillaChanges;

public class TreasureMagnetChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveTreasureMagnet;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.TreasureMagnet;
    }

    public override void SetDefaults(Item entity) {
        entity.StatsModifiedBy.Add(Mod);
    }

    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (item.type == ItemID.ObsidianLockbox) {
            itemLoot.RemoveItemId(ItemID.TreasureMagnet);
        }
    }

    public static void RemoveTreasureMagnetFromHellChestArray() {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            ExtendArray.Remove(ref GenVars.hellChestItem, ItemID.TreasureMagnet);
        }
    }

    public static void CheckShadowChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            chest.ReplaceFirst(ItemID.TreasureMagnet, ItemID.HellstoneBar, WorldGen.genRand.Next(10, 21));
        }
    }
}
