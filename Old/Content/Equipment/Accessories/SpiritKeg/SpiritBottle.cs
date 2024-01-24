using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.Accessories.SpiritKeg;

public class SpiritBottle : ModItem {
    public static int IncreaseGhostSlots { get; set; } = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IncreaseGhostSlots);

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PygmyNecklace);
        Item.neckSlot = 0;
        Item.hasVanityEffects = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.maxTurrets += IncreaseGhostSlots;
    }
}