using AequusRemake.Core;
using AequusRemake.Content.Backpacks;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.ScavengerBag;

[LegacyName("AmmoBackpack")]
[AutoloadEquip(EquipType.Back)]
public class ScavengerBag : BackpackModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Capacity);

    public override int Capacity { get; set; } = 15;
    public override float SlotHue { get; set; } = 0.2f;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.rare = Commons.Rare.BiomeOcean;
        Item.value = Commons.Cost.BiomeOcean;
    }
}