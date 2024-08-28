using Aequus.Common.ContentTemplates.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Common.ContentTemplates.Armor;

internal class InstancedArmorPiece(UnifiedArmorSet Parent, string Suffix, ArmorStats Stats) : InstancedModItem(Parent.Name + Suffix, Parent.Texture + Suffix) {
    public override string LocalizationCategory => $"{Parent.LocalizationCategory}.{Parent.Name}";

    public override LocalizedText Tooltip => Stats.TooltipFormatArgs == null ? base.Tooltip : base.Tooltip.WithFormatArgs(Stats.TooltipFormatArgs);

    //public override void Load() {
    //}

    public override void OnCreated(ItemCreationContext context) {
        // We need to use this for equip textures. Since Type is not reserved during Load.
        if (context is InitializationItemCreationContext) {
            if (Stats.EquipTypes != null) {
                foreach (var equip in Stats.EquipTypes) {
                    EquipLoader.AddEquipTexture(Mod, $"{Texture}_{equip}", equip, this);
                }
            }
        }
    }

    public override void SetDefaults() {
        Item.width = 18;
        Item.height = 18;
        Item.defense = Stats.Defense;
    }

    public override void UpdateEquip(Player player) {
        Stats.UpdateEquip?.Invoke(Item, player);
    }

    public override bool IsArmorSet(Item head, Item body, Item legs) {
        return Stats.Setbonus?.IsArmorSet(head, body, legs) ?? false;
    }

    public override void UpdateArmorSet(Player player) {
        player.setBonus = Stats.Setbonus!.SetbonusText.Value;
        Stats.Setbonus.UpdateArmorSet(Item, player);
    }
}
