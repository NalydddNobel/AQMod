using Aequus.Common;
using Aequus.Common.Chests;
using Aequus.Content.Configuration;
using Aequus.Content.Tools.NameTag;
using Aequus.Core.Initialization;
using Aequus.DataSets;

namespace Aequus.Content.Chests;

public class ChestPopulator : ISetStaticDefaults {
    public void SetStaticDefaults(Aequus aequus) {
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.Surface, ItemID.SlimeCrown, chanceDemoninator: 7, conditions: Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.SlimeCrownInSurfaceChests)));
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.AllUnderground, ItemID.Toolbelt, chanceDemoninator: 7, conditions: Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveToolbelt)));
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.AllUnderground, ModContent.ItemType<NameTag>(), chanceDemoninator: 4);
        ChestLootDatabase.Instance.Register(ChestLoot.Shadow, new ChestRules.ReplaceItem(
            ItemIdToReplace: ItemID.TreasureMagnet,
            new ChestRules.Remove(),
            Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveTreasureMagnet))
        ));
    }

    public void Load(Mod mod) { }
    public void Unload() { }
}
