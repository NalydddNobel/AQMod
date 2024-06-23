using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusPlayer {
    [CompilerGenerated]
    public bool forceUseItem;
    [CompilerGenerated]
    public byte disableItem;
    [CompilerGenerated]
    public StatModifier wingTime;
    [CompilerGenerated]
    public Item goldenKey;
    [CompilerGenerated]
    public Item shadowKey;
    [CompilerGenerated]
    public Item accBreathRestore;
    [CompilerGenerated]
    public int accBreathRestoreStacks;
    [CompilerGenerated]
    public bool accInfoDayCalendar;
    [CompilerGenerated]
    public bool accInfoDebuffDPS;
    [CompilerGenerated]
    public bool accInfoMoneyMonocle;
    [CompilerGenerated]
    public bool accInfoShimmerMonocle;
    [CompilerGenerated]
    public string accGifterRing;
    [CompilerGenerated]
    public Item accWeightedHorseshoe;
    [CompilerGenerated]
    public bool showHorseshoeAnvilRope;
    [CompilerGenerated]
    public int cHorseshoeAnvil;
    [CompilerGenerated]
    public int consumedBeyondLifeCrystals;
    [CompilerGenerated]
    public int consumedBeyondManaCrystals;
    [CompilerGenerated]
    public bool usedConvergentHeart;
    [CompilerGenerated]
    public bool usedMaxHPRespawnReward;
    [CompilerGenerated]
    public bool usedCosmicChest;
    [CompilerGenerated]
    public int potSightRange;
    
    [CompilerGenerated]
    private void SetControlsInner() {
        Common.Items.UseEffects.ForceItemUse(Player, this);
    }
    
    [CompilerGenerated]
    private void ResetEffectsInner() {
        ResetObj(ref wingTime);
        ResetObj(ref goldenKey);
        ResetObj(ref shadowKey);
        ResetObj(ref accBreathRestore);
        ResetObj(ref accBreathRestoreStacks);
        ResetObj(ref accGifterRing);
        ResetObj(ref accWeightedHorseshoe);
        ResetObj(ref showHorseshoeAnvilRope);
        ResetObj(ref cHorseshoeAnvil);
        ResetObj(ref potSightRange);
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
    private void PostUpdateEquipsInner() {
        Content.Items.Accessories.WeightedHorseshoe.WeightedHorseshoe.OnPostUpdateEquips(Player, this);
        Content.Items.PermaPowerups.Shimmer.CosmicChest.OnPostUpdateEquips(this);
    }
    
    [CompilerGenerated]
    private void SaveInner(TagCompound tag) {
        this.SaveObj(tag, "consumedBeyondLifeCrystals", consumedBeyondLifeCrystals);
        this.SaveObj(tag, "consumedBeyondManaCrystals", consumedBeyondManaCrystals);
        this.SaveObj(tag, "usedConvergentHeart", usedConvergentHeart);
        this.SaveObj(tag, "usedMaxHPRespawnReward", usedMaxHPRespawnReward);
        this.SaveObj(tag, "usedCosmicChest", usedCosmicChest);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        this.LoadObj(tag, "consumedBeyondLifeCrystals", ref consumedBeyondLifeCrystals);
        this.LoadObj(tag, "consumedBeyondManaCrystals", ref consumedBeyondManaCrystals);
        this.LoadObj(tag, "usedConvergentHeart", ref usedConvergentHeart);
        this.LoadObj(tag, "usedMaxHPRespawnReward", ref usedMaxHPRespawnReward);
        this.LoadObj(tag, "usedCosmicChest", ref usedCosmicChest);
    }
    
    [CompilerGenerated]
    private void OnRespawnInner() {
        Content.Items.PermaPowerups.NoHit.NoHitReward.DoPermanentMaxHPRespawn(Player, this);
    }
}