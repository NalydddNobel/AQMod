using AequusRemake.Systems.Chests.DropRules;
using AequusRemake.Systems.Fishing;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Structures.Conversion;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
    IFishDropRule ToFishDropRule() { return null; }
}
