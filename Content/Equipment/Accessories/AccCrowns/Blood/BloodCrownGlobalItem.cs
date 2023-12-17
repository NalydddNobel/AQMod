using Aequus.Common.Players.Stats;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;

public class BloodCrownGlobalItem : GlobalItem {
    public int _localUUID;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.accessory;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) || BloodCrown.TooltipUUID != _localUUID || _localUUID == 0) {
            return;
        }

        if (tooltips.TryGetItemName(out var itemName)) {
            itemName.Text = Language.GetTextValue(BloodCrown.CategoryKey + ".Affix") + " " + itemName.Text;
        }
        int prefixLine = tooltips.GetPrefixIndex();
        int lineNum = 0;
        foreach (var trackedStat in BloodCrown.StatComparer.Comparisons) {
            if (trackedStat.Difference == StatDifference.None) {
                continue;
            }

            string text = trackedStat.DifferenceText;
            if (trackedStat.Suffix != null) {
                text += " " + trackedStat.Suffix.Value;
            }
            tooltips.Insert(prefixLine, new TooltipLine(Mod, "PrefixBloodCrown" + lineNum++, text) { IsModifier = true, IsModifierBad = trackedStat.Difference == StatDifference.Negative, });
        }
        if (lineNum == 0) {
            tooltips.Insert(prefixLine, new TooltipLine(Mod, "PrefixBloodCrown0", Language.GetTextValue(BloodCrown.CategoryKey + ".StatAffix.None")) { IsModifier = true, IsModifierBad = false, });
        }
    }
}