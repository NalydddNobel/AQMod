using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class AnalysisPlayer : ModPlayer {
    public int completed;
    public int timeForNextQuest;
    public bool dayTimeForNextQuest;
    public int questResetTime;
    public AnalysisQuest quest;

    public override void Initialize() {
        completed = 0;
        quest = new AnalysisQuest();
    }

    public override void SaveData(TagCompound tag) {
        tag["Completed"] = completed;

        if (questResetTime > 0) {
            tag["ResetTime"] = questResetTime;
        }

        if (timeForNextQuest > 0) {
            tag["TimeForNextQuest"] = timeForNextQuest;
            tag["DayTimeForNextQuest"] = dayTimeForNextQuest;
        }
        quest.SaveData(tag);
    }

    public override void LoadData(TagCompound tag) {
        quest = AnalysisQuest.LoadData(tag);
        completed = tag.Get<int>("Completed");
        questResetTime = tag.Get<int>("ResetTime");
        timeForNextQuest = tag.Get<int>("TimeForNextQuest");
        dayTimeForNextQuest = tag.Get<bool>("DayTimeForNextQuest");
    }

    public void RefreshQuest(int questsCompletedScaleFactor) {
        timeForNextQuest = 0;
        quest.itemValue = 0;
        quest.itemRarity = 0;
        quest.isValid = false;

        int valueWantedMax = questsCompletedScaleFactor * Item.silver * 25;
        if (questsCompletedScaleFactor > 5) {
            valueWantedMax *= 2;
        }

        int highestValue = 0;
        var rareList = new List<TrackedItemRarity>();
        foreach (var rare in AnalysisSystem.RareTracker) {
            rare.Value.UpdateSearch();
            rareList.Add(rare.Value);
            if (rare.Value.highestValueObtained > highestValue) {
                highestValue = rare.Value.highestValueObtained;
            }
        }

        valueWantedMax = Math.Min(valueWantedMax, highestValue);
        int valueWantedMin = (int)(valueWantedMax * (1 - Math.Pow(0.9999f, questsCompletedScaleFactor)));

        if (rareList.Count == 0) {
            return;
        }

        for (int i = 0; i < 100; i++) {
            var checkRare = Main.rand.Next(rareList);
            if (checkRare.highestValueObtained >= valueWantedMax) {
                //Main.NewText($"{checkRare.rare}: {checkRare.highestValueObtained}", Color.Red);
                SetQuest(checkRare, Main.rand.Next(valueWantedMin, valueWantedMax));
                break;
            }
        }
    }

    public void SetQuest(TrackedItemRarity rarity, int value) {
        quest.itemRarity = rarity.rare;
        quest.itemValue = value;
        quest.isValid = true;
        questResetTime = 43200;
    }

    public override void PostUpdate() {
        if (timeForNextQuest != 0 && Main.dayTime == dayTimeForNextQuest && Main.time >= timeForNextQuest) {
            timeForNextQuest = 0;
            dayTimeForNextQuest = false;
            if (questResetTime >= 0) {
                quest.isValid = false;
            }

            questResetTime = -1;
        }
        if (questResetTime > 0) {
            questResetTime--;
            if (questResetTime == 0) {
                quest.isValid = false;
            }
        }
    }

    public IEnumerable<Item> GetAnalysisRewardDrops() {
        var primaryRewardsList = AnalysisSystem.PhysicistPrimaryRewardItems;

        List<int> potentialRewards = new List<int>(primaryRewardsList.Count);
        foreach (int rewardItem in primaryRewardsList) {
            if (!Player.HasItemInAnyInventory(rewardItem)) {
                continue;
            }

            // Check if the player has this item equipped in their light pet slot.
            Item lightPet = Player.miscEquips[Player.miscSlotLight];
            if (lightPet.IsAir && lightPet.type == rewardItem) {
                continue;
            }

            potentialRewards.Add(rewardItem);
        }

        // If the player has all of the primary items in their inventory, allow them to roll them all.
        if (potentialRewards.Count == 0) {
            foreach (int rewardItem in primaryRewardsList) {
                potentialRewards.Add(rewardItem);
            }
        }

        // Return a random primary reward
        yield return new Item(Main.rand.Next(potentialRewards));

        // Iterate through all secondary rewards, and grant them randomly (50% chance per secondary reward type.)
        foreach (int rewardItem in AnalysisSystem.PhysicistSecondaryRewardItems) {
            if (Main.rand.NextBool()) {
                yield return new Item(rewardItem);
            }
        }
    }
}