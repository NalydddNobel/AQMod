namespace Aequus.Common.Utilities.Helpers;

public class MathTools {
    /// <returns><paramref name="numerator"/> divided by <paramref name="denominator"/>. If anything is leftover, the value is increased by 1.</returns>
    public static int DivCeiling(int numerator, int denominator) {
        int result = numerator / denominator;

        if (numerator % denominator > 0) {
            result++;
        }

        return result;
    }
}
