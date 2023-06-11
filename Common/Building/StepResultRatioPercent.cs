using System;

namespace Aequus.Common.Building {
    public record struct StepResultRatioPercent : IStepResults {
        public readonly int Value;
        public readonly int Max;

        public float Ratio => Value / (float)Max;
        public ResultType ResultType { get; set; }

        public StepResultRatioPercent(int Value, int Max) {
            this.Value = Value;
            this.Max = Max;
            ResultType = Value >= Max ? ResultType.Success : Value > 0 ? ResultType.Almost : ResultType.Fail;
        }

        public string GetResultText() {
            return string.Format(((int)(Math.Min(Ratio, 1f) * 100f)).ToString() + "%");
        }
    }
}
