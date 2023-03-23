using Aequus.Common.Net;
using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus {
    public partial class AequusWorld {

        private bool _initDay;
        private bool dayTime;
        private int checkNPC;

        public void CheckDayInit() {
            if (_initDay) {
                dayTime = Main.dayTime;
                _initDay = false;
                checkNPC = Math.Max(checkNPC, 0);
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    InitDay_ResetAverageHappiness();
                }
            }

            if (checkNPC >= 0 && Main.netMode != NetmodeID.MultiplayerClient) {

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

    public class DayNightInitPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.DayNightInit;

        public void Write(BinaryWriter writer) {
            writer.Write(AequusWorld.AverageHappiness);
        }

        public void Send() {
            var p = GetPacket();
            Write(p);
            p.Send();
        }

        public override void Receive(BinaryReader reader) {
            AequusWorld.AverageHappiness = reader.ReadSingle();
        }
    }
}