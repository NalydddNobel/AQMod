using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusPlayer {
    [CompilerGenerated]
    public Item accBreathRestore;
    [CompilerGenerated]
    public int accBreathRestoreStacks;
    [CompilerGenerated]
    public Item accStariteExpert;
    [CompilerGenerated]
    public global::Aequus.Content.Items.Accessories.CelesteTorus.CelesteTorusDrawData stariteExpertDrawData;
    [CompilerGenerated]
    public int stariteExpertStacks;
    [CompilerGenerated]
    public int stariteExpertDye;
    [CompilerGenerated]
    public int accDamageOnKill;
    [CompilerGenerated]
    public int buffDamageOnKill;
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
    public Item accGoldenFeather;
    [CompilerGenerated]
    public int accGoldenFeatherRespawnTimeModifier;
    [CompilerGenerated]
    public byte accGoldenFeatherTeammate;
    [CompilerGenerated]
    public bool setMeadow;
    [CompilerGenerated]
    public bool setMeadowMagicTag;
    [CompilerGenerated]
    public Item equipDartTrapHat;
    [CompilerGenerated]
    public bool infiniteWormhole;
    
    [CompilerGenerated]
    private void ResetEffectsInner() {
        SourceGeneratorTools.ResetObj(ref accBreathRestore);
        SourceGeneratorTools.ResetObj(ref accBreathRestoreStacks);
        SourceGeneratorTools.ResetObj(ref accStariteExpert);
        SourceGeneratorTools.ResetObj(ref stariteExpertDrawData);
        SourceGeneratorTools.ResetObj(ref stariteExpertStacks);
        SourceGeneratorTools.ResetObj(ref stariteExpertDye);
        SourceGeneratorTools.ResetObj(ref accDamageOnKill);
        Content.Items.Accessories.DamageOnKillClaw.DamageOnKillClaw.CheckBuffStacks(Player, this);
        SourceGeneratorTools.ResetObj(ref accWeightedHorseshoe);
        SourceGeneratorTools.ResetObj(ref showHorseshoeAnvilRope);
        SourceGeneratorTools.ResetObj(ref cHorseshoeAnvil);
        SourceGeneratorTools.ResetObj(ref accGoldenFeather);
        SourceGeneratorTools.ResetObj(ref accGoldenFeatherRespawnTimeModifier);
        SourceGeneratorTools.ResetObj(ref accGoldenFeatherTeammate);
        SourceGeneratorTools.ResetObj(ref setMeadow);
        SourceGeneratorTools.ResetObj(ref setMeadowMagicTag);
        SourceGeneratorTools.ResetObj(ref equipDartTrapHat);
        SourceGeneratorTools.ResetObj(ref infiniteWormhole);
    }
    
    [CompilerGenerated]
    private void OnBreakTileInner(int X, int Y) {
        Content.Items.Accessories.Breath.BreathConserver.RestoreBreathOnBrokenTile(Player, this, X, Y);
    }
    
    [CompilerGenerated]
    private void OnKillNPCInner(EnemyKillInfo info) {
        Content.Items.Accessories.Breath.BreathConserver.RestoreBreathOnKillNPC(Player, this, info);
        Content.Items.Accessories.DamageOnKillClaw.DamageOnKillClaw.OnKillNPC(Player, this, info);
    }
    
    [CompilerGenerated]
    private void OnHitNPCInner(NPC target, NPC.HitInfo hit) {
        Content.Items.Accessories.DamageOnKillClaw.DamageOnKillClaw.OnHitNPC(Player, this, target, hit);
        Content.Items.Armor.Meadow.MeadowArmor.OnHitNPC(Player, this, target, hit);
        Content.Items.GrapplingHooks.Meathook.Meathook.CheckMeathookSound(Player, this, target, hit);
    }
    
    [CompilerGenerated]
    private void PostUpdateEquipsInner() {
        Content.Items.Accessories.FallSpeedHorseshoe.WeightedHorseshoe.OnPostUpdateEquips(Player, this);
        Content.Items.Accessories.RespawnFeather.GoldenFeather.UpdateGoldenFeather(Player, this);
    }
    
    [CompilerGenerated]
    private void ResetInfoAccessoriesInner() {
        SourceGeneratorTools.ResetObj(ref accInfoDayCalendar);
        SourceGeneratorTools.ResetObj(ref accInfoDebuffDPS);
        SourceGeneratorTools.ResetObj(ref accInfoMoneyMonocle);
        SourceGeneratorTools.ResetObj(ref accInfoShimmerMonocle);
    }
    
    [CompilerGenerated]
    private void MatchInfoAccessoriesInner(AequusPlayer other) {
        accInfoDayCalendar |= other.accInfoDayCalendar;
        accInfoDebuffDPS |= other.accInfoDebuffDPS;
        accInfoMoneyMonocle |= other.accInfoMoneyMonocle;
        accInfoShimmerMonocle |= other.accInfoShimmerMonocle;
    }
    
    [CompilerGenerated]
    private void ModifyHitNPCWithProjInner(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
        Content.Items.Armor.Meadow.MeadowArmor.ModifyHitNPCWithProj(Player, this, proj, target, ref modifiers);
    }
    
    [CompilerGenerated]
    private void ModifyHitNPCInner(NPC target, ref NPC.HitModifiers modifiers) {
        Content.Items.GrapplingHooks.Meathook.Meathook.CheckMeathookDamage(target, ref modifiers);
    }
}