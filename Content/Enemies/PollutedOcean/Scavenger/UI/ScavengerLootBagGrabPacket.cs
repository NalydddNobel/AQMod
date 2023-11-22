using Aequus.Core.Networking;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger.UI;

public class ScavengerLootBagGrabPacket : PacketHandler {
    public void Send(int talkNPC, int player, int slot) {
        var p = GetPacket();
        p.Write(talkNPC);
        p.Write(player);
        p.Write(slot);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int talkNPC = reader.ReadInt32();
        int player = reader.ReadInt32();
        int slot = reader.ReadInt32();
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