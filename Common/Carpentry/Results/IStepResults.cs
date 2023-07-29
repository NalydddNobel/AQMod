namespace Aequus.Common.Carpentry.Results {
    public interface IStepResults {
        StepResultType ResultType { get; set; }
        string GetResultText();
    }
}