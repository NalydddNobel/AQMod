namespace Aequus.Common.Carpentry.Results {
    public interface IScanResults {
        StepResultType ResultType { get; set; }
        string GetResultText();
    }
}