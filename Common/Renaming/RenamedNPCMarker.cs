using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public sealed class RenamedNPCMarker {
    public static readonly Vector2 BoxSize = new Vector2(2160f, 1440f);

    public int type;
    public string customName;
    public int tileX;
    public int tileY;
    public TagCompound MiscData { get; private set; }

    public Rectangle SpawnBox { get; private set; }

    public bool IsTrackingNPC => TrackNPC != -1;
    public bool IsTrackedNPCValid => Main.npc[TrackNPC].active && Main.npc[TrackNPC].type == type && Main.npc[TrackNPC].TryGetGlobalNPC<RenameNPC>(out var renameNPC) && renameNPC.CustomName == customName;

    public int TrackNPC { get; internal set; } = -1;

    public void Recalculate() {
        SpawnBox = Utils.CenteredRectangle(new Point(tileX, tileY).ToWorldCoordinates(), BoxSize);
    }

    public TagCompound Save() {
        var tag = new TagCompound {
            ["id"] = type,
            ["name"] = customName,
            ["x"] = tileX,
            ["y"] = tileY,
        };
        if (MiscData != null && MiscData.Count > 0) {
            tag["misc"] = MiscData;
        }
        return tag;
    }

    public static RenamedNPCMarker Load(TagCompound tag) {
        var marker = new RenamedNPCMarker() {
            type = tag.GetInt("id"),
            customName = tag.GetString("name"),
            tileX = tag.GetInt("x"),
            tileY = tag.GetInt("y"),
        };
        if (tag.TryGet<TagCompound>("misc", out var miscData)) {
            marker.MiscData = miscData;
        }
        marker.Recalculate();
        return marker;
    }

    public void UpdateTagCompound(NPC npc) {
        // Todo: Make NPCs saved like this sustain global data.
        MiscData = new();
        npc.ModNPC?.SaveData(MiscData);
    }

    public static RenamedNPCMarker FromNPC(NPC npc, RenameNPC renameNPC) {
        var marker = new RenamedNPCMarker() {
            type = npc.netID,
            customName = renameNPC.CustomName,
            tileX = npc.Center.ToTileCoordinates().X,
            tileY = npc.Center.ToTileCoordinates().Y,
        };

        if (NPCID.Sets.SpecialSpawningRules.TryGetValue(npc.netID, out int value)) {
            switch (value) {
                case 0: {
                        marker.tileX = (int)npc.ai[0];
                        marker.tileY = (int)npc.ai[1];
                    }
                    break;
            }
        }
        if (NPCID.Sets.RespawnEnemyID.TryGetValue(npc.netID, out int respawnId) && respawnId == 0) {
            marker.type = respawnId;
        }

        marker.UpdateTagCompound(npc);

        marker.Recalculate();
        marker.TrackNPC = npc.whoAmI;
        return marker;
    }

    public void SetupNPC(NPC npc) {
        if (NPCID.Sets.SpecialSpawningRules.TryGetValue(npc.netID, out int value)) {
            switch (value) {
                case 0: {
                        Point point = npc.Center.ToTileCoordinates();
                        npc.ai[0] = point.X;
                        npc.ai[1] = point.Y;
                    }
                    break;
            }
        }

        TrackNPC = npc.whoAmI;
        if (npc.TryGetGlobalNPC<RenameNPC>(out var renameNPC)) {
            renameNPC.CustomName = customName;
        }
        if (MiscData != null) {
            npc.ModNPC?.LoadData(MiscData);
        }
    }

    public void NetSend(BinaryWriter writer) {
        writer.Write(type);
        writer.Write(customName);
        writer.Write(tileX);
        writer.Write(tileY);
    }

    public void NetReceive(BinaryReader reader) {
        type = reader.ReadInt32();
        customName = reader.ReadString();
        tileX = reader.ReadInt32();
        tileY = reader.ReadInt32();
        Recalculate();
    }
}