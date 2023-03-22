using Terraria;
using Terraria.ID;

namespace Aequus {
    public partial class AequusWorld {

        /// <summary>
        /// Higher values mean less happy town NPCs, and lower values mean happier town NPCs.
        /// <para>This is because the calculated happiness value is actually the price reduction, which is lower on happier NPCs.</para>
        /// </summary>
        public static float AverageHappiness { get; private set; }

        private float _totalHappiness;
        private int _totalNPCs;

        private void InitDay_ResetAverageHappiness() {
            _totalHappiness = 0f;
            _totalNPCs = 0;
        }

        private void CalcAverageHappiness(NPC npc) {

            if (!npc.townNPC || npc.type == NPCID.OldMan || npc.type == NPCID.TravellingMerchant || NPCID.Sets.ActsLikeTownNPC[npc.type]) {
                return;
            }
            int i = 0;
            for (; i < Main.maxPlayers; i++) {
                if (Main.player[i].active) {
                    ScanBiomesToPlayer(Main.player[i], out var _, npc.Center);
                    break;
                }
            }

            if (i == Main.maxPlayers) {
                return;
            }

            var shopSettings = Main.ShopHelper.GetShoppingSettings(Main.player[i], npc);
            var priceAdjustment = shopSettings.PriceAdjustment;
            _totalHappiness += (float)priceAdjustment;
            _totalNPCs++;
            Main.NewText($"Report for {npc.whoAmI} ({npc.FullName}): [{_totalNPCs}, {_totalHappiness}] , {priceAdjustment}");
            Main.NewText($"{shopSettings.HappinessReport}", Microsoft.Xna.Framework.Color.Beige);
        }

        private void FinalizeHappinessCalculation() {
            if (_totalNPCs == 0) {
                AverageHappiness = 0f;
                return;
            }

            AverageHappiness = _totalHappiness / _totalNPCs;
            Main.NewText($"Happiness result is: {AverageHappiness}");
        }
    }
}