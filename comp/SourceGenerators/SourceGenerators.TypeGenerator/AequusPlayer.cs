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
    public Item accHyperCrystal;
    [CompilerGenerated]
    public int hyperCrystalCooldownMax;
    [CompilerGenerated]
    public int cHyperCrystal;
    [CompilerGenerated]
    public bool accInfoQuestFish;
    [CompilerGenerated]
    public float buffNeutronYogurt;
    [CompilerGenerated]
    public bool minionScribble;
    [CompilerGenerated]
    public bool minionStarite;
    [CompilerGenerated]
    public int ghostChains;
    [CompilerGenerated]
    public float zombieDebuffMultiplier;
    [CompilerGenerated]
    public int ghostProjExtraUpdates;
    [CompilerGenerated]
    public bool accRitualSkull;
    [CompilerGenerated]
    public int ghostShadowDash;
    [CompilerGenerated]
    public NPCAnchor gravetenderGhost;
    [CompilerGenerated]
    public int ghostSlots;
    [CompilerGenerated]
    public int ghostSlotsOld;
    [CompilerGenerated]
    public int ghostSlotsMax;
    [CompilerGenerated]
    public StatModifier ghostLifespan;
    
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
        ResetObj(ref accHyperCrystal);
        ResetObj(ref hyperCrystalCooldownMax);
        ResetObj(ref cHyperCrystal);
        ResetObj(ref buffNeutronYogurt);
        ResetObj(ref ghostChains);
        ResetObj(ref zombieDebuffMultiplier);
        ResetObj(ref ghostProjExtraUpdates);
        ResetObj(ref accRitualSkull);
        ResetObj(ref ghostShadowDash);
        ResetObj(ref gravetenderGhost);
        ResetObj(ref ghostSlotsMax);
        ResetObj(ref ghostLifespan);
        Old.Content.Necromancy.NecromancySystem.OnResetEffects(this);
    }
    
    [CompilerGenerated]
    private void ResetInfoAccessoriesInner() {
        ResetObj(ref accInfoDayCalendar);
        ResetObj(ref accInfoDebuffDPS);
        ResetObj(ref accInfoMoneyMonocle);
        ResetObj(ref accInfoShimmerMonocle);
        ResetObj(ref accInfoQuestFish);
    }
    
    [CompilerGenerated]
    private void MatchInfoAccessoriesInner(AequusPlayer other) {
        accInfoDayCalendar |= other.accInfoDayCalendar;
        accInfoDebuffDPS |= other.accInfoDebuffDPS;
        accInfoMoneyMonocle |= other.accInfoMoneyMonocle;
        accInfoShimmerMonocle |= other.accInfoShimmerMonocle;
        accInfoQuestFish |= other.accInfoQuestFish;
    }
    
    [CompilerGenerated]
    private void PostUpdateEquipsInner() {
        Content.Items.Accessories.WeightedHorseshoe.WeightedHorseshoe.OnPostUpdateEquips(Player, this);
        Content.Items.PermaPowerups.Shimmer.CosmicChest.OnPostUpdateEquips(this);
        Old.Content.Items.Potions.NeutronYogurt.NeutronYogurt.UpdateNeutronYogurt(Player, this);
        Old.Content.Necromancy.Equipment.Accessories.RitualisticSkull.OnPostUpdateEquips(Player, this);
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
}