using System;

namespace Aequus.Common.ContentTemplates.Armor;

internal readonly record struct ArmorStats(EquipType[]? EquipTypes, int Defense, Action<Item, Player>? UpdateEquip = null, ISetbonusProvider? Setbonus = null, object[] TooltipFormatArgs = null);
