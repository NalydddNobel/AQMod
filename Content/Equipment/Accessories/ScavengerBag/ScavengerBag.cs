using Aequus.Common.Backpacks;
using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.ScavengerBag;

[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 15;
    public override float SlotHue { get; set; } = 0.2f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
    }
}