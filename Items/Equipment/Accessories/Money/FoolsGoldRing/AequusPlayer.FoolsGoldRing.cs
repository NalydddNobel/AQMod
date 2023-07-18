using Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;
using Terraria;
using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusPlayer {
        /// <summary>
        /// Set by <see cref="FoolsGoldRing"/>
        /// </summary>
        public int accFoolsGoldRing;
        public int accFoolsGoldRingCharge;
        public float increasedEnemyMoney;
        public int showFoolsGoldRingChargeDuration;

        public void ResetEffects_FoolsGoldRing() {
            accFoolsGoldRing = 0;
            increasedEnemyMoney = 0f;

            if (showFoolsGoldRingChargeDuration > 0)
                showFoolsGoldRingChargeDuration--;
        }

        public void PostUpdate_FoolsGoldRing() {
            if (accFoolsGoldRingCharge > Item.gold * FoolsGoldRing.GoldCoinsRequired) {
                Player.AddBuff(ModContent.BuffType<FoolsGoldRingBuff>(), 4);
            }
            if (accFoolsGoldRing <= 0) {
                accFoolsGoldRingCharge = 0;
                showFoolsGoldRingChargeDuration = 0;
            }
        }
    }
}