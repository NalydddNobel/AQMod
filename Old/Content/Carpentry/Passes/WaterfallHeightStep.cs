using Aequus.Common.Carpentry.Results;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Building.Carpentry.Results;

namespace Aequus.Old.Content.Carpentry.Passes {
    public class WaterfallHeightStep : StepRequirement<ScanInfo, WaterfallHeightStep.Parameters> {
        public record struct Parameters(int DesiredHeight, ScanMap<bool> ScanMap) : IStepRequirementParameters {
        }

        public override IScanResults GetStepResults(in ScanInfo info, in Parameters parameters = default(Parameters)) {
            int maxY;
            int minY;
            int j;
            for (j = 0; j < info.Height; j++) {
                for (int i = 0; i < info.Width; i++) {
                    if (parameters.ScanMap[i, j]) {
                        minY = j;
                        maxY = j;
                        j++;
                        for (; j < info.Height; j++) {
                            for (i = 0; i < info.Width; i++) {
                                if (parameters.ScanMap[i, j]) {
                                    maxY = j;
                                    break;
                                }
                            }
                        }
                        return new StepResultRatio(maxY - minY, parameters.DesiredHeight);
                    }
                }
            }
            return new StepResultRatio(0, parameters.DesiredHeight);
        }
    }
}