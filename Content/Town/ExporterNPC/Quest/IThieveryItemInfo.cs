using Terraria;

namespace Aequus.Content.Town.ExporterNPC.Quest
{
    public interface IThieveryItemInfo
    {
        void SpawnLoot(Player player, int thieveryItemInventoryIndex);
        void OnQuestCompleted(Player player, int thieveryItemInventoryIndex)
        {
        }
    }
}