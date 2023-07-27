namespace Aequus.Common.Building.Results {
    public interface IStepResults {
        StepResultType ResultType { get; set; }
        string GetResultText();
    }
}