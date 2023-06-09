using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Building {
    public abstract class BuildChallenge : ModType {
        public int Type { get; private set; }
        public readonly List<StepRequirement> Steps;

        public BuildChallenge() {
            Steps = new();
        }

        protected sealed override void Register() {
            Type = LoaderManager.Get<BuildChallengeLoader>().Register(this);
        }

        protected void AddStep<T>() where T : StepRequirement {
            Steps.Add(ModContent.GetInstance<T>());
        }

        public sealed override void SetupContent() {
            SetStaticDefaults();
        }

        public IStepResults[] Scan(ref HighlightInfo highlightInfo, in ScanInfo info) {
            var result = new IStepResults[Steps.Count];
            Scan(result, ref highlightInfo, in info);
            return result;
        }
        public void Scan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo info) {
            DoScan(results, ref highlightInfo, in info);
        }
        protected abstract void DoScan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo);
    }
}