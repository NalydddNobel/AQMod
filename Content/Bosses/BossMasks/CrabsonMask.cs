using Aequus.Common.Items;

namespace Aequus.Content.Bosses.BossMasks;

[AutoloadEquip(EquipType.Head)]
public class CrabsonMask : ModItem {
    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemCommons.Rarity.BossMasks;
        Item.vanity = true;
    }
}