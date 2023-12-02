using Aequus.Common.Items;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Vanity;

[AutoloadEquip(EquipType.Head)]
public class BreadOfCthulhuMask : ContentItem {
    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemCommons.Rarity.BossMasks;
        Item.vanity = true;
    }
}