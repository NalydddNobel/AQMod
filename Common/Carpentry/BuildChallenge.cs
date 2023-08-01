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
    }
}