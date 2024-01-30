using Terraria.Localization;

namespace Aequus.Common.Items.EquipmentBooster;

public struct EquipBoostEntry {
    public delegate System.Boolean CustomUpdateMethod(Player player, Item item);

    public System.Boolean Invalid => Tooltip == null;

    public LocalizedText Tooltip { get; internal set; }
    public readonly CustomUpdateMethod CustomEquipUpdate;

    public EquipBoostEntry(LocalizedText tooltip, CustomUpdateMethod customUpdateMethod = null) {
        Tooltip = tooltip;
        CustomEquipUpdate = customUpdateMethod;
    }
}