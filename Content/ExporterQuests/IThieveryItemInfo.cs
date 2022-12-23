using Terraria;

namespace Aequus.Content.ExporterQuests
{
    public interface IThieveryItemInfo
    {
        void SpawnLoot(Player player, int thieveryItemInventoryIndex);
        void OnQuestCompleted(Player player, int thieveryItemInventoryIndex)
        {
        }
    }
}