using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Summon.WarHorn;

public class WarHorn : ModItem {
    public static int FrenzyTime = 240;
    public static int CooldownTime = 600;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FrenzyTime / 60, CooldownTime / 60);

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(this.GetLocalization("BoostTooltip").WithFormatArgs(FrenzyTime * 2 / 60)));
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.Aequus().accWarHorn++;
    }
}