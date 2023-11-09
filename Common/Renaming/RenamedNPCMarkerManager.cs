using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public class RenamedNPCMarkerManager : GlobalNPC {
    public override bool InstancePerEntity => true;

    protected override bool CloneNewInstances => true;

    public int MarkerGuid { get; internal set; } = 0;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return RenameNPC.CanRename(entity) && !NPCLoader.SavesAndLoads(entity);
    }

    public override void AI(NPC npc) {
        if (!npc.TryGetGlobalNPC<RenameNPC>(out var renameNPC) || !renameNPC.HasCustomName) {
            return;
        }

        if (npc.immortal || !renameNPC.HasCustomName) {
            RenamingSystem.RenamedNPCs.Remove(MarkerGuid);
            return;
        }

        if (RenamingSystem.RenamedNPCs.TryGetValue(MarkerGuid, out var marker)) {
            marker.customName = renameNPC.CustomName;
            marker.tileX = npc.Center.ToTileCoordinates().X;
            marker.tileY = npc.Center.ToTileCoordinates().Y;
            marker.TrackNPC = npc.whoAmI;
            marker.Recalculate();
        }
        else {
            if (MarkerGuid == 0) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    return;
                }
                MarkerGuid = Guid.NewGuid().GetHashCode();
            }
            var newMarker = RenamedNPCMarker.FromNPC(npc, renameNPC);
            newMarker.TrackNPC = npc.whoAmI;
            RenamingSystem.RenamedNPCs.Add(MarkerGuid, newMarker);
            npc.netUpdate = true;
        }
    }

    public override bool CheckActive(NPC npc) {
        if (MarkerGuid != 0 && RenamingSystem.RenamedNPCs.TryGetValue(MarkerGuid, out var marker)) {
            for (int j = 0; j < RenamingSystem.SpawnRectangles.Count; j++) {
                if (RenamingSystem.SpawnRectangles[j].Intersects(marker.SpawnBox)) {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool PreKill(NPC npc) {
        RenamingSystem.RenamedNPCs.Remove(MarkerGuid);
        return true;
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
        binaryWriter.Write(MarkerGuid);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
        MarkerGuid = binaryReader.ReadInt32();
    }
}