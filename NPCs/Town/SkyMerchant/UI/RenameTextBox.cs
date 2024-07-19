using Aequus.Common.GUI.Elements;
using Aequus.Systems.Renaming;

namespace Aequus.NPCs.Town.SkyMerchant.UI;

public class RenameTextBox(string text, float textScale = 1, bool large = false) : ImprovedTextBox(text, textScale, large) {
    public override void ModifyDisplayText() {
        _displayText = RenamingSystem.GetColoredDecodedText(_text, pulse: true);
    }
}
