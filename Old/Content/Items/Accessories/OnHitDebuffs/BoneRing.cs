using Aequus.Common;
using Terraria.Localization;

namespace Aequus.Old.Content.Items.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
[LegacyName("BoneHawkRing")]
public class BoneRing : ModItem {
    public static int DebuffDuration { get; set; } = 30;
    public static float MovementSpeedMultiplier { get; set; } = 0.4f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(MovementSpeedMultiplier), ExtendLanguage.Seconds(DebuffDuration));

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = Commons.Rare.BiomeDungeon;
        Item.value = Commons.Cost.BiomeDungeon;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBoneRing++;
    }
}