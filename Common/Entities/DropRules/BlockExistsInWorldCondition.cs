using Aequus.Common.Utilities;
using Aequus.Common.World;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.Entities.DropRules;

public class BlockExistsInWorldCondition(int Type) : IItemDropRuleCondition {
    private LocalizedText _text = ALanguage.GetText($"ItemDropCondition.BlockExists").WithFormatArgs(Lang._mapLegendCache.FromType(Type));

    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) {
        return ModContent.GetInstance<TilesInWorld>().Found[Type];
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() {
        return ModContent.GetInstance<TilesInWorld>().Found[Type];
    }

    string IProvideItemConditionDescription.GetConditionDescription() {
        return _text.Value;
    }
}
