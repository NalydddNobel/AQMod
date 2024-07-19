using Aequus.Systems.Chests.DropRules;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Structures.Conversion;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
    //IFishDropRule ToFishDropRule() { return null; }
}
