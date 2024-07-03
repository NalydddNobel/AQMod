using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Core.Entities.Items.DropRules;

// Drop Rules related to items being inside an NPC when they spawn.
public interface IBodyDropRule { }
public interface IBodyItemContainer {
    int ItemId { get; set; }
    int Stack { get; set; }

    public static ItemDropAttemptResult RollRules(NPC npc, Player player) {
        ItemDropAttemptResult result = default;
        DropAttemptInfo info = ExtendLoot.GetDropAttemptInfo(npc, player);
        List<IItemDropRule> rules = ExtendLoot.GetDropRules(npc.type);

        // Iterate through all drop rules until one returns a success.
        foreach (IItemDropRule rule in rules.Where(r => r is IBodyDropRule)) {
            result = ExtendLoot.ResolveRule(rule, in info);
            if (result.State == ItemDropAttemptResultState.Success) {
                break;
            }
        }

        return result;
    }

    public void DropItem(IEntitySource source, Rectangle hitbox) {
        if (ItemId > ItemID.None) {
            Item.NewItem(source, hitbox, ItemId, Stack);
        }
    }
}

public class CommonBodyDropRule(int itemId, int chanceDenominator, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1) : CommonDrop(itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator), IBodyDropRule {
    // Prevent on-kill drop rule logic.
    public override bool CanDrop(DropAttemptInfo info) {
        return false;
    }

    public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
        // An Opaque Slime is required for special logic.
        if (info.npc?.ModNPC is not IBodyItemContainer bodyContainer) {
            return base.TryDroppingItem(info);
        }

        if (info.player.RollLuck(chanceDenominator) < chanceNumerator) {
            bodyContainer.ItemId = itemId;
            bodyContainer.Stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
            return new ItemDropAttemptResult() with { State = ItemDropAttemptResultState.Success };
        }

        return new ItemDropAttemptResult() with { State = ItemDropAttemptResultState.FailedRandomRoll };
    }
}

public class OneFromOptionsBodyDropRule(int chanceDenominator, int chanceNumerator, params int[] options) : IItemDropRule, IBodyDropRule {
    public int[] dropIds = options;
    public int chanceDenominator = chanceDenominator;
    public int chanceNumerator = chanceNumerator;

    public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; } = new();

    // Prevent on-kill drop rule logic.
    public bool CanDrop(DropAttemptInfo info) {
        return false;
    }

    public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
        if (info.player.RollLuck(chanceDenominator) < chanceNumerator) {
            int itemToDrop = dropIds[info.rng.Next(dropIds.Length)];

            // An Opaque Slime is required for special logic.
            if (info.npc?.ModNPC is IBodyItemContainer bodyContainer) {
                bodyContainer.ItemId = itemToDrop;
            }
            else {
                CommonCode.DropItem(info, itemToDrop, 1);
            }

            return new ItemDropAttemptResult() with { State = ItemDropAttemptResultState.Success };
        }

        return new ItemDropAttemptResult() with { State = ItemDropAttemptResultState.FailedRandomRoll };
    }

    public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
        float num = chanceNumerator / (float)chanceDenominator;
        float num2 = num * ratesInfo.parentDroprateChance;
        float dropRate = 1f / dropIds.Length * num2;
        for (int i = 0; i < dropIds.Length; i++) {
            drops.Add(new DropRateInfo(dropIds[i], 1, 1, dropRate, ratesInfo.conditions));
        }

        Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
    }
}
