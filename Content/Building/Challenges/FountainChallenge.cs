using Aequus.Common.Building;
using Aequus.Content.Building.Passes;

namespace Aequus.Content.Building.Challenges {
    public class FountainChallenge : BuildChallenge {
        public FindWaterfallPass FindWaterfalls;

        public override void LoadPasses() {
            FindWaterfalls = AddPass(new FindWaterfallPass());
        }

        public override void Scan(ScanResults[] results, ref ScanInfo info) {
            InvokePass(results, ref info, FindWaterfalls);
        }
    }
}