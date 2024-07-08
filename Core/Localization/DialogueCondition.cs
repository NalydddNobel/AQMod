using Terraria.Localization;

namespace AequusRemake.Core.Localization;

public class DialogueCondition(Condition Condition) : IDialogueCondition {
    bool IDialogueCondition.IsMet(LocalizedText text, string name) {
        return Condition.IsMet();
    }
}
