using Terraria.ModLoader;

namespace Aequus.Common.Carpentry.Results {
    public record struct StepResult : IStepResults {
        public StepResultType ResultType { get; set; }
        public readonly string SuffixAdd;
        public readonly ILocalizedModType LocalizedType;

        public StepResult(StepResultType resultType, ILocalizedModType step, string suffixAdd = "") {
            ResultType = resultType;
            LocalizedType = step;
            SuffixAdd = string.IsNullOrEmpty(suffixAdd) ? "" : suffixAdd + ".";
        }

        public string GetResultText() {
            return LocalizedType.GetLocalizedValue($"{SuffixAdd}{ResultType}");
        }
    }
}
