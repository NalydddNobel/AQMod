using AequusRemake.Content.Systems.Renaming;
using AequusRemake.Core.UI.Elements;

namespace AequusRemake.Content.TownNPCs.SkyMerchant.UI;

public class RenameTextBox : ImprovedTextBox {
    public RenameTextBox(string text, float textScale = 1, bool large = false) : base(text, textScale, large) {
    }

    public override void ModifyDisplayText() {
        _displayText = RenamingSystem.GetColoredDecodedText(_text, pulse: true);
    }
}
