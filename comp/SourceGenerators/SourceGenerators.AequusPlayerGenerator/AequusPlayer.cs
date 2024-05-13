using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus;

public partial class AequusPlayer {
    [CompilerGenerated]
    public bool usedConvergentHeart;
    [CompilerGenerated]
    public bool usedMaxHPRespawnReward;
    [CompilerGenerated]
    public bool usedCosmicChest;
    
    [CompilerGenerated]
    private void SaveInner(TagCompound tag) {
        SaveObj(tag, "usedConvergentHeart", usedConvergentHeart);
        SaveObj(tag, "usedMaxHPRespawnReward", usedMaxHPRespawnReward);
        SaveObj(tag, "usedCosmicChest", usedCosmicChest);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        LoadObj(tag, "usedConvergentHeart", ref usedConvergentHeart);
        LoadObj(tag, "usedMaxHPRespawnReward", ref usedMaxHPRespawnReward);
        LoadObj(tag, "usedCosmicChest", ref usedCosmicChest);
    }
}