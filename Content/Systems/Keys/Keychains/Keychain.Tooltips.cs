using Aequus.Common.Items.Tooltips;
using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Helpers;
using System.Collections.Generic;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.Localization;

namespace Aequus.Content.Systems.Keys.Keychains;

public partial class Keychain : IAddSpecialTooltips {
    void IAddSpecialTooltips.AddSpecialTooltips(List<SpecialAbilityTooltipInfo> tooltips) {
        KeychainPlayer keychain = Main.LocalPlayer.GetModPlayer<KeychainPlayer>();
        if (keychain.sortedKeysForIcons == null || keychain.sortedKeysForIcons.Count == 0) {
            return;
        }

        SpecialAbilityTooltipInfo keyword = new(Language.GetTextValue("Mods.Aequus.Misc.ContainsHeader"), Color.Gold, ItemID.GoldenKey);
        foreach (Item item in keychain.sortedKeysForIcons) {
            int stack = item.stack;
            item.stack = 1;
            Color color = item.rare == ItemRarityID.White ? Color.White : ItemRarity.GetColor(item.rare);
            string text = $"{ItemTagHandler.GenerateTag(item)}{ColorTagProvider.Get(color, item.Name)}";
            item.stack = stack;

            if (item.stack > 1) {
                text += $" ({item.stack})";
            }

            keyword.AddLine(text);
        }

        tooltips.Add(keyword);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        string smartSelectButton = PlayerInput.GenerateInputTag_ForCurrentGamemode_WithHacks(tagForGameplay: false, "SmartSelect");
        if (!smartSelectButton.Contains('[')) {
            smartSelectButton = ALanguage.PrettyPrint(smartSelectButton);
        }

        foreach (TooltipLine line in tooltips) {
            if (line.Name.StartsWith("Tooltip")) {
                line.Text = line.Text.Replace("<shift>", smartSelectButton);
            }
        }
    }
}
