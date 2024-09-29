using Aequus.Common.IO;
using Aequus.Common.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Entities.PotionAffixes.Bounded;

public class BoundedPotionPlayer : ModPlayer {
    public readonly List<int> bounded = [];

    public override void Load() {
        On_Player.DelBuff += On_Player_DelBuff;
    }

    static void On_Player_DelBuff(On_Player.orig_DelBuff orig, Player player, int b) {
        int buffType = player.buffType[b];

        // Clear bounded data for this buff Id
        if (player.TryGetModPlayer(out BoundedPotionPlayer potions)) {
            potions.bounded.Remove(buffType);
        }

        orig(player, b);
    }

    public override void UpdateDead() {
        foreach (int buff in bounded) {
            // Set all bounded buffs to be persistent
            // Since if they were already persistant, you wouldn't be able to make the potion anyway.
            Main.persistentBuff[buff] = true;
        }
    }

    public override void ResetEffects() {
        foreach (int buff in bounded) {
            // Set all bounded buffs to not be persistent
            Main.persistentBuff[buff] = false;
        }
    }

    const string TAG = "Bounded";

    public override void SaveData(TagCompound tag) {
        if (bounded.Count > 0) {
            tag[TAG] = bounded.Select(IDCommons<BuffID>.GetStringIdentifier).ToList();
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet(TAG, out List<string> boundedPotions)) {
            bounded.Clear();

            foreach (string b in boundedPotions) {
                if (IDCommons<BuffID>.FromStringIdentifier(b, out int id)) {
                    bounded.Add(id);
                }
            }
        }
    }

    public override void CopyClientState(ModPlayer targetCopy) {
        BoundedPotionPlayer copy = (BoundedPotionPlayer)targetCopy;

        copy.bounded.Clear();
        copy.bounded.AddRange(bounded);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        Instance<BoundedPotionPlayerPacket>().Send(Player, this, toWho, fromWho);
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        BoundedPotionPlayer clone = (BoundedPotionPlayer)clientPlayer;

        if (!BoundPotionsMatch(clone)) {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }

    bool BoundPotionsMatch(BoundedPotionPlayer clone) {
        if (clone.bounded.Count != bounded.Count) {
            return false; // Length mismatch
        }

        for (int i = 0; i < clone.bounded.Count; i++) {
            if (clone.bounded[i] != bounded[i]) {
                return false; // Type mismatch
            }
        }

        return true;
    }
}

public class BoundedPotionPlayerPacket : PacketHandler {
    private readonly List<int> _boundedPotions = [];

    public override PacketType LegacyPacketType => PacketType.BoundedPotionPlayer;

    public void Send(Player player, BoundedPotionPlayer potionPlayer, int toWho = -1, int fromWho = -1) {
        var packet = GetPacket();

        packet.Write((byte)player.whoAmI);

        lock (potionPlayer) {
            int count = potionPlayer.bounded.Count;

            // We pray that Player.MaxBuffs is the same between clients.
            if (Player.MaxBuffs < byte.MaxValue) {
                packet.Write((byte)count);
            }
            else {
                packet.Write(count);
            }

            for (int i = 0; i < count; i++) {
                packet.Write(potionPlayer.bounded[i]);
            }
        }

        SendPacket(packet, toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte plr = reader.ReadByte();

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
        _boundedPotions.Clear();
        _boundedPotions.EnsureCapacity(count);

        for (int i = 0; i < count; i++) {
            _boundedPotions.Add(reader.ReadInt32());
        }

        if (!Main.player[plr].active || !Main.player[plr].TryGetModPlayer(out BoundedPotionPlayer potionPlayer)) {
            return;
        }

        // Add potion data received from packet.
        potionPlayer.bounded.Clear();
        potionPlayer.bounded.AddRange(_boundedPotions);
    }
}
