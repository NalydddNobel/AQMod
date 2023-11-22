using Aequus.Common.Players.Backpacks;
using Aequus.Core;
using Terraria.ModLoader.IO;

namespace Aequus;

public partial class AequusPlayer {
    public override void SaveData(TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this);
        BackpackLoader.SaveBackpacks(tag, this);
    }

    public override void LoadData(TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this);
        BackpackLoader.LoadBackpacks(tag, this);
    }
}