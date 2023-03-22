using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Aequus {
    partial class AequusWorld {

        private bool _initDay;
        private bool dayTime;
        private int checkNPC;

        public void CheckDayInit() {
            if (_initDay) {
                dayTime = Main.dayTime;
                _initDay = false;
                checkNPC = Math.Max(checkNPC, 0);
                InitDay_ResetAverageHappiness();
            }

            if (checkNPC >= 0) {

                if (Main.npc[checkNPC].active) {
                    CalcAverageHappiness(Main.npc[checkNPC]);
                }
                checkNPC++;
                if (checkNPC >= Main.maxNPCs) {
                    checkNPC = -1;
                    FinalizeHappinessCalculation();
                }
            }

            // Force initialization on day/night swap 
            if (dayTime != Main.dayTime) {
                dayTime = Main.dayTime;
                _initDay = true;
            }
        }
    }
}