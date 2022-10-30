using System;
using System.IO;

namespace Aequus.Content.AnalysisQuests
{
    public class TrackedItemRarity
    {
        public int rare;
        public int highestValueObtained;

        private bool searched;
        public int LowestValueSearch { get; private set; }
        public int HighestValueSearch { get; private set; }

        public void UpdateSearch()
        {
            if (searched)
                return;

            LowestValueSearch = int.MaxValue;
            if (AnalysisSystem.RarityToItemList.TryGetValue(rare, out var list))
            {
                foreach (var i in list)
                {
                    if (i.value < LowestValueSearch)
                    {
                        LowestValueSearch = i.value;
                    }
                    if (i.value > HighestValueSearch)
                    {
                        HighestValueSearch = i.value;
                    }
                }
            }
            HighestValueSearch = Math.Max(LowestValueSearch, HighestValueSearch);
            searched = true;
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.Write(highestValueObtained);
        }

        public static TrackedItemRarity NetReceive(BinaryReader reader, int rare)
        {
            var val = new TrackedItemRarity()
            {
                rare = rare,
                highestValueObtained = reader.ReadInt32(),
            };
            return val;
        }
    }
}