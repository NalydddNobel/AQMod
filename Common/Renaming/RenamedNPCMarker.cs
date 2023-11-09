using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Renaming;

public struct RenamedNPCMarker {
    public int type;
    public string customName;
    public int tileX;
    public int tileY;
    public readonly Guid Guid;

    public RenamedNPCMarker() {
        Guid = Guid.NewGuid();
    }

    public TagCompound Save() {
        var tag = new TagCompound {
            ["id"] = type,
            ["name"] = customName,
            ["x"] = tileX,
            ["y"] = tileY,
        };
        return tag;
    }

    public static RenamedNPCMarker Load(TagCompound tag) {
        return new RenamedNPCMarker() {
            type = tag.GetInt("id"),
            customName = tag.GetString("name"),
            tileX = tag.GetInt("x"),
            tileY = tag.GetInt("y"),
        };
    }

    public static RenamedNPCMarker FromNPC(NPC npc, RenameNPC renameNPC) {
        int tileX = npc.Center.ToTileCoordinates().X;
        int tileY = npc.Center.ToTileCoordinates().Y;
        if (NPCID.Sets.SpecialSpawningRules.TryGetValue(npc.netID, out int value)) {
            switch (value) {
                case 0: {
                    tileX = (int)npc.ai[0];
                    tileY = (int)npc.ai[1];
                }
                break;
            }
        }

        return new RenamedNPCMarker() {
            type = npc.netID,
            customName = renameNPC.CustomName,
            tileX = tileX,
            tileY = tileY,
        };
    }

    public static void SetupNPC(NPC npc) {
        if (NPCID.Sets.SpecialSpawningRules.TryGetValue(npc.netID, out int value)) {
            switch (value) {
                case 0: {
                    Point point = npc.Center.ToTileCoordinates();
                    npc.ai[0] = point.X;
                    npc.ai[1] = point.Y;
                    npc.netUpdate = true;
                }
                break;
            }
        }
    }
}