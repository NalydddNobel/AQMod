using Aequus.DataSets.Structures.DropRulesChest;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.DataSets.Structures;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
}
