using Terraria.Localization;

namespace AequusRemake.Core.Localization;

public interface IDialogueCondition {
    bool IsMet(LocalizedText text, string name);
}
