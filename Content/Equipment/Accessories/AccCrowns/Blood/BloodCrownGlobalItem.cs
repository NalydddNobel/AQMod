using Aequus.Common.Players.Stats;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

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
            AddTooltip(text, isModifierBad: trackedStat.Difference == StatDifference.Negative);
        }
        if (lineNum == 0) {
            AddTooltip(Language.GetTextValue(BloodCrown.CategoryKey + ".StatAffix.None"));
        }

        void AddTooltip(string text, bool isModifierBad = false) {
            // Add padding for symbol.
            text += TextHelper.AirCharacter;

            tooltips.Insert(prefixLine, new TooltipLine(Mod, "PrefixBloodCrown" + lineNum++, text) { IsModifier = true, IsModifierBad = isModifierBad, });
        }
    }

    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
        if (line.Name.StartsWith("PrefixBloodCrown")) {
            var lineMeasurement = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
            Main.spriteBatch.Draw(AequusTextures.BloodSymbol, new Vector2(line.X + lineMeasurement.X - 6f, line.Y + lineMeasurement.Y / 2f - 2f), null, Colors.AlphaDarken(Color.White) * 0.75f, 0f, AequusTextures.BloodSymbol.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }
        return true;
    }
}