using Aequus.Old.Common.Carpentry.Results;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Old.Common.Carpentry;
public abstract class BuildChallenge : ModTexturedType, ILocalizedModType {
    public int Type { get; private set; }
    public StepRequirement[] Steps { get; protected set; }
    public abstract int BountyNPCType { get; }

    public string LocalizationCategory => "Carpenter.BuildChallenge";

    protected virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
    protected virtual LocalizedText Description => this.GetLocalization("Description", () => "");
    protected virtual LocalizedText CompletionMessage => this.GetLocalization("CompletionMessage", () => "");

    private LocalizedText _displayName;
    private LocalizedText _description;
    private LocalizedText _completionMessage;

    public ModBuff BountyBuff { get; private set; }

    public LocalizedText GetDisplayName() {
        return _displayName;
    }

    public LocalizedText GetDescription() {
        return _description;
    }

    public LocalizedText GetCompletionMessage() {
        return _completionMessage;
    }

    public BuildChallenge() { }

    public abstract StepRequirement[] LoadSteps();

    protected sealed override void Register() {
        _displayName = DisplayName;
        _description = Description;
        _completionMessage = CompletionMessage;
        Type = BuildChallengeLoader.Register(this);
    }

    public sealed override void Load() {
        BountyBuff = new InstancedBuilderBuff(this);
        Mod.AddContent(BountyBuff);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
        Steps = LoadSteps();
    }

    public IScanResults[] Scan(ref HighlightInfo highlightInfo, in ScanInfo info) {
        var result = new IScanResults[Steps.Length];
        PopulateScanResults(result, ref highlightInfo, in info);
        return result;
    }

    public abstract void PopulateScanResults(IScanResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo);

    public string[] GetStepDescriptions() {
        var descriptions = new string[Steps.Length];
        PopulateStepDescriptions(descriptions);
        return descriptions;
    }

    public abstract void PopulateStepDescriptions(string[] descriptions);

    public abstract IEnumerable<Item> GetRewards();

    public virtual void OnCompleteBounty(Player player, NPC npc) {
        foreach (var reward in GetRewards()) {
            player.QuickSpawnItem(npc.GetSource_GiftOrReward(), reward, reward.stack);
        }
    }

    public virtual bool IsAvailable() {
        int npcType = BountyNPCType;
        if (npcType > 0 && !NPC.AnyNPCs(npcType)) {
            return false;
        }
        return true;
    }

    public abstract void UpdateBuff(Player player, ref int buffIndex);
}