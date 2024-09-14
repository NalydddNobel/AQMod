using Aequus.Systems.Backpacks;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.Backpacks;

#if POLLUTED_OCEAN
[LegacyName("AmmoBackpack")]
#endif
[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 15;
    public override float SlotHue { get; set; } = 0.2f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.CloneShopValues(ItemID.Flipper);
    }
}