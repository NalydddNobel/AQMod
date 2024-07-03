using System.IO;
using tModLoaderExtended.Networking;

namespace Aequu2.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class PacketRequestNewAnalysisQuest : PacketHandler {
    public void Send(Player player) {
        ModPacket packet = GetPacket();
        packet.Write(player.whoAmI);
        packet.Write(player.GetModPlayer<AnalysisPlayer>().completed);
        packet.Send();
    }

    public void SendFromServer(Player player, AnalysisQuest newQuest) {
        ModPacket packet = GetPacket();
        packet.Write(player.whoAmI);
        newQuest.NetSend(packet);
        packet.Send(toClient: player.whoAmI);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int player = reader.ReadInt32();
        AnalysisPlayer analysisPlayer = Main.player[player].GetModPlayer<AnalysisPlayer>();

        if (Main.netMode == NetmodeID.Server) {
            int completed = reader.ReadInt32();
            analysisPlayer.RefreshQuest(completed);
            var quest = analysisPlayer.quest;
            if (quest.isValid) {
                SendFromServer(Main.player[player], quest);
            }
            return;
        }

        analysisPlayer.quest = AnalysisQuest.NetRecieve(reader);
        if (player == Main.myPlayer && Physicist.AwaitQuest > 0) {
            ModContent.GetInstance<Physicist>().QuestButtonPressed();
        }
    }
}
