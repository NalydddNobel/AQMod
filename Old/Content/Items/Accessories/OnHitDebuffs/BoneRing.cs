using Aequu2.Core;
using Terraria.Localization;

namespace Aequu2.Old.Content.Items.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
[LegacyName("BoneHawkRing")]
public class BoneRing : ModItem {
    public static int DebuffDuration { get; set; } = 30;
    public static float MovementSpeedMultiplier { get; set; } = 0.4f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Percent(MovementSpeedMultiplier), XLanguage.Seconds(DebuffDuration));

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = Commons.Rare.BiomeDungeon;
        Item.value = Commons.Cost.BiomeDungeon;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().accBoneRing++;
    }
}