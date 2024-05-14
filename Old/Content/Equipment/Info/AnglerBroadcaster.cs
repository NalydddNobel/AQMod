using Aequus.Core.CodeGeneration;
using Terraria.Localization;
using Terraria.UI;

namespace Aequus.Old.Content.Equipment.Info;

[PlayerGen.InfoField("accInfoQuestFish")]
public class AnglerBroadcaster : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().accInfoQuestFish = true;
    }
}

public class AnglerBroadcasterIcon : InfoDisplay {
    private int _lastAnglerFish;
    private string _tooltip;

    private LocalizedText _finishedText;
    private LocalizedText _unavailableText;

    public const int ALTERNATING_DISPLAY_TEXT_TIME = 480;

    public override void SetStaticDefaults() {
        _finishedText = this.GetLocalization("Finished");
        _unavailableText = this.GetLocalization("Unavailable");
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        // Display "Unavailable" if the angler is dead, or the angler quest id is mangled.
        if (!NPC.AnyNPCs(NPCID.Angler) || !Main.anglerQuestItemNetIDs.IndexInRange(Main.anglerQuest)) {
            displayColor = InactiveInfoTextColor;
            return _unavailableText.Value;
        }

        // Display "Finished" if the quest is finished.
        if (Main.anglerQuestFinished) {
            displayColor = InactiveInfoTextColor;
            return _finishedText.Value;
        }

        int questFish = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        // Update the tooltip text if the current angler quest is different
        if (_lastAnglerFish != questFish) {
            _lastAnglerFish = questFish;
            UpdateTooltipCache(questFish);
        }

        int maxTime = ALTERNATING_DISPLAY_TEXT_TIME;
        int showItemNameTime = maxTime / 2;

        // Alternate between showing the fish's name and tooltip every 2 seconds.
        // Or just only display the name if the fish has no tooltip.
        if (Main.GameUpdateCount % maxTime < showItemNameTime || string.IsNullOrEmpty(_tooltip)) {
            return Lang.GetItemNameValue(questFish);
        }

        return _tooltip;
    }

    private void UpdateTooltipCache(int questFish) {
        ItemTooltip tooltip = Lang.GetTooltip(questFish);
        // If the quest fish has no tooltip, set the tooltip to empty.
        if (tooltip == null || tooltip.Lines != 1) {
            _tooltip = string.Empty;
            return;
        }

        string tip = tooltip.GetLine(0);

        if (tip.StartsWith('\'')) {
            tip = tip[1..];
        }
        if (tip.EndsWith('\'')) {
            tip = tip[..^1];
        }

        // Save the tooltip, so we dont need to recalculate it every frame.
        _tooltip = tip;
    }

    public override bool Active() {
        return Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoQuestFish;
    }
}
