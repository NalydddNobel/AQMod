using Aequus.Common;
using Aequus.Content.Configuration;
using Aequus.Content.Tools.NameTag;
using Aequus.Core.Initialization;
using Aequus.DataSets;
using Aequus.DataSets.Structures.DropRulesChest;
using Aequus.DataSets.Structures.Enums;
using System.Security.Cryptography;

namespace Aequus.Content.Chests;

public class ChestPopulator : ISetStaticDefaults {
    public void SetStaticDefaults(Aequus aequus) {
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
            new CommonChestRule(ModContent.ItemType<Old.Content.Weapons.Melee.Valari.Valari>())
        ]);
#endif
    }

    public void Load(Mod mod) { }
    public void Unload() { }
}
