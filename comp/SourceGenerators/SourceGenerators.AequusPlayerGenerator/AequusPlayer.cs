using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;

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
    
    [CompilerGenerated]
    private void OnRespawnInner() {
        Content.Items.PermaPowerups.NoHit.NoHitReward.DoPermanentMaxHPRespawn(Player, this);
    }
}/*
PlayerGen.Field;
bool;
PlayerGen.Field;
byte;
PlayerGen.ResetField;
StatModifier;
PlayerGen.ResetField;
Item;
PlayerGen.ResetField;
Item;
PlayerGen.ResetField;
Item;
PlayerGen.ResetField;
int;
PlayerGen.InfoField;
PlayerGen.InfoField;
PlayerGen.InfoField;
PlayerGen.InfoField;
PlayerGen.ResetField;
string;
PlayerGen.ResetField;
Item;
PlayerGen.ResetField;
bool;
PlayerGen.ResetField;
int;
PlayerGen.SavedField;
bool;
PlayerGen.SavedField;
bool;
PlayerGen.SavedField;
bool;
PlayerGen.ResetField;
int;
*/
