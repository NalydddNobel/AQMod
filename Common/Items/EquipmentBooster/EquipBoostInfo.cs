using System.Runtime.CompilerServices;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostInfo {
    public readonly int Slot;
    public EquipBoostType Boost;

    public bool HasAnyBoost => Boost > 0;
    public bool HasAbilityBoost => HasFlag(EquipBoostType.Abilities);
    public bool HasDefenseBoost => HasFlag(EquipBoostType.Defense);

    public EquipBoostInfo(int slot) {
        Slot = slot;
    }

    public void ResetEffects() {
        Boost = EquipBoostType.None;
    }

    public void ApplyModifier(Item equipItem, Player player, AequusPlayer aequus, bool hideVisual = false) {
        equipItem.GetGlobalItem<EquipBoostGlobalItem>().equipEmpowerment = this;
        if (HasAbilityBoost) {
            int defense = equipItem.defense;
            if (EquipBoostDatabase.Instance.SpecialUpdate.TryGetValue(equipItem.type, out var value)) {
                value(equipItem, player, hideVisual);
            }
            else {
                equipItem.defense = 0;
                try {
                    player.GrantArmorBenefits(equipItem);
                }
                catch {
                }
                equipItem.defense = defense;
                player.ApplyEquipFunctional(equipItem, false);
            }

            if (equipItem.wingSlot != -1) {
                player.wingTimeMax *= 2;
            }
        }
        if (HasDefenseBoost) {
            player.statDefense += equipItem.defense;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasFlag(EquipBoostType flag) {
        return Boost.HasFlag(flag);
    }
}