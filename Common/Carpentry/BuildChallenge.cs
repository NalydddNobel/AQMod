using Aequus.Common.Carpentry.Results;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Carpentry {
    public abstract class BuildChallenge : ModType, ILocalizedModType {
        public int Type { get; private set; }
        public StepRequirement[] Steps { get; protected set; }
        public abstract int BuildBuffType { get; }
        public abstract int BountyNPCType { get; }

        public string LocalizationCategory => "Carpenter.BuildChallenge";

        protected virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
        protected virtual LocalizedText Description => this.GetLocalization("Description", () => "");
        protected virtual LocalizedText CompletionMessage => this.GetLocalization("CompletionMessage", () => "");

        private LocalizedText _displayName;
        private LocalizedText _description;
        private LocalizedText _completionMessage;

        public LocalizedText GetDisplayName() => _displayName;
        public LocalizedText GetDescription() => _description;
        public LocalizedText GetCompletionMessage() => _completionMessage;

        public BuildChallenge() {
        }

        public abstract StepRequirement[] LoadSteps();

        protected sealed override void Register() {
            _displayName = DisplayName;
            _description = Description;
            _completionMessage = CompletionMessage;
            Type = BuildChallengeLoader.Register(this);
        }

        public sealed override void SetupContent() {
            SetStaticDefaults();
            Steps = LoadSteps();
        }

        public IStepResults[] Scan(ref HighlightInfo highlightInfo, in ScanInfo info) {
            var result = new IStepResults[Steps.Length];
            Scan(result, ref highlightInfo, in info);
            return result;
        }
        public void Scan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo info) {
            DoScan(results, ref highlightInfo, in info);
        }
        protected abstract void DoScan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo);

        public abstract IEnumerable<Item> GetRewards();

        public virtual void OnCompleteBounty(Player player, NPC npc) {
        }
    }
}