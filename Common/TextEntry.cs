using Terraria.Localization;

namespace Aequus.Common {
    public record struct TextEntry(string Key) {
        /// <returns>A <see cref="LocalizedText"/> value for this entry. Caching is recommended.</returns>
        public LocalizedText GetText() {
            return Language.GetText(Key);
        }
        /// <returns><see cref="LocalizedText.Value"/> for this entry. Caching is recommended.</returns>
        public string GetTextValue() {
            return Language.GetTextValue(Key);
        }

        public static implicit operator TextEntry(string Key) {
            return new TextEntry(Key);
        }
        public static implicit operator LocalizedText(TextEntry textEntry) {
            return textEntry.GetText();
        }
        public static implicit operator string(TextEntry textEntry) {
            return textEntry.GetTextValue();
        }
    }
}