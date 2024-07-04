using AequusRemake.DataSets.Structures.DropRulesChest;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.DataSets.Structures;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
}
