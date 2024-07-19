using Terraria.GameContent.UI.Chat;

namespace Aequus.Common.Utilities.Helpers;

/// <summary>Contains helper methods for generating chat tag strings. For item and glyphs, <see cref="ItemTagHandler"/> and <see cref="GlyphTagHandler"/> have static helper methods.</summary>
public sealed class ColorTagProvider {
    public const char TagStartChar = '[';
    public const char TagEndChar = ']';
    public static readonly string ColorTagFormat = $$"""{{TagStartChar}}c/{0}:{1}{{TagEndChar}}""";

    public static string Get(Color color, string text) {
        return Get(color.Hex3(), text);
    }
    public static string Get(Vector3 color, string text) {
        return Get(new Color(color), text);
    }
    public static string Get(string color, string text) {
        return string.Format(ColorTagFormat, color, text);
    }
}
