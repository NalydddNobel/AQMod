using Aequus.Common.Items.EquipmentBooster;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.Accessories.WarHorn;

public class WarHorn : ModItem {
    public static int FrenzyTime { get; set; } = 240;
    public static int CooldownTime { get; set; } = 480;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FrenzyTime / 60, CooldownTime / 60);

    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accWarHorn++;
    }
}