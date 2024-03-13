using Aequus.Core.IO;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public struct AnalysisQuest {
    public const string SAVEKEY_RARITY_NAME = "r";
    public const string SAVEKEY_RARITY_HIGHEST_VALUE = "v";

    public int itemRarity;
    public int itemValue;
    public bool isValid;

    public void SaveData(TagCompound tag) {
        if (isValid) {
            IDCommons.SaveRarityToTag(tag, SAVEKEY_RARITY_NAME, itemRarity);
            tag[SAVEKEY_RARITY_HIGHEST_VALUE] = itemValue;
        }
    }

    public static AnalysisQuest LoadData(TagCompound tag) {
        AnalysisQuest newQuest = new AnalysisQuest();
        if (IDCommons.LoadRarityFromTag(tag, SAVEKEY_RARITY_NAME, out int rare)) {
            newQuest.itemRarity = rare;
            newQuest.isValid = true;
        }
        if (newQuest.isValid) {
            newQuest.itemValue = tag.Get<int>(SAVEKEY_RARITY_HIGHEST_VALUE);
        }
        return newQuest;
    }

    public void NetSend(BinaryWriter writer) {
        writer.Write(isValid);
        if (isValid) {
            writer.Write(itemRarity);
            writer.Write(itemValue);
        }
    }

    public static AnalysisQuest NetRecieve(BinaryReader reader) {
        var info = new AnalysisQuest();
        if (reader.ReadBoolean()) {
            info.isValid = true;
            info.itemRarity = reader.ReadInt32();
            info.itemValue = reader.ReadInt32();
        }
        return info;
    }
}