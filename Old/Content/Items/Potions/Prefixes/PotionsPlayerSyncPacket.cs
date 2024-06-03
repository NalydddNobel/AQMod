using System.Collections.Generic;
using System.IO;
using tModLoaderExtended.Networking;

namespace Aequus.Old.Content.Items.Potions.Prefixes;

public class PotionsPlayerSyncPacket : PacketHandler {
    private readonly List<int> _potionData = new();

    public void Send(Player player, PotionsPlayer potionPlayer, int toWho = -1, int fromWho = -1) {
        ModPacket packet = GetPacket();

        packet.Write((byte)player.whoAmI);

        lock (potionPlayer) {
            packet.Write(potionPlayer.empoweredPotionId);

            int count = potionPlayer.BoundedPotionIds.Count;

            // We pray that Player.MaxBuffs is the same between clients.
            if (Player.MaxBuffs < byte.MaxValue) {
                packet.Write((byte)count);
            }
            else {
                packet.Write(count);
            }

            for (int i = 0; i < count; i++) {
                packet.Write(potionPlayer.BoundedPotionIds[i]);
            }
        }

        packet.Send(toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte plr = reader.ReadByte();

        int empoweredPotionId = reader.ReadInt32();

        // Second praying session
        int count;
        if (Player.MaxBuffs < byte.MaxValue) {
            count = reader.ReadByte();
        }
        else {
            count = reader.ReadInt32();
        }

        // Add all data to this potion data list.
        // If the below conditions don't fall through, we don't want to lose our position in the packet.
        // So we read the data prematurely.
        _potionData.Clear();
        _potionData.EnsureCapacity(count);

        for (int i = 0; i < count; i++) {
            _potionData.Add(reader.ReadInt32());
        }

        if (!Main.player[plr].active || !Main.player[plr].TryGetModPlayer(out PotionsPlayer potionPlayer)) {
            return;
        }

        // Add potion data received from packet.
        potionPlayer.BoundedPotionIds.Clear();
        potionPlayer.BoundedPotionIds.AddRange(_potionData);
        potionPlayer.empoweredPotionId = empoweredPotionId;
    }
}
