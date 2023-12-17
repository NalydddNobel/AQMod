namespace Aequus.Common.Players.Stats;

public enum StatDifference {
    None,
    Negative,
    Positive
}

public static class DifferenceMethods {
    public static string GetSign(this StatDifference diff, string positive = "+", string negative = "") {
        return diff == StatDifference.Positive ? positive : negative;
    }

    public static StatDifference CompareFloat(float after, float before) {
        return after == before ? StatDifference.None : after < before ? StatDifference.Negative : StatDifference.Positive;
    }
}