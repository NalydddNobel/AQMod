using System.Runtime.CompilerServices;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostInfo {
    public readonly System.Int32 Slot;
    public EquipBoostType Boost;

    public System.Boolean HasAnyBoost => Boost > 0;
    public System.Boolean HasAbilityBoost => HasFlag(EquipBoostType.Abilities);
    public System.Boolean HasDefenseBoost => HasFlag(EquipBoostType.Defense);

    public EquipBoostInfo(System.Int32 slot) {
        Slot = slot;
    }

    public void ResetEffects() {
        Boost = EquipBoostType.None;
    }

    public void ApplyModifier(Item equipItem, Player player, AequusPlayer aequus, System.Boolean hideVisual = false) {
        equipItem.GetGlobalItem<EquipBoostGlobalItem>().equipEmpowerment = this;
        if (HasAbilityBoost) {
            System.Int32 defense = equipItem.defense;
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
    public System.Boolean HasFlag(EquipBoostType flag) {
        return Boost.HasFlag(flag);
    }
}