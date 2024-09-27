using Aequus.Common.ContentTemplates.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Common.ContentTemplates.Armor;

internal class InstancedArmorPiece(UnifiedArmorSet Parent, string Suffix, ArmorStats Stats) : InstancedModItem(Parent.Name + Suffix, Parent.Texture + Suffix) {
    public override string LocalizationCategory => $"{Parent.LocalizationCategory}.{Parent.Name}";

    public override LocalizedText Tooltip => Stats.TooltipFormatArgs == null ? base.Tooltip : base.Tooltip.WithFormatArgs(Stats.TooltipFormatArgs);

    public override void OnCreated(ItemCreationContext context) {
        // We need to use this for equip textures. Since Type is not reserved during Load.
        if (context is InitializationItemCreationContext) {
            if (Stats.EquipTypes != null) {
                bool first = true;
                foreach (EquipType equip in Stats.EquipTypes) {
                    AddSlot(equip, first);
                    first = false;
                }
            }
        }

        int AddSlot(EquipType equip, bool first) {
            string texture = $"{Texture}_{equip}";

            if (first) {
                return EquipLoader.AddEquipTexture(Mod, texture, equip, this);
            }

            int slot = EquipLoader.AddEquipTexture(Mod, texture, equip, name: Name);

            if (equip == EquipType.Legs) {
                Stats.RobeSlot = slot;
            }

            return slot;
        }
    }

    public override void SetDefaults() {
        Item.width = 18;
        Item.height = 18;
        Item.defense = Stats.Defense;
        Item.rare = Stats.Rare;
        Item.value = Stats.Price;
        Item.vanity = Stats.Vanity;
    }

    public override void UpdateEquip(Player player) {
        Stats.UpdateEquip?.Invoke(Item, player);
    }

    public override bool IsArmorSet(Item head, Item body, Item legs) {
        return Stats.Setbonus?.IsArmorSet(head, body, legs) ?? false;
    }

    public override void UpdateArmorSet(Player player) {
        player.setBonus = Stats.Setbonus!.SetbonusText!.Value;
        Stats.Setbonus.UpdateArmorSet(Item, player);
    }

    public override void SetMatch(bool male, ref int equipSlot, ref bool robes) {
        if (Stats.RobeSlot != 0) {
            robes = true;
            equipSlot = Stats.RobeSlot;
        }
    }
}
