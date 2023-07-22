using Aequus.NPCs;
using Aequus.NPCs.BossMonsters.Crabson;
using System.IO;
using Terraria.ID;

namespace Aequus.Common.Net;

public partial class PacketSystem {
    public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber) {
        //if (messageType == MessageID.PlayerHurtV2) {
        //    Crabson.Laugh();
        //}
        return false;
    }
}