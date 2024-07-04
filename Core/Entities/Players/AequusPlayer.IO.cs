using AequusRemake.Core.IO;
using Terraria.ModLoader.IO;

namespace AequusRemake;

public partial class AequusPlayer {
    public override void SaveData(TagCompound tag) {
        SaveInner(tag);
        SaveDataAttribute.SaveData(tag, this);
    }

    public override void LoadData(TagCompound tag) {
        LoadInner(tag);
        SaveDataAttribute.LoadData(tag, this);
    }
}