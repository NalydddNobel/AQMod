using System;
using System.IO;
using Terraria.Map;

namespace Aequus.Content.Maps.CartographyTable;

public struct ServerMapTile {
    public byte Light;
    public byte Type;

    public bool Equals(ServerMapTile other) {
        return other.Light == Light && other.Type == Type;
    }

    public bool Write(BinaryWriter writer) {
        writer.Write(Light);

        if (Light == 0) {
            return false;
        }

        writer.Write(Type);
        return true;
    }

    public static ServerMapTile FromReader(BinaryReader reader) {
        ServerMapTile next = new ServerMapTile();

        next.Light = reader.ReadByte();
        if (next.Light > 0) {
            next.Type = reader.ReadByte();
        }

        return next;
    }

    public static ServerMapTile FromMapTile(int x, int y) {
        Tile tile = Main.tile[x, y];
        MapTile mapTile = Main.Map[x, y];
        ServerMapTile next = new ServerMapTile();

        next.Light = mapTile.Light;
        next.Type = MapTypeConvert.Instance.ToServerType(tile, mapTile.Type);

        return next;
    }

    public MapTile Create() {
        ushort type = MapTypeConvert.Instance.ToClientType(Type);

        return MapTile.Create(type, Light, 0);
    }

    public bool ApplyToClient(int x, int y) {
        if (Main.netMode == NetmodeID.Server) {
            return false;
        }

        bool usesUnknownTile = false;
        MapTile tile = Main.Map[x, y];
        tile.Light = Math.Max(tile.Light, Light);
        if (tile.Light <= 0) {
            tile.Color = 0;
            tile.Type = MapTypeConvert.Instance.ToClientType(Type);
            usesUnknownTile = true;
        }

        Main.Map.SetTile(x, y, ref tile);

        Main.refreshMap = true;

        return usesUnknownTile;
    }
}
