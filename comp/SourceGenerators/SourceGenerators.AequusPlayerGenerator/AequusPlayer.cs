using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus;

public partial class AequusPlayer {
    [CompilerGenerated]
    public Item accBreathRestore;
    [CompilerGenerated]
    public int accBreathRestoreStacks;
    [CompilerGenerated]
    public string accGifterRing;
    [CompilerGenerated]
    public Item accWeightedHorseshoe;
    [CompilerGenerated]
    public bool showHorseshoeAnvilRope;
    [CompilerGenerated]
    public int cHorseshoeAnvil;
    [CompilerGenerated]
    public bool accInfoDayCalendar;
    [CompilerGenerated]
    public bool accInfoDebuffDPS;
    [CompilerGenerated]
    public bool accInfoMoneyMonocle;
    [CompilerGenerated]
    public bool accInfoShimmerMonocle;
    [CompilerGenerated]
    public bool usedConvergentHeart;
    [CompilerGenerated]
    public bool usedMaxHPRespawnReward;
    [CompilerGenerated]
    public bool usedCosmicChest;
    
    [CompilerGenerated]
    private void ResetEffectsInner() {
        ResetObj(ref accBreathRestore);
        ResetObj(ref accBreathRestoreStacks);
        ResetObj(ref accGifterRing);
        ResetObj(ref accWeightedHorseshoe);
        ResetObj(ref showHorseshoeAnvilRope);
        ResetObj(ref cHorseshoeAnvil);
    }
    
    [CompilerGenerated]
    private void ResetInfoAccessoriesInner() {
        ResetObj(ref accInfoDayCalendar);
        ResetObj(ref accInfoDebuffDPS);
        ResetObj(ref accInfoMoneyMonocle);
        ResetObj(ref accInfoShimmerMonocle);
    }
    
    [CompilerGenerated]
    private void MatchInfoAccessoriesInner(AequusPlayer other) {
        accInfoDayCalendar |= other.accInfoDayCalendar;
        accInfoDebuffDPS |= other.accInfoDebuffDPS;
        accInfoMoneyMonocle |= other.accInfoMoneyMonocle;
        accInfoShimmerMonocle |= other.accInfoShimmerMonocle;
    }
    
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