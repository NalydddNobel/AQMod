using Aequus.Common.Backpacks;
using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.ScavengerBag;

[LegacyName("AmmoBackpack")]
[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override System.Int32 Capacity { get; set; } = 15;
    public override System.Single SlotHue { get; set; } = 0.2f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }
}