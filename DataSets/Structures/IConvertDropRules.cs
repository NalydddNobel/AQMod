using Aequu2.DataSets.Structures.DropRulesChest;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.DataSets.Structures;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
}
