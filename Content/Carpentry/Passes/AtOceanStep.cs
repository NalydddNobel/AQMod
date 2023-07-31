using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Terraria;

namespace Aequus.Content.Carpentry.Passes;

public class AtOceanStep : StepRequirement<ScanInfo> {
    public override IScanResults GetStepResults(in ScanInfo info, in NoParameters parameters = default(NoParameters)) {
        StepResultType result = StepResultType.Fail;
        if (WorldGen.oceanDepths(info.X, info.Y) || WorldGen.oceanDepths(info.X + info.Width, info.Y)) {
            result = StepResultType.Success;
        }
        else if (Helper.InOuterX(info.X, info.Y, 5) || Helper.InOuterX(info.X + info.Width, info.Y, 5)) {
            result = StepResultType.Almost;
        }
        return new StepResult(result, this);
    }
}