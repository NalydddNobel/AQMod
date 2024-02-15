using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using System;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class AnalysisPickupItemTracker : GlobalItem {
    public override bool OnPickup(Item item, Player player) {
        if (item.value > 0 && !item.IsACoin && !item.questItem && !item.vanity) {
            int rare = item.OriginalRarity;
            if (!AnalysisSystem.IgnoreRarities.Contains(rare)) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    Aequus.GetPacket<AnalysisItemPickupPacket>().Send(player, item);
                }
                else {
                    AnalysisSystem.HandleItemPickup(player, item);
                }

                //Main.NewText($"{item.value} | {rare}:{AequusText.GetRarityNameValue(rare)}");
                //Main.NewText($"{AnalysisSystem.quest.itemValue} | {AnalysisSystem.quest.itemRarity}:{AequusText.GetRarityNameValue(AnalysisSystem.quest.itemRarity)} | {AnalysisSystem.quest.itemValue}", Main.DiscoColor);
                //Main.NewText($"{AnalysisSystem.RareTracker[rare].highestValueObtained}", Main.DiscoColor);
            }
        }
        return true;
    }
}