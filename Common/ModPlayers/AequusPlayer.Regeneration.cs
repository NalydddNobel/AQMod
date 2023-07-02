using Aequus.Buffs.Debuffs;
using System;

namespace Aequus {
    partial class AequusPlayer {
        /// <summary>
        /// An amount of regen to add to the player in <see cref="UpdateLifeRegen"/>
        /// </summary>
        public int regenerationFlat;
        public float regenerationMultiplier;

        public int regenerationNaturalFlat;
        public float regenerationNaturalMultiplier;

        public int regenerationBadFlat;
        public float regenerationBadMultiplier;

        public override void NaturalLifeRegen(ref float regen) {
            regen *= regenerationNaturalMultiplier;
            regen += regenerationNaturalFlat;

            regenerationNaturalMultiplier = 1f;
            regenerationNaturalFlat = 0;
        }

        public override void UpdateLifeRegen() {
            if (Player.lifeRegen >= 0) {
                Player.lifeRegen = (int)(Player.lifeRegen * regenerationMultiplier);
                Player.lifeRegen += regenerationFlat;
            }
            else {
                Player.lifeRegen = (int)(Player.lifeRegen * regenerationBadMultiplier);
                Player.lifeRegen -= regenerationBadFlat;
            }

            regenerationBadMultiplier = 1f;
            regenerationBadFlat = 0;
            regenerationMultiplier = 1f;
            regenerationFlat = 0;
        }

        private void AddBadRegen(int amountTimesTwo) {
            Player.lifeRegen = Math.Min(Player.lifeRegen, 0);
            Player.lifeRegenTime = Math.Min(Player.lifeRegenTime, 0); // Natural life regeneration timer
            Player.lifeRegen -= amountTimesTwo;
        }

        public override void UpdateBadLifeRegen() {
            if (Player.HasBuff<BlueFire>()) {
                AddBadRegen(-16);
            }
            if (Player.HasBuff<CrimsonHellfire>()) {
                AddBadRegen(-16);
            }
            if (Player.HasBuff<CorruptionHellfire>()) {
                AddBadRegen(-16);
            }
            UpdateBadLifeRegen_Vampire();
        }
    }
}