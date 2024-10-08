using Aequus.Common.Net;
using System.Text;

namespace Aequus.Content.Systems.Commands;

public class PacketCountCommand : ModCommand {
    public override CommandType Type => CommandType.World;

    public override string Command => "aequuspkt";

    public override string Description => "Checks specific packet type counts.";

    public override void Action(CommandCaller caller, string input, string[] args) {
        StringBuilder b = new();

        for (int i = 0; i < (int)PacketType.Count; i++) {
            string name = ((PacketType)i).ToString();

            b.AppendLine($"{name} - Sent {PacketSystem._sent[i]} / Received {PacketSystem._received[i]}");
        }

        caller.Reply(b.ToString());
    }
}
