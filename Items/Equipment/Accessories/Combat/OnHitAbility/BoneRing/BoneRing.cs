using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;

[AutoloadEquip(EquipType.HandsOn)]
[LegacyName("BoneHawkRing")]
public class BoneRing : ModItem, IBoneRing {
    public int DebuffDuration => 30;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Create.PercentDifference(BoneRingDebuff.MovementSpeedMultiplier), DebuffDuration / 60f);

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(base.Tooltip.WithFormatArgs(
            TextHelper.Create.PercentDifference(BoneRingDebuff.MovementSpeedMultiplier),
            DebuffDuration * 2 / 60f)
        ));
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.Aequus().accBoneRing.SetAccessory(Item, this);
    }
}