using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Bosses;

[AutoloadEquip(EquipType.Head)]
internal class InstancedBossMask : InstancedModItem {
    public InstancedBossMask(ModNPC modNPC) : base($"{modNPC.Name}Mask", $"{modNPC.NamespaceFilePath()}/Items/{modNPC.Name}Mask") {
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
    }
}