using System;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequu2.Content.Systems.Renaming;

public sealed class RenamedNPCMarkerManager : GlobalNPC {
    public override bool InstancePerEntity => true;

    protected override bool CloneNewInstances => true;

    public int MarkerId { get; internal set; } = 0;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return RenameNPC.CanRename(entity) && !NPCLoader.SavesAndLoads(entity) && (!NPCSets.RespawnEnemyID.TryGetValue(entity.type, out int respawnId) || respawnId != 0);
    }

    public override void AI(NPC npc) {
        if (!npc.TryGetGlobalNPC<RenameNPC>(out var renameNPC) || !renameNPC.HasCustomName) {
            return;
        }

        if (npc.immortal || !renameNPC.HasCustomName) {
            RenamingSystem.RenamedNPCs.Remove(MarkerId);
            return;
        }

        if (RenamingSystem.RenamedNPCs.TryGetValue(MarkerId, out var marker)) {
            marker.customName = renameNPC.CustomName;
            marker.tileX = npc.Center.ToTileCoordinates().X;
            marker.tileY = npc.Center.ToTileCoordinates().Y;
            marker.TrackNPC = npc.whoAmI;
            marker.Recalculate();
        }
        else if (npc.realLife == -1 || npc.realLife == npc.whoAmI) {
            if (MarkerId == 0) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    return;
                }
                MarkerId = Guid.NewGuid().GetHashCode();
            }

            var newMarker = RenamedNPCMarker.FromNPC(npc, renameNPC);
            newMarker.TrackNPC = npc.whoAmI;
            npc.netUpdate = true;

            RenamingSystem.RenamedNPCs.Add(MarkerId, newMarker);
            if (Main.netMode == NetmodeID.Server) {
                ModContent.GetInstance<PacketAddMarker>().Send(MarkerId, newMarker);
            }
        }
    }

    //public override bool CheckActive(NPC npc) {
    //    if (MarkerId != 0 && RenamingSystem.RenamedNPCs.TryGetValue(MarkerId, out var marker)) {
    //        for (int j = 0; j < RenamingSystem.SpawnRectangles.Count; j++) {
    //            if (RenamingSystem.SpawnRectangles[j].Intersects(marker.SpawnBox)) {
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}

    public override void HitEffect(NPC npc, NPC.HitInfo hit) {
        if (npc.life <= 0 && Main.netMode != NetmodeID.MultiplayerClient) {
            RenamingSystem.Remove(MarkerId);
        }
    }

    public override bool PreKill(NPC npc) {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            RenamingSystem.Remove(MarkerId);
        }
        return true;
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(MarkerId);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        MarkerId = binaryReader.ReadInt32();
    }
}