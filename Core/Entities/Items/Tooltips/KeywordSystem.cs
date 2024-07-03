using Aequu2.Content.Backpacks;
using Aequu2.Core.Entities.Items.Components;
using System.Collections.Generic;

namespace Aequu2.Core.Entities.Items.Tooltips;
public class KeywordSystem : ModSystem {
    public static int HoveredItemID { get; internal set; }
    public static int LastHoveredItemID { get; internal set; }

    public static readonly List<Keyword> Tooltips = new();

    public override void Unload() {
        Tooltips.Clear();
    }

    public static void AddItemKeywords(Item item) {
        if (item.ModItem is IAddKeywords addSpecialTooltips) {
            addSpecialTooltips.AddSpecialTooltips();
        }
        BackpackSlotsUI.AddBackpackWarningTip(item);
    }

    //private void AddCrownOfBloodTooltip(Item item) {
    //    SpecialAbilityTooltipInfo tooltip = new(TextHelper.GetItemName<CrownOfBloodItem>().Value, Color.PaleVioletRed, ModContent.ItemType<CrownOfBloodItem>());
    //    if (item.defense > 0) {
    //        tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.Defense", item.defense * 2));
    //    }

    //    if (item.wingSlot > -1) {
    //        tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.Wings"));
    //    }

    //    if (EquipBoostDatabase.Instance.Entries.IndexInRange(item.type) && !EquipBoostDatabase.Instance.Entries[item.type].Invalid) {
    //        tooltip.AddLine(EquipBoostDatabase.Instance.Entries[item.type].Tooltip.Value);
    //    }

    //    if (tooltip.tooltipLines.Count == 0) {
    //        tooltip.AddLine(TextHelper.GetTextValue("Items.BoostTooltips.UnknownEffect"));
    //    }
    //    _tooltips.Add(tooltip);
    //}

    public static void AddPlayerSpecificKeywords(Player player, Item item) {
        var Aequu2Player = player.GetModPlayer<AequusPlayer>();
        //if (Aequu2Player.accCrownOfBlood != null && item.ModItem is not CrownOfBloodItem && item.accessory && !item.vanity && item.createTile != TileID.MusicBoxes) {
        //    AddCrownOfBloodTooltip(item);
        //}
        //if (Aequu2Player.accSentryInheritence != null && item.accessory && !item.vanity && item.createTile != TileID.MusicBoxes) {
        //    SpecialAbilityTooltipInfo tooltip = new(Aequu2Player.accSentryInheritence.Name, Color.LawnGreen, Aequu2Player.accSentryInheritence.type);
        //    tooltip.AddLine("Sentries will summon Spores around them to damage enemies");
        //    _tooltips.Add(tooltip);
        //}
    }

    public override void UpdateUI(GameTime gameTime) {
        LastHoveredItemID = HoveredItemID;
        HoveredItemID = ItemID.None;
    }
}
