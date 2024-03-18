using Aequus.Content.Chests;
using Aequus.Core;
using System.Collections.Generic;
using System.Data;

namespace Aequus.Common.Chests;

internal class ChestRules {
    public record class Common(int Item, int MinStack = 1, int MaxStack = 1, int ChanceDenominator = 1, int ChanceNumerator = 1, params Condition[] OptionalConditions) : IChestLootRule {
        public List<IChestLootChain> ChainedRules { get; set; } = new();
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            // Roll RNG.
            if (info.RNG.Next(ChanceDenominator) >= ChanceNumerator) {
                return ChestLootResult.FailedRandomRoll;
            }

            // Add the item.
            info.Add.AddItem(Item, info.RNG.Next(MinStack, MaxStack + 1), in info);
            return ChestLootResult.Success;
        }
    }

    public record class OneFromOptions(int[] Options, int ChanceDenominator = 1, int ChanceNumerator = 1, params Condition[] OptionalConditions) : IChestLootRule {
        public List<IChestLootChain> ChainedRules { get; set; } = new();
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            // Roll RNG.
            if (info.RNG.Next(ChanceDenominator) >= ChanceNumerator) {
                return ChestLootResult.FailedRandomRoll;
            }

            // Add the item.
            info.Add.AddItem(info.RNG.Next(Options), 1, in info);
            return ChestLootResult.Success;
        }
    }

    /// <summary>This rule goes through each rule sequentually. This is similar to how items in the Dungeon are handled.</summary>    
    public record class Indexed(IChestLootRule[] Rules, params Condition[] OptionalConditions) : IChestLootRule {
        public int Index { get; private set; }
        public int RuleIndex => Index % Rules.Length;

        public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Rules);
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            // Get the next rule index.
            IChestLootRule selectedRule = Rules[RuleIndex];

            // Solve that rule.
            ChestLootResult result = ChestLootDatabase.Instance.SolveSingleRule(selectedRule, in info);

            // Increment the index, and return the result.
            Index++;
            return result;
        }

        public void Reset() {
            // Reset the index upon world generation and etc.
            ResetIndex();
        }

        public void ResetIndex() {
            Index = 0;
        }
    }

    /// <summary>
    /// Finds <paramref name="ItemIdToReplace"/>, deletes the item, and runs <paramref name="Rule"/> on its slot with an <see cref="InsertToChest"/> context.
    /// <para>
    /// To remove an item properly, use <see cref="Remove"/> as the only <paramref name="Rule"/> to pull all of the slots backwards upon deleting the item.
    /// </para>
    /// </summary>
    public record class Replace(int ItemIdToReplace, IChestLootRule Rule, params Condition[] OptionalConditions) : IChestLootRule {
        public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Rule);
        public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

        public ChestLootResult AddItem(in ChestLootInfo info) {
            Chest chest = Main.chest[info.ChestId];
            for (int i = 0; i < chest.item.Length; i++) {
                if (!chest.item[i].IsAir && chest.item[i].type == ItemIdToReplace) {
                    // Delete item
                    chest.item[i].TurnToAir();

                    // Run Rule with "Insert" properties.
                    ChestLootInfo childInfo = new ChestLootInfo(info, new InsertToChest(i));
                    ChestLootDatabase.Instance.SolveSingleRule(Rule, in childInfo);

                    // Success!!
                    return ChestLootResult.Success;
                }
            }

            // No loot-related code was ran, since there was no item to replace.
            return ChestLootResult.DidNotRunCode;
        }
    }

    /// <summary>Usually used in conjunction with <see cref="Replace"/> to pull all of the item slots backwards after deleting the item.</summary>
    public class Remove : IChestLootRule {
        public List<IChestLootChain> ChainedRules { get; set; } = new();
        public ConditionCollection Conditions { get; set; } = null;

        public ChestLootResult AddItem(in ChestLootInfo info) {
            info.Add.RemoveItem(in info);
            return ChestLootResult.Success;
        }
    }
}
