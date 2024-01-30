using Aequus.Common.Items;

namespace Aequus.Content.Bosses;

[AutoloadEquip(EquipType.Head)]
internal class InstancedBossMask : InstancedModItem {
    public InstancedBossMask(System.String name) : base($"{name}Mask", $"{typeof(InstancedBossMask).NamespaceFilePath()}/BossMasks/{name}Mask") {
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
    }
}