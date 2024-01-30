using Aequus.Content.Configuration;
using Terraria.WorldBuilding;

namespace Aequus.Content.VanillaChanges;

public class TreasureMagnetChanges : GlobalItem {
    public override System.Boolean IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveTreasureMagnet;
    }

    public override System.Boolean AppliesToEntity(Item entity, System.Boolean lateInstantiation) {
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
