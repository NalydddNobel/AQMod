using Aequus.Common.Chests;
using Aequus.Content.Potions.Healing.Restoration;
using Aequus.Content.Tools.Keys;
using Aequus.Core.CrossMod;
using Aequus.DataSets;

namespace Aequus.Content.Chests.BuriedChests;

public class CopperChest : BuriedChestTemplate {
    internal CopperChest() : base(new ChestInfo(
            Key: new ProvideGenericTypeModContentId<CopperKey>(),
            LootPool: ChestLoot.BuriedCopperChest,
            MapEntryColor: Color.OrangeRed.SaturationMultiply(0.4f)
        )) {
    }

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();
        DustType = DustID.Copper;
    }

    public override void PostSetDefaults() {
        ChestLoot type = ChestLoot.BuriedCopperChest;
        ChestLootDatabase.Instance.RegisterOneFromOptions(type, new IChestLootRule[] {
            new ChestRules.Common(ItemID.BandofRegeneration),
            new ChestRules.Common(ItemID.MagicMirror),
            new ChestRules.Common(ItemID.CloudinaBottle),
            new ChestRules.Common(ItemID.HermesBoots),
            new ChestRules.Common(ItemID.Mace),
            new ChestRules.Common(ItemID.ShoeSpikes),
            new ChestRules.Common(ItemID.Extractinator),
            new ChestRules.Common(ItemID.FlareGun).OnSucceed(new ChestRules.Common(ItemID.Flare, MinStack: 10, MaxStack: 25)),
        });
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Bomb, minStack: 5, maxStack: 10, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.AngelStatue, chanceDemoninator: 10);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Rope, minStack: 25, maxStack: 50, chanceDemoninator: 6);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.ThrowingKnife, minStack: 15, maxStack: 30, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ModContent.ItemType<LesserRestorationPotion>(), minStack: 1, maxStack: 3, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterOneFromOptions(type, new IChestLootRule[] {
            new ChestRules.Common(ItemID.RegenerationPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.ShinePotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.NightOwlPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.SwiftnessPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.ArcheryPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.GillsPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.HunterPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.MiningPotion, ChanceDenominator: 3),
            new ChestRules.Common(ItemID.TrapsightPotion, ChanceDenominator: 3),
        });
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.Torch, minStack: 5, maxStack: 10, chanceDemoninator: 4);
        ChestLootDatabase.Instance.RegisterCommon(type, ItemID.RecallPotion, chanceDemoninator: 6);
    }
}
