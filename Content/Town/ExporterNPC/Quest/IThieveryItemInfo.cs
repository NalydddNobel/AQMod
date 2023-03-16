using System;
using Terraria;

namespace Aequus.Content.Town.ExporterNPC.Quest
{
    [Obsolete("Exporter thievery was removed.")]
    public interface IThieveryItemInfo
    {
        void SpawnLoot(Player player, int thieveryItemInventoryIndex);
        void OnQuestCompleted(Player player, int thieveryItemInventoryIndex)
        {
        }
    }
}