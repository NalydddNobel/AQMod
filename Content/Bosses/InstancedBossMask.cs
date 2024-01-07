using Aequus.Common.Items;
using Aequus.Common.NPCs;

namespace Aequus.Content.Bosses;

[AutoloadEquip(EquipType.Head)]
internal class InstancedBossMask : InstancedModItem {
    public InstancedBossMask(AequusBoss boss) : base($"{boss.Name}Mask", boss.ItemPath("Mask")) { }
    public InstancedBossMask(string name) : base($"{name}Mask", $"{typeof(InstancedBossMask).NamespaceFilePath()}/BossMasks/{name}Mask") { }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
    }
}