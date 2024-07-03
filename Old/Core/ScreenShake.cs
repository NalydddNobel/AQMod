namespace Aequu2.Old.Core;

public class ScreenShake {
    public static void SetShake(float intensity, float multiplyPerTick, Vector2 where = default) {
        ViewHelper.LegacyScreenShake(intensity, multiplyPerTick, where);
    }
    public static void SetShake(float intensity, Vector2 where = default) {
        SetShake(intensity, 0.9f, where);
    }
}
