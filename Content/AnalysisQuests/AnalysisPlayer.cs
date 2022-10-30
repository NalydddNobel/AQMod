using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.AnalysisQuests
{
    public class AnalysisPlayer : ModPlayer
    {
        public int completed;
        public int timeForNextQuest;
        public bool dayTimeForNextQuest;
        public int questResetTime;
        public QuestInfo quest;

        public override void Initialize()
        {
            completed = 0;
            quest = new QuestInfo();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Completed"] = completed;

            if (questResetTime > 0)
            {
                tag["ResetTime"] = questResetTime;
            }

            if (timeForNextQuest > 0)
            {
                tag["TimeForNextQuest"] = timeForNextQuest;
                tag["DayTimeForNextQuest"] = dayTimeForNextQuest;
            }
            quest.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            quest = QuestInfo.LoadData(tag);
            completed = tag.Get<int>("Completed");
            questResetTime = tag.Get<int>("ResetTime");
            timeForNextQuest = tag.Get<int>("TimeForNextQuest");
            dayTimeForNextQuest = tag.Get<bool>("DayTimeForNextQuest");
        }

        public void RefreshQuest(int questsCompletedScaleFactor)
        {
            timeForNextQuest = 0;
            quest.itemValue = 0;
            quest.itemRarity = 0;
            quest.isValid = false;

            int valueWantedMax = questsCompletedScaleFactor * Item.silver * 25;
            if (questsCompletedScaleFactor > 5)
            {
                valueWantedMax *= 2;
            }

            int highestValue = 0;
            var rareList = new List<TrackedItemRarity>();
            foreach (var rare in AnalysisSystem.RareTracker)
            {
                rare.Value.UpdateSearch();
                rareList.Add(rare.Value);
                if (rare.Value.highestValueObtained > highestValue)
                {
                    highestValue = rare.Value.highestValueObtained;
                }
            }

            valueWantedMax = Math.Min(valueWantedMax, highestValue);
            int valueWantedMin = (int)(valueWantedMax * (1 - Math.Pow(0.9999f, questsCompletedScaleFactor)));

            //Main.NewText(highestValue, Color.BlueViolet);
            //Main.NewText(valueWantedMin, Color.AliceBlue);
            //Main.NewText(valueWantedMax, Color.Orange);
            //Main.NewText(highestValueSearched, Color.Red);

            if (rareList.Count == 0)
                return;

            for (int i = 0; i < 100; i++)
            {
                var checkRare = Main.rand.Next(rareList);
                if (checkRare.highestValueObtained >= valueWantedMax)
                {
                    //Main.NewText($"{checkRare.rare}: {checkRare.highestValueObtained}", Color.Red);
                    SetQuest(checkRare, Main.rand.Next(valueWantedMin, valueWantedMax));
                    break;
                }
            }
        }

        public void SetQuest(TrackedItemRarity rarity, int value)
        {
            Main.NewText("Set Quest");
            quest.itemRarity = rarity.rare;
            quest.itemValue = value;
            quest.isValid = true;
            questResetTime = 43200;
        }

        public override void PostUpdate()
        {
            if (timeForNextQuest != 0 && Main.dayTime == dayTimeForNextQuest && Main.time >= timeForNextQuest)
            {
                timeForNextQuest = 0;
                dayTimeForNextQuest = false;
                if (questResetTime >= 0)
                    quest.isValid = false;
                questResetTime = -1;
            }
            if (questResetTime > 0)
            {
                questResetTime--;
                if (questResetTime == 0)
                {
                    quest.isValid = false;
                }
            }
        }
    }
}
