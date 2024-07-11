using AequusRemake.Content.Configuration;
using AequusRemake.Systems.Chests;
using AequusRemake.Systems.Configuration;

namespace AequusRemake.Systems.VanillaChanges;

public class SlimeCrownChanges : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return VanillaChangesConfig.Instance.SlimeCrownInSurfaceChests;
    }

    public override void SetStaticDefaults() {
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.Surface, ItemID.SlimeCrown, chanceDemoninator: 7, conditions: ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.SlimeCrownInSurfaceChests)));
    }
}
