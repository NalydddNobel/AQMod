using Terraria.GameContent.UI.Chat;

namespace AequusRemake.Core.Util;

/// <summary>Contains helper methods for generating chat tag strings. For item and glyphs, <see cref="ItemTagHandler"/> and <see cref="GlyphTagHandler"/> have static helper methods.</summary>
public sealed class ColorTagProvider {
    public const char TagStartChar = '[';
    public const char TagEndChar = ']';
    public static readonly string ColorTagFormat = $$"""{{TagStartChar}}c/{0}:{1}{{TagEndChar}}""";

    public static string Color(Color color, string text) {
        return Color(color.Hex3(), text);
    }
    public static string Color(Vector3 color, string text) {
        return Color(new Color(color), text);
    }
    public static string Color(string color, string text) {
        return string.Format(ColorTagFormat, color, text);
    }
}
