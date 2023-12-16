using Aequus.Common.Items;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Vanity;

[AutoloadEquip(EquipType.Head)]
public class BreadOfCthulhuMask : ModItem {
    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemDefaults.Rarity.BossMasks;
        Item.vanity = true;
    }
}