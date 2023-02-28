using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Content.AnalysisQuests
{
    public struct QuestInfo
    {
        public int itemRarity;
        public int itemValue;
        public bool isValid;

        public void SaveData(TagCompound tag)
        {
            if (isValid)
            {
                Helper.SaveRarity(tag, "RarityName", "Rarity", itemRarity);
                tag["Value"] = itemValue;
            }
        }

        public static QuestInfo LoadData(TagCompound tag)
        {
            var info = new QuestInfo();
            if (Helper.LoadRarity(tag, "RarityName", "Rarity", out int rare))
            {
                info.itemRarity = rare;
                info.isValid = true;
            }
            if (info.isValid)
            {
                info.itemValue = tag.Get<int>("Value");
            }
            return info;
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.Write(isValid);
            if (isValid)
            {
                writer.Write(itemRarity);
                writer.Write(itemValue);
            }
        }

        public static QuestInfo NetRecieve(BinaryReader reader)
        {
            var info = new QuestInfo();
            if (reader.ReadBoolean())
            {
                info.isValid = true;
                info.itemRarity = reader.ReadInt32();
                info.itemValue = reader.ReadInt32();
            }
            return info;
        }
    }
}