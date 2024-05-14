using Aequus.Common;
using Aequus.Common.Backpacks;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.ScavengerBag;

[LegacyName("AmmoBackpack")]
[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 15;
    public override float SlotHue { get; set; } = 0.2f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.rare = Commons.Rare.PollutedOceanLoot;
        Item.value = Commons.Cost.PollutedOceanLoot;
    }
}