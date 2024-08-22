using Aequus.Common.Utilities;
using Aequus.Common.World;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Entities.DropRules;

public class BlockExistsInWorldCondition(int Type) : IItemDropRuleCondition {
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) {
        return ModContent.GetInstance<TilesInWorld>().Found[Type];
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() {
        return ModContent.GetInstance<TilesInWorld>().Found[Type];
    }

    string IProvideItemConditionDescription.GetConditionDescription() {
        return ALanguage.GetText($"ItemDropCondition.BlockExists").Format(Lang._mapLegendCache.FromType(Type));
    }
}
