using Aequus.Content.Events.DemonSiege;
using Terraria.Localization;

namespace Aequus.Content.Necromancy.Accessories.SpiritKeg;

[LegacyName("Malediction", "CartilageRing", "NaturesCruelty")]
public class BottleOSpirits : ModItem {
    public static int GhostSlotIncrease { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(GhostSlotIncrease);

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().ghostSlotsMax++;
    }
}