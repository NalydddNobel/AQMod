using System;

namespace Aequus.Old.Common.Carpentry.Results {
    public record struct StepResultRatioPercent : IScanResults {
        public readonly int Value;
        public readonly int Max;

        public float Ratio => Value / (float)Max;
        public StepResultType ResultType { get; set; }

        public StepResultRatioPercent(int Value, int Max) {
            this.Value = Value;
            this.Max = Max;
            ResultType = Value >= Max ? StepResultType.Success : Value > 0 ? StepResultType.Almost : StepResultType.Fail;
        }

        public string GetResultText() {
            return string.Format(((int)(Math.Min(Ratio, 1f) * 100f)).ToString() + "%");
        }
    }
}
