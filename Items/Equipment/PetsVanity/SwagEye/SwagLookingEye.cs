using Aequus.Common.Items.Dedications;

namespace Aequus.Items.Equipment.PetsVanity.SwagEye;
public class SwagLookingEye : PetItemBase {
    public override int ProjId => ModContent.ProjectileType<TorraPet>();
    public override int BuffId => ModContent.BuffType<TorraBuff>();

    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("torra th", new Color(80, 60, 255, 255)));
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[ItemID.SuspiciousLookingEye] = Type;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = Item.sellPrice(gold: 1);
    }
}