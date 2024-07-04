using AequusRemake.Content.Items.Potions.Healing.Restoration;
using AequusRemake.Content.Items.Tools.Keys;
using AequusRemake.Core.Structures.ID;
using AequusRemake.DataSets;
using AequusRemake.DataSets.Structures.DropRulesChest;
using AequusRemake.DataSets.Structures.Enums;

namespace AequusRemake.Content.Chests.BuriedChests;

public class CopperChest : UnifiedBuriedChest {
    internal CopperChest() : base(new ChestInfo(
            Key: new TypeId<CopperKey>(),
            LootPool: ChestPool.BuriedCopperChest,
            MapEntryColor: Color.OrangeRed.SaturationMultiply(0.4f)
        )) {
    }

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();
        DustType = DustID.Copper;
    }

    public override void PostSetDefaults() {
        ChestPool type = ChestPool.BuriedCopperChest;
        ChestLootDatabase.Instance.RegisterOneFromOptions(type, new IChestLootRule[] {
            new CommonChestRule(ItemID.BandofRegeneration),
            new CommonChestRule(ItemID.MagicMirror),
            new CommonChestRule(ItemID.CloudinaBottle),
            new CommonChestRule(ItemID.HermesBoots),
            new CommonChestRule(ItemID.Mace),
            new CommonChestRule(ItemID.ShoeSpikes),
            new CommonChestRule(ItemID.Extractinator),
            new CommonChestRule(ItemID.FlareGun).OnSucceed(new CommonChestRule(ItemID.Flare, MinStack: 10, MaxStack: 25)),
        });
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Bomb, minStack: 5, maxStack: 10, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.AngelStatue, chanceDemoninator: 10);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Rope, minStack: 25, maxStack: 50, chanceDemoninator: 6);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.ThrowingKnife, minStack: 15, maxStack: 30, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ModContent.ItemType<LesserRestorationPotion>(), minStack: 1, maxStack: 3, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterOneFromOptions(type, new IChestLootRule[] {
            new CommonChestRule(ItemID.RegenerationPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.ShinePotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.NightOwlPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.SwiftnessPotion, ChanceDenominator : 3),
            new CommonChestRule(ItemID.ArcheryPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.GillsPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.HunterPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.MiningPotion, ChanceDenominator: 3),
            new CommonChestRule(ItemID.TrapsightPotion, ChanceDenominator: 3),
        });
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Torch, minStack: 5, maxStack: 10, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.RecallPotion, chanceDemoninator: 6);
    }
}
