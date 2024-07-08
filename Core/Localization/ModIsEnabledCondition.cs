using Terraria.Localization;

namespace AequusRemake.Core.Localization;

public class ModIsEnabledCondition() : IDialogueCondition {
    bool IDialogueCondition.IsMet(LocalizedText text, string name) {
        return ModLoader.HasMod(name);
    }
}
