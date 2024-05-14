using Aequus.DataSets;
using System.Collections.Generic;

namespace Aequus.Content.Fishing;
internal class AnglerRewards : ModPlayer {
#if !DEBUG
    public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems) {
        IEnumerable<Item> rewards = GetRewards();
        if (rewards != null) {
            foreach (var i in rewards) {
                rewardItems.Add(i);
            }
        }
    }

    private IEnumerable<Item> GetRewards() {
        int questFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        int questsComplete = Player.anglerQuestsFinished;

        // Roll Omni Bait
        if (Main.rand.Next(25) < questsComplete) {
            yield return new Item(ModContent.ItemType<Old.Content.Fishing.GimmickBait.Omnibait>(), Main.rand.Next(1, 6));
        }
        // Roll Legendberry
        if (Main.rand.Next(45) < questsComplete && Main.rand.NextBool()) {
            yield return new Item(ModContent.ItemType<Old.Content.Fishing.GimmickBait.LegendberryBait>(), 1);
        }

        // Get quest specific items
        if (TryGetQuestSpecificItem(questFish, out Item questSpecificItem)) {
            yield return questSpecificItem;
        }
        yield return new Item(ItemID.SilverCoin);
    }

    private static bool TryGetQuestSpecificItem(int questFish, out Item questSpecificItem) {
        // Give spectral poppers upon returning a hallow fish
        if (FishDataSet.Hallow.Contains(questFish)) {
            questSpecificItem = new Item(ModContent.ItemType<Old.Content.Fishing.Poppers.HallowPopper>(), Main.rand.Next(1, 6)); return true;
        }
        // Give cursed poppers upon returing a corruption fish
        if (FishDataSet.Corrupt.Contains(questFish)) {
            questSpecificItem = new Item(ModContent.ItemType<Old.Content.Fishing.Poppers.CorruptPopper>(), Main.rand.Next(1, 6)); return true;
        }
        // Give ichor poppers upon returing a crimson fish
        if (FishDataSet.Crimson.Contains(questFish)) {
            questSpecificItem = new Item(ModContent.ItemType<Old.Content.Fishing.Poppers.CrimsonPopper>(), Main.rand.Next(1, 6)); return true;
        }

        questSpecificItem = default;
        return false;
    }
#endif
}
