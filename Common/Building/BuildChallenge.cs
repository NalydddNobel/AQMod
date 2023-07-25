using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Building {
    public abstract class BuildChallenge : ModType {
        public int Type { get; private set; }
        public StepRequirement[] Steps { get; protected set; }
        public abstract int BuildBuffType { get; }
        public abstract int BountyNPCType { get; }

        public BuildChallenge() {
        }

        public abstract StepRequirement[] LoadSteps();

        protected sealed override void Register() {
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
    }
}