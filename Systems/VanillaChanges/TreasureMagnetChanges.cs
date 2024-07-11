using AequusRemake.Content.Configuration;
using AequusRemake.Systems.Chests;
using AequusRemake.Systems.Chests.DropRules;
using AequusRemake.Systems.Configuration;
using Terraria.WorldBuilding;

namespace AequusRemake.Systems.VanillaChanges;

public class TreasureMagnetChanges : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.MoveTreasureMagnet;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.type == ItemID.TreasureMagnet;
    }

    public override void SetStaticDefaults() {
        ChestLootDatabase.Instance.Register(ChestPool.Shadow, new ReplaceItemChestRule(
            ItemIdToReplace: ItemID.TreasureMagnet,
            new RemoveItemChestRule(),
            ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveTreasureMagnet))
        ));
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
            ArrayExtensions.Remove(ref GenVars.hellChestItem, ItemID.TreasureMagnet);
        }
    }

    public static void CheckShadowChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            chest.ReplaceFirst(ItemID.TreasureMagnet, ItemID.HellstoneBar, WorldGen.genRand.Next(10, 21));
        }
    }
}
