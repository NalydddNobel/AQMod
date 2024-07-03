using Aequu2.Core;
using Aequu2.Content.Configuration;
using Aequu2.Content.Items.Tools.NameTag;
using Aequu2.DataSets;
using Aequu2.DataSets.Structures.DropRulesChest;
using Aequu2.DataSets.Structures.Enums;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequu2.Content.Chests;

public class ChestPopulator : ISetupContent {
    public void SetupContent(Mod mod) {
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Surface, ItemID.SlimeCrown, chanceDemoninator: 7, conditions: Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.SlimeCrownInSurfaceChests)));

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Underground, ItemID.Toolbelt, chanceDemoninator: 7, conditions: Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveToolbelt)));
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Underground, ModContent.ItemType<NameTag>(), chanceDemoninator: 4);

        ChestLootDatabase.Instance.Register(ChestPool.Shadow, new ReplaceItemChestRule(
            ItemIdToReplace: ItemID.TreasureMagnet,
            new RemoveItemChestRule(),
            Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveTreasureMagnet))
        ));

#if !DEBUG
        ChestLootDatabase.Instance.RegisterIndexed(ChestPool.Dungeon, [
            new CommonChestRule(ModContent.ItemType<Old.Content.Items.Weapons.Melee.Valari.Valari>())
        ]);
#endif
    }
}
