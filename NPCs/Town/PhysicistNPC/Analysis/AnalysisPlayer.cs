using Aequus.Items;
using Aequus.Items.Accessories.Life;
using Aequus.Items.Accessories.Misc.Info;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Tiles.Furniture.Gravity;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Town.PhysicistNPC.Analysis {
    public class AnalysisPlayer : ModPlayer {
        public int completed;
        public int timeForNextQuest;
        public bool dayTimeForNextQuest;
        public int questResetTime;
        public QuestInfo quest;

        public override void Initialize() {
            completed = 0;
            quest = new QuestInfo();
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
            quest = QuestInfo.LoadData(tag);
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

            //Main.NewText(highestValue, Color.BlueViolet);
            //Main.NewText(valueWantedMin, Color.AliceBlue);
            //Main.NewText(valueWantedMax, Color.Orange);
            //Main.NewText(highestValueSearched, Color.Red);

            if (rareList.Count == 0)
                return;

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
                if (questResetTime >= 0)
                    quest.isValid = false;
                questResetTime = -1;
            }
            if (questResetTime > 0) {
                questResetTime--;
                if (questResetTime == 0) {
                    quest.isValid = false;
                }
            }
        }

        public List<Item> GetAnalysisRewardDrops() {
            var l = new List<Item>();
            AddMainRewardDrops(l);
            if (Main.rand.NextBool()) {
                l.Add(AequusItem.SetDefaults<GravityChest>());
            }
            return l;
        }

        public void AddMainRewardDrops(List<Item> item) {
            bool addedAny = false;
            var potentialRewards = new List<Item>();
            if (!Player.HasItemCheckAllBanks<PersonalDronePack>() && Player.miscEquips[Player.miscSlotLight].type != ModContent.ItemType<PersonalDronePack>()) {
                potentialRewards.Add(AequusItem.SetDefaults<PersonalDronePack>());
                addedAny = true;
            }
            if (!Player.HasItemCheckAllBanks<HoloLens>()) {
                potentialRewards.Add(AequusItem.SetDefaults<HoloLens>());
                addedAny = true;
            }
            if (!Player.HasItemCheckAllBanks<HyperJet>()) {
                potentialRewards.Add(AequusItem.SetDefaults<HyperJet>());
                addedAny = true;
            }
            if ((!addedAny || Main.netMode != NetmodeID.SinglePlayer) && !Player.HasItemCheckAllBanks<PhaseMirror>()) {
                potentialRewards.Add(AequusItem.SetDefaults<PhaseMirror>());
                addedAny = true;
            }
            if (!addedAny) {
                potentialRewards.Add(AequusItem.SetDefaults<HyperJet>());
                potentialRewards.Add(AequusItem.SetDefaults<PersonalDronePack>());
                potentialRewards.Add(AequusItem.SetDefaults<HoloLens>());
                potentialRewards.Add(AequusItem.SetDefaults<PhaseMirror>());
            }
            item.Add(Main.rand.Next(potentialRewards));
            potentialRewards.Clear();
        }
    }
}