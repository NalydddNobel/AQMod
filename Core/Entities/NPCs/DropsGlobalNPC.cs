using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Networking;

namespace AequusRemake.Core.Entities.NPCs;

public sealed class DropsGlobalNPC : GlobalNPC {
    private static readonly Dictionary<int, List<IItemDropRule>> _dropRules = new();

    public bool noOnKillEffects;

    public override bool InstancePerEntity => true;

    public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
        globalLoot.Add(Content.Items.PermaPowerups.NoHit.NoHitReward.GetGlobalLoot());
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (_dropRules.TryGetValue(npc.netID, out var dropRules)) {
            foreach (var rule in dropRules) {
                npcLoot.Add(rule);
            }
        }
    }

    public override bool SpecialOnKill(NPC npc) {
        if (noOnKillEffects) {
            return true;
        }

        return base.SpecialOnKill(npc);
    }

    public override void OnKill(NPC npc) {
        // Only activate on kill effects on NPCs which are not friendly and have greater than 5 HP.
        if (npc.friendly || npc.lifeMax <= 5) {
            return;
        }

        // Find the closest eligable player.
        Player nearestPlayer = null;
        float closestDistance = 2000f; // On kill range.
        for (int i = 0; i < Main.maxPlayers; i++) {
            Player player = Main.player[i];
            if (!npc.playerInteraction[i] || !player.active || player.DeadOrGhost || !player.TryGetModPlayer<AequusPlayer>(out _)) {
                continue;
            }

            float distance = npc.Distance(player.Center);
            if (distance < closestDistance) {
                closestDistance = distance;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer == null) {
            return;
        }

        var killInfo = new AequusPlayer.KillInfo(npc.Center, npc.type);
        nearestPlayer.GetModPlayer<AequusPlayer>().OnKillNPC(in killInfo);

        if (Main.netMode == NetmodeID.Server) {
            GetPacket<OnKillPacket>().Send(nearestPlayer, killInfo);
        }
    }

    /// <summary>
    /// Allows you to add a drop rule to an NPC. Please only call this in SetStaticDefaults/PostSetupContent.
    /// </summary>
    /// <param name="npcId">The NPC type.</param>
    /// <param name="rule">The item drop rule.</param>
    internal static void AddNPCLoot(int npcId, IItemDropRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_dropRules, npcId, out _) ??= new()).Add(rule);
    }

    public override void Load() {
    }

    public override void Unload() {
        _dropRules.Clear();
    }
}

internal class OnKillPacket : PacketHandler {
    public void Send(Player player, AequusPlayer.KillInfo info) {
        ModPacket packet = GetPacket();
        packet.WritePackedVector2(info.Center);
        packet.Write(info.Type);
        packet.Write((byte)player.whoAmI);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        Vector2 center = reader.ReadPackedVector2();
        int type = reader.ReadInt32();
        int player = reader.ReadByte();

        if (!Main.player.IndexInRange(player) || Main.player[player].TryGetModPlayer(out AequusPlayer AequusRemake)) {
            return;
        }

        var killInfo = new AequusPlayer.KillInfo(center, type);
        AequusRemake.OnKillNPC(killInfo);
    }
}