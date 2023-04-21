using Aequus.Common.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;

namespace Aequus.Common.ModPlayers {

    public class EquipEmpowermentManager {

        public static Color CrownOfBloodEmpowermentColor = new(255, 128, 140, 255);
        public static Color BasicEmpowermentColor = new(140, 255, 128, 255);

        private EquipEmpowerment[] vanilla;

        public EquipEmpowermentManager() {
            vanilla = new EquipEmpowerment[Player.SupportedSlotSets];
            for (int i = 0; i < Player.SupportedSlotSets; i++) {
                vanilla[i] = new(i);
            }
        }

        public void ResetEffects() {
            for (int i = 0; i < vanilla.Length; i++) {
                vanilla[i].ResetEffects();
            }
        }

        public ref EquipEmpowerment Head() {
            return ref vanilla[0];
        }
        public ref EquipEmpowerment Body() {
            return ref vanilla[1];
        }
        public ref EquipEmpowerment Legs() {
            return ref vanilla[2];
        }
        public ref EquipEmpowerment FirstUnempoweredAccessory(EquipEmpowermentParameters parameters) {
            for (int slot = Player.SupportedSlotsArmor; slot < Player.SupportedSlotSets; slot++) {
                if ((vanilla[slot].type & parameters) == parameters) {
                    continue;
                }

                return ref vanilla[slot];
            }
            return ref vanilla[3];
        }
        public ref EquipEmpowerment GetVanilla(int slot) {
            return ref vanilla[slot];
        }
    }

    public class EquipEmpowerment {

        public readonly int Slot;

        public int addedStacks;
        public EquipEmpowermentParameters type;
        public Color? textColor;

        public EquipEmpowerment(int slot) {
            Slot = slot;
        }

        public void ResetEffects() {
            addedStacks = 0;
            type = EquipEmpowermentParameters.None;
            textColor = null;
        }

        public void ApplyModifier(Item equipItem, Player player, AequusPlayer aequus, bool hideVisual = false) {

            equipItem.Aequus().equipEmpowerment = this;
            if (type.HasFlag(EquipEmpowermentParameters.Abilities)) {

                int defense = equipItem.defense;
                equipItem.defense = 0;
                try {
                    player.GrantArmorBenefits(equipItem);
                }
                catch {
                }
                equipItem.defense = defense;
                player.ApplyEquipFunctional(equipItem, false);

                if (equipItem.wingSlot != -1) {
                    player.wingTimeMax *= 2;
                }
            }
            if (type.HasFlag(EquipEmpowermentParameters.Defense)) {

                player.statDefense += equipItem.defense;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasFlag(EquipEmpowermentParameters flag) {
            return type.HasFlag(flag);
        }
    }

    [Flags]
    public enum EquipEmpowermentParameters : byte {
        None = 0,
        Defense = 1,
        Abilities = 2,
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public EquipEmpowermentManager equipModifiers;

        private void Initalize_EquipModifiers() {
            equipModifiers ??= new();
        }

        private void ResetEffects_EquipModifiers() {
            (equipModifiers ??= new()).ResetEffects();
        }

        private void UpdateEquips_UpdateEmpoweredAccessories() {
            for (int i = Player.SupportedSlotsArmor; i < Player.SupportedSlotSets; i++) {

                var accessoryItem = Player.armor[i];
                if (accessoryItem.IsAir)
                    continue;

                var modifier = equipModifiers.GetVanilla(i);
                if (modifier.type == EquipEmpowermentParameters.None) {

                    continue;
                }

                modifier.ApplyModifier(accessoryItem, Player, this, hideVisual: Player.hideVisibleAccessory[i]);
            }
        }
        private void PostUpdateEquips_UpdateEmpoweredArmors() {
            for (int i = 0; i < Player.SupportedSlotsArmor; i++) {

                var armorItem = Player.armor[2];
                if (armorItem.IsAir)
                    continue;

                var modifier = equipModifiers.GetVanilla(i);
                if (modifier.type == EquipEmpowermentParameters.None) {
                    armorItem.Aequus().equipEmpowerment = null;
                    continue;
                }

                modifier.ApplyModifier(armorItem, Player, this, hideVisual: false);
            }
        }

        private void PostUpdateEquips_EmpoweredEquipAbilities() {

            if (Player.boneGloveItem != null && Player.boneGloveTimer > 1) {

                var empowerment = Player.boneGloveItem.Aequus().equipEmpowerment;
                if (empowerment != null) {
                    Player.boneGloveTimer -= -1;
                    if (Player.boneGloveTimer < 1) {
                        Player.boneGloveTimer = 1;
                    }
                }
            }
        }
    }
}