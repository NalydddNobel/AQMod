using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items.EquipmentBooster; 

public struct EquipBoostEntry {
    public delegate bool CustomUpdateMethod(Player player, Item item);

    public bool Invalid => Tooltip == null;

    public LocalizedText Tooltip { get; internal set; }
    public readonly CustomUpdateMethod CustomEquipUpdate;

    public EquipBoostEntry(LocalizedText tooltip, CustomUpdateMethod customUpdateMethod = null) {
        Tooltip = tooltip;
        CustomEquipUpdate = customUpdateMethod;
    }

    public void Apply(Item item, List<TooltipLine> tooltips) {

    }
}