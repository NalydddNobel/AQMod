using AequusRemake.Content.Tiles.PollutedOcean.PolymerSands;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Systems.Fishing.Crates;

internal class PollutedOceanCrate : UnifiedModCrate {
    public override IEnumerable<IItemDropRule> ModifyCrateLoot(ModItem crate, ItemLoot loot) {
        foreach (IItemDropRule rule in base.ModifyCrateLoot(crate, loot)) {
            yield return rule;
        }

        yield return ItemDropRule.NotScalingWithLuck(PolymerSand.Item.Type, 3, 20, 50);
    }
}
