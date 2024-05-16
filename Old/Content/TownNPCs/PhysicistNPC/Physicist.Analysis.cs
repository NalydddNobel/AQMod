using Aequus.DataSets;
using Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using tModLoaderExtended;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

public partial class Physicist {
    public void SetAnalaysisButton(ref string button) {
        if (Main.npcChatCornerItem > 0) {
            button = this.GetLocalizedValue("Analysis.CompleteButton");
        }
        else {
            button = this.GetLocalizedValue("Analysis.Button");
        }
    }

    public void QuestButtonPressed() {
        var player = Main.LocalPlayer;
        var questPlayer = player.GetModPlayer<AnalysisPlayer>();

        if (!questPlayer.quest.isValid && questPlayer.timeForNextQuest == 0 && questPlayer.questResetTime <= 0) {
            questPlayer.quest = default;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                ExtendedMod.GetPacket<PacketRequestNewAnalysisQuest>().Send(player);
            }
            else {
                questPlayer.RefreshQuest(questPlayer.completed);
            }
        }

        if (!questPlayer.quest.isValid || questPlayer.timeForNextQuest > 0) {
            Main.npcChatText = this.GetLocalization("Analysis.ChatNoQuest").FormatWith(new { Time = ExtendLanguage.WatchTime(questPlayer.timeForNextQuest, questPlayer.dayTimeForNextQuest), });
            return;
        }

        var validItem = FindPotentialQuestItem(player, questPlayer.quest);
        if (Main.npcChatCornerItem > 0 && validItem != null) {
            var popupItem = validItem.Clone();
            popupItem.position = player.position;
            popupItem.width = player.width;
            popupItem.height = player.height;
            popupItem.stack = 1;
            var itemText = PopupText.NewText(PopupTextContext.RegularItemPickup, popupItem, 1);
            Main.popupText[itemText].name = $"{Main.popupText[itemText].name} (-1)";
            Main.popupText[itemText].position.X = Main.LocalPlayer.Center.X - FontAssets.ItemStack.Value.MeasureString(Main.popupText[itemText].name).X / 2f;

            validItem.stack--;
            if (validItem.stack <= 0) {
                validItem.TurnToAir();
            }
            questPlayer.completed++;
            var items = questPlayer.GetAnalysisRewardDrops();
            var source = player.talkNPC != -1 ? Main.npc[player.talkNPC].GetSource_GiftOrReward() : player.GetSource_GiftOrReward();
            foreach (var i in items) {
                player.QuickSpawnItem(source, i, i.stack);
            }
            int time = Main.rand.Next(28800, 43200);
            AnalysisSystem.AddToTime(Main.time, time, Main.dayTime, out double result, out bool dayTime);
            questPlayer.timeForNextQuest = (int)Math.Min(result, dayTime ? Main.dayLength - 60 : Main.nightLength - 60);
            questPlayer.dayTimeForNextQuest = dayTime;
            SoundEngine.PlaySound(SoundID.Grab);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = this.GetLocalizedValue("Analysis.ChatCompleted");
            return;
        }
        Main.npcChatText = QuestChat(questPlayer.quest);

        if (validItem != null) {
            Main.npcChatCornerItem = validItem.type;
            Main.npcChatText += $"\n{this.GetLocalization("Analysis.ChatFoundItem").FormatWith(new { Item = validItem.Name, })}";
        }
    }
    public static Item FindPotentialQuestItem(Player player, AnalysisQuest questInfo) {
        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            if (CanBeQuestItem(player.inventory[i], questInfo)) {
                return player.inventory[i];
            }
        }
        for (int i = 0; i < Chest.maxItems; i++) {
            if (CanBeQuestItem(player.bank4.item[i], questInfo)) {
                return player.bank4.item[i];
            }
        }
        return null;
    }

    public static bool CanBeQuestItem(Item item, AnalysisQuest questInfo) {
        return !item.favorited && !item.IsAir && !item.IsACoin &&
            item.OriginalRarity == questInfo.itemRarity && !ItemDataSet.CannotTradeWithPhysicist.Contains(item.type) && !Main.itemAnimationsRegistered.Contains(item.type);
    }

    public string QuestChat(AnalysisQuest questInfo) {
        return this.GetLocalization("Analysis.ChatQuest").FormatWith(new {
            Rarity = ChatTagWriter.Color(ItemRarity.GetColor(questInfo.itemRarity), AnalysisSystem.GetRarityName(questInfo.itemRarity).Value),
        });
    }
}