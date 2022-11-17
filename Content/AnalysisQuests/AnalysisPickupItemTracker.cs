using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.AnalysisQuests
{
    public class AnalysisPickupItemTracker : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (item.value > 0 && !item.IsACoin && !item.questItem && !item.vanity)
            {
                int rare = item.OriginalRarity;
                if (!AnalysisSystem.IgnoreRarities.Contains(rare))
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var p = Aequus.GetPacket(PacketType.SyncRarityObtained);
                        p.Write(rare);
                        p.Write(item.value);
                        p.Send();
                    }
                    else
                    {
                        if (AnalysisSystem.RareTracker.TryGetValue(rare, out var val))
                        {
                            val.highestValueObtained = Math.Max(val.highestValueObtained, item.value);
                        }
                        else
                        {
                            AnalysisSystem.RareTracker[rare] = new TrackedItemRarity() { rare = rare, highestValueObtained = item.value };
                        }
                    }

                    //Main.NewText($"{item.value} | {rare}:{AequusText.GetRarityNameValue(rare)}");
                    //Main.NewText($"{AnalysisSystem.quest.itemValue} | {AnalysisSystem.quest.itemRarity}:{AequusText.GetRarityNameValue(AnalysisSystem.quest.itemRarity)} | {AnalysisSystem.quest.itemValue}", Main.DiscoColor);
                    //Main.NewText($"{AnalysisSystem.RareTracker[rare].highestValueObtained}", Main.DiscoColor);
                }
            }
            return true;
        }
    }
}