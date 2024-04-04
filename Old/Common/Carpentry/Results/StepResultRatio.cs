namespace Aequus.Old.Common.Carpentry.Results {
    public record struct StepResultRatio : IScanResults {
        public readonly int Value;
        public readonly int Max;

        public float Ratio => Value / (float)Max;
        public StepResultType ResultType { get; set; }

        public StepResultRatio(int Value, int Max) {
            this.Value = Value;
            this.Max = Max;
            ResultType = Value >= Max ? StepResultType.Success : Value > 0 ? StepResultType.Almost : StepResultType.Fail;
        }

        public string GetResultText() {
            return Value + "/" + Max;
        }
    }
}
