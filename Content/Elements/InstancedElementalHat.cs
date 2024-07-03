using Aequu2.Core;
using Aequu2.Content.Elements;
using Aequu2.Core.ContentGeneration;
using Terraria.Localization;

namespace Aequu2.Content.Elements;

[AutoloadEquip(EquipType.Head)]
internal class InstancedElementalHat(Element parent) : InstancedModItem($"{parent.Name}Mask", $"{parent.Texture}Mask") {
    public override LocalizedText DisplayName => parent.GetLocalization("Mask.DisplayName", () => $"{parent.Name} Mask");
    public override LocalizedText Tooltip => parent.GetLocalization("Mask.Tooltip", () => "");

    public override void SetStaticDefaults() {
        HelmetEquipSets.IsTallHat[Item.headSlot] = true;
        parent.AddItem(Type);
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.defense = 10;
        Item.rare = Commons.Rare.BossDustDevil;
        Item.value = Commons.Cost.BossDustDevil;
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<ElementalPlayer>().visibleElements.Add(parent);
    }
}
