using Terraria.GameContent.UI.Chat;

namespace Aequus.Core.Utilities;

/// <summary>Contains helper methods for generating chat tag strings. For item and glyphs, <see cref="ItemTagHandler"/> and <see cref="GlyphTagHandler"/> have static helper methods.</summary>
public class ChatTagWriter {
    public const char TAG_START = '[';
    public const char TAG_END = ']';
    public static readonly string COLOR_TAG = $$"""{{TAG_START}}c/{0}:{1}{{TAG_END}}""";

    public static string Color(Color color, string text) {
        return Color(color.Hex3(), text);
    }
    public static string Color(Vector3 color, string text) {
        return Color(new Color(color), text);
    }
    public static string Color(string color, string text) {
        return string.Format(COLOR_TAG, color, text);
    }
}
