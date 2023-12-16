using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;

public class CrownOfBloodGlobalItem : GlobalItem {
    public int _uuid;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.accessory;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) || aequusPlayer._bloodCrownUUID != _uuid || _uuid == 0) {
            return;
        }

        if (tooltips.TryGetItemName(out var itemName)) {
            itemName.Text = Language.GetTextValue(CrownOfBlood.CategoryKey + ".Affix") + " " + itemName.Text;
        }
        int prefixLine = tooltips.GetPrefixIndex();
        int lineNum = 0;
        foreach (var trackedStat in Boosts.TrackedStats) {
            if (trackedStat.Difference == Boosts.StatDifference.None) {
                continue;
            }

            string text = trackedStat.DifferenceText;
            if (trackedStat.Suffix != null) {
                text += " " + trackedStat.Suffix.Value;
            }
            tooltips.Insert(prefixLine, new TooltipLine(Mod, "PrefixCrownOfBlood" + lineNum++, text) { IsModifier = true, IsModifierBad = trackedStat.Difference == Boosts.StatDifference.Negative, });
        }
        if (lineNum == 0) {
            tooltips.Insert(prefixLine, new TooltipLine(Mod, "PrefixCrownOfBlood0", Language.GetTextValue(CrownOfBlood.CategoryKey + ".StatAffix.None")) { IsModifier = true, IsModifierBad = false, });
        }
    }
}