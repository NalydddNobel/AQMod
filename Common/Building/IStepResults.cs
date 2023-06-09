namespace Aequus.Common.Building {
    public interface IStepResults {
        ResultType ResultType { get; set; }
        string GetResultText();
    }
}