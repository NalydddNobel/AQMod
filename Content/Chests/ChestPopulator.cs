using AequusRemake.Content.Configuration;
using AequusRemake.Content.Items.Tools.NameTag;
using AequusRemake.Core.Structures.Chests;
using AequusRemake.Core.Structures.Enums;
using AequusRemake.DataSets.Structures.DropRulesChest;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Content.Chests;

public class ChestPopulator : ISetupContent {
    public void SetupContent(Mod mod) {
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Surface, ItemID.SlimeCrown, chanceDemoninator: 7, conditions: ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.SlimeCrownInSurfaceChests)));

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Underground, ItemID.Toolbelt, chanceDemoninator: 7, conditions: ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveToolbelt)));
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Underground, ModContent.ItemType<NameTag>(), chanceDemoninator: 4);

        ChestLootDatabase.Instance.Register(ChestPool.Shadow, new ReplaceItemChestRule(
            ItemIdToReplace: ItemID.TreasureMagnet,
            new RemoveItemChestRule(),
            ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveTreasureMagnet))
        ));
    }
}
