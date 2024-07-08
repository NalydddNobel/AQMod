using Terraria.Localization;

namespace AequusRemake.Core.Localization;

public class RareCondition() : IDialogueCondition {
    bool IDialogueCondition.IsMet(LocalizedText text, string name) {
        return Main.rand.NextBool(50);
    }
}
