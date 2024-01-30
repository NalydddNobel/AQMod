using Aequus.Common.Items.Components;
using Aequus.Old.Content.Equipment.Accessories.SpiritKeg;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.Accessories.HighSteaks;

[LegacyName("SteakEyes")]
[AutoloadEquip(EquipType.Waist)]
public class HighSteaks : ModItem, IHaveDownsideTip {
    /// <summary>
    /// Default Value: 0.15
    /// <para>This is only added on the first stack of the accessory.</para>
    /// </summary>
    public static float NotStackableCritDamage { get; set; } = 0.15f;
    public static float StackableCritDamage { get; set; } = 0.1f;

    public static int ExpertModeCost { get; set; } = Item.silver * 3;
    public static int ClassicModeCost { get; set; } = Item.silver;

    /// <summary>
    /// The cost for performing a critical strike.
    /// </summary>
    public static int WantedCost => Main.expertMode ? ExpertModeCost : ClassicModeCost;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(NotStackableCritDamage + StackableCritDamage), this.GetLocalization("Downside"));

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SaivoryKnife>();
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(16, 16);
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var highSteaksPlayer = player.GetModPlayer<HighSteaksPlayer>();
        highSteaksPlayer.highSteaksHidden = hideVisual;
        highSteaksPlayer.highSteaksDamage = Math.Max(highSteaksPlayer.highSteaksDamage, NotStackableCritDamage) + StackableCritDamage;
        if (highSteaksPlayer.highSteaksCost > 0) {
            highSteaksPlayer.highSteaksCost = Math.Max(highSteaksPlayer.highSteaksCost / 2, 1);
        }
        else {
            highSteaksPlayer.highSteaksCost = ExpertModeCost;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        const string REPLACE = "{Price}";
        string replaceValue = ExtendLanguage.PriceTextColored(WantedCost, AlphaPulse: true);

        foreach (var t in tooltips.Where(t => t.Name.StartsWith("Tooltip") && t.Text.Contains(REPLACE))) {
            t.Text = t.Text.Replace(REPLACE, replaceValue);
        }
    }
}
