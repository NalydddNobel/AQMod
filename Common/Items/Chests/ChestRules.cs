using Aequus.Content.Chests;
using Aequus.Core;
using System.Collections.Generic;

namespace Aequus.Common.Items.Chests;
internal class ChestRules {
    public record class Common(int Item, int MinStack = 1, int MaxStack = 1, params Condition[] OptionalConditions) : IChestLootRule {
        public List<IChestLootChain> ChainedRules { get; set; } = new();
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            Main.chest[info.Chest].AddItem(Item, info.RNG.Next(MinStack, MaxStack + 1));
            return ChestLootResult.Success;
        }
    }
    /// <summary>This rule goes through each rule sequentually. This is similar to how items in the Dungeon are handled.</summary>
    public record class Indexed(IChestLootRule[] Rules, params Condition[] OptionalConditions) : IChestLootRule {
        public int Index { get; private set; }
        public int RealIndex => Index % Rules.Length;

        public List<IChestLootChain> ChainedRules { get; set; } = new();
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            IChestLootRule selectedRule = Rules[RealIndex];
            ChestLootDatabase.Instance.SolveSingleRule(selectedRule, in info);

            Index++;
            return ChestLootResult.Success;
        }

        public bool CanDrop(in ChestLootInfo info) {
            return true;
        }

        public void Reset() {
            ResetIndex();
        }

        public void ResetIndex() {
            Index = 0;
        }
    }
}
