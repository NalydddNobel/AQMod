namespace Aequus.Common.Building {
    public record struct StepResultRatio : IStepResults {
        public readonly int Value;
        public readonly int Max;

        public float Ratio => Value / (float)Max;
        public ResultType ResultType { get; set; }

        public StepResultRatio(int Value, int Max) {
            this.Value = Value;
            this.Max = Max;
            ResultType = Value >= Max ? ResultType.Success : Value > 0 ? ResultType.Almost : ResultType.Fail;
        }

        public string GetResultText() {
            return Value + "/" + Max;
        }
    }
}
