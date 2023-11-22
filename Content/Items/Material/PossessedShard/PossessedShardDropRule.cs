using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material.PossessedShard;

public class PossessedShardDropRule : IItemDropRule {
    public int chanceDenominator;
    public int amountDroppedMinimum;
    public int amountDroppedMaximum;
    public int chanceNumerator;

    public List<IItemDropRuleChainAttempt> ChainedRules { get; set; }

    public PossessedShardDropRule(int chanceDenominator, int min = 1, int max = 1, int chanceNumerator = 1) {
        ChainedRules = new();
        this.chanceDenominator = chanceDenominator;
        amountDroppedMinimum = min;
        amountDroppedMaximum = max;
        this.chanceNumerator = chanceNumerator;
    }

    public bool CanDrop(DropAttemptInfo info) {
        return true;
    }

    public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
        float dropChance = chanceNumerator / (float)chanceDenominator;
        float dropRate = dropChance * ratesInfo.parentDroprateChance;
        drops.Add(new DropRateInfo(ModContent.ItemType<PossessedShard>(), amountDroppedMinimum, amountDroppedMaximum, dropRate, ratesInfo.conditions));
        Chains.ReportDroprates(ChainedRules, dropChance, drops, ratesInfo);
    }

    public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
        ItemDropAttemptResult result;
        if (info.player.RollLuck(chanceDenominator) < chanceNumerator) {
            int amount = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
            if (info.npc != null) {
                // Spawn as NPCs if dropped from an NPC
                var hitbox = info.npc.Hitbox;
                var source = info.npc.GetSource_Loot();
                for (int i = 0; i < amount; i++) {
                    NPC.NewNPC(source, hitbox.X + Main.rand.Next(hitbox.Width), hitbox.Y + Main.rand.Next(hitbox.Height), ModContent.NPCType<PossessedShardNPC>());
                }
            }
            else {
                // Spawn as items if dropped from anything else
                CommonCode.DropItem(info, ModContent.NPCType<PossessedShardNPC>(), amount);
            }
            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.Success;
            return result;
        }

        result = default(ItemDropAttemptResult);
        result.State = ItemDropAttemptResultState.FailedRandomRoll;
        return result;
    }
}
