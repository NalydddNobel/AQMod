using Terraria.ModLoader.IO;

namespace Aequus.Core;

public class World : ModSystem {
    [SaveData("DemonT1")]
    public static bool DownedDemonSiegeT1;

    public override void SaveWorldData(TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this);
    }

    public override void LoadWorldData(TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this);
    }
}
