using System;

namespace Aequus.Common.ContentTemplates.Armor;

internal record struct ArmorStats(EquipType[]? EquipTypes, int Defense = 0, Action<Item, Player>? UpdateEquip = null, ISetbonusProvider? Setbonus = null, object[] TooltipFormatArgs = null, bool Vanity = false, int Rare = 0, int Price = 0) {
    public int RobeSlot;
}
