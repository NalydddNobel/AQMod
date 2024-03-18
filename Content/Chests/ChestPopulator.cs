using Aequus.Common.Chests;
using Aequus.Content.Configuration;
using Aequus.Content.Tools.NameTag;
using Aequus.Core.Initialization;

namespace Aequus.Content.Chests;

public class ChestPopulator : ISetStaticDefaults {
    public void SetStaticDefaults(Aequus aequus) {
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.Surface, ItemID.SlimeCrown, chanceDemoninator: 7, conditions: Aequus.ConditionConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.SlimeCrownInSurfaceChests)));
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.Underground, ItemID.Toolbelt, chanceDemoninator: 7, conditions: Aequus.ConditionConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveToolbelt)));
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.Underground, ModContent.ItemType<NameTag>(), chanceDemoninator: 4);
        ChestLootDatabase.Instance.Register(ChestLoot.Shadow, new ChestRules.Replace(
            ItemIdToReplace: ItemID.TreasureMagnet,
            new ChestRules.Remove(),
            Aequus.ConditionConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveTreasureMagnet))
        ));
    }

    public void Load(Mod mod) { }
    public void Unload() { }
}
