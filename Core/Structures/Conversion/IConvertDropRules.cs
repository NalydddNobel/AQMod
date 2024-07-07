using AequusRemake.DataSets.Structures.DropRulesChest;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Structures.Conversion;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
}
