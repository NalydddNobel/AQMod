using System;

namespace NALib.Common;

public sealed class MathCommon {
    public static float abs(int value) {
        return Math.Abs(value);
    }

    public static float abs(float value) {
        return Math.Abs(value);
    }

    public static float sin(float x, float mag) {
        return sin(x, 0f, mag);
    }

    public static float sin(float x, float min, float max) {
        return (float)(min + (Math.Sin(x) + 1f) / 2f * (max - min));
    }
}
