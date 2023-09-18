using System;

namespace Aequus;
public static class Helper {
    public static float Oscillate(float time, float magnitude) {
        return Oscillate(time, magnitude, magnitude);
    }

    public static float Oscillate(float time, float minimum, float maximum) {
        return (float)(minimum + (Math.Sin(time) + 1f) / 2f * (maximum - minimum));
    }
}
