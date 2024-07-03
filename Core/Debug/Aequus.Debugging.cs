using System.Diagnostics;

namespace Aequu2;

partial class Aequu2 {
    internal const bool DEBUG_MODE =
#if DEBUG
            true;
#else
        false;
#endif

    internal static string DEBUG_FILES_PATH => $"{Main.SavePath.Replace("tModLoader-preview", "tModLoader")}";

    [Conditional("DEBUG")]
    public static void DebugDustLine(Vector2 start, Vector2 end, int amt, int dustType = 6) {
        var dustTravel = (start - end) / amt;
        for (int i = 0; i < amt; i++) {
            DebugDust(start - dustTravel * i, dustType);
        }
    }

    public static void DebugDustColor(Vector2 where, Color color) {
        var dust = Dust.NewDustPerfect(where, DustID.TintableDustLighted);
        if (dust != null) {
            dust.color = color;
        }
    }

    [Conditional("DEBUG")]
    public static void DebugDust(Vector2 where, int dustType = DustID.Torch) {
        Dust.NewDustPerfect(where, dustType);
    }

    [Conditional("DEBUG")]
    public static void DebugDustRectangle(Point where, int dustType = DustID.Torch) {
        DebugDustRectangle(where.X, where.Y, dustType);
    }

    [Conditional("DEBUG")]
    public static void DebugDustRectangle(int x, int y, int dustType = DustID.Torch) {
        var rect = new Rectangle(x * 16, y * 16, 16, 16);
        for (int i = 0; i < 4; i++) {
            i *= 4;
            var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            i /= 4;
        }
    }

    [Conditional("DEBUG")]
    public static void DebugDustRectangle(Rectangle rect, int dustType = DustID.Torch) {
        int amt = rect.Width / 2;
        for (int i = 0; i < amt; i++) {
            i *= 2;
            var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            i /= 2;
        }
        amt = rect.Height / 2;
        for (int i = 0; i < amt; i++) {
            i *= 2;
            var d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            i /= 2;
        }
    }
}
