using Aequus.Core.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Core;

#pragma warning disable CA2211 // Non-constant fields should not be visible
public class WorldFlags : ModSystem {
    [SaveData("DemonT1")]
    public static bool DownedDemonSiegeT1;

    public override void SaveWorldData(TagCompound tag) {
        tag["DemonT1"] = DownedDemonSiegeT1;
    }

    public override void LoadWorldData(TagCompound tag) {
    }
}
#pragma warning restore CA2211 // Non-constant fields should not be visible