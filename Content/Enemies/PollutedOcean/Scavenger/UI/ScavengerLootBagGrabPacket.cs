using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger.UI;

public class ScavengerLootBagGrabPacket : PacketHandler {
    public void Send(System.Int32 talkNPC, System.Int32 player, System.Int32 slot) {
        var p = GetPacket();
        p.Write(talkNPC);
        p.Write(player);
        p.Write(slot);
        p.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 talkNPC = reader.ReadInt32();
        System.Int32 player = reader.ReadInt32();
        System.Int32 slot = reader.ReadInt32();
        if (!Main.npc.IndexInRange(talkNPC) || !Main.player.IndexInRange(player) || Main.npc[talkNPC].ModNPC is not ScavengerLootBag lootBag || !lootBag.drops.IndexInRange(slot)) {
            return;
        }

        if (Main.myPlayer == player) {
            Main.player[player].GetItem(player, lootBag.drops[slot].Clone(), GetItemSettings.NPCEntityToPlayerInventorySettings);
            Recipe.FindRecipes();
        }
        lootBag.drops[slot].TurnToAir();

        if (Main.netMode == NetmodeID.Server) {
            Send(talkNPC, player, slot);
        }
    }
}