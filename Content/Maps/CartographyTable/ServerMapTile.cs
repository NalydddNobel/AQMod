using System;
using System.IO;
using Terraria.Map;

namespace Aequus.Content.Maps.CartographyTable;

public struct ServerMapTile {
    public byte Light;
    public byte Type;

    public readonly bool Equals(ServerMapTile other) {
        return other.Light == Light && other.Type == Type;
    }

    public readonly bool Write(BinaryWriter writer) {
        // We dont really care about tiles which have less than 4 light.
        byte value = (byte)(Light >> 2);
        value |= (byte)(Type << 6);
        //if (value > 0) {
        //    Main.NewText(value);
        //}
        writer.Write(value);
        return true;
    }

    public static ServerMapTile FromReader(BinaryReader reader) {
        ServerMapTile next = new ServerMapTile();

        byte value = reader.ReadByte();
        next.Light = (byte)(value << 2);
        next.Type = (byte)(value >> 6);
        if (next.Light > 0) {
            return next;
        }
        return next;
    }

    public static ServerMapTile FromMapTile(int x, int y) {
        Tile tile = Main.tile[x, y];
        MapTile mapTile = Main.Map[x, y];
        ServerMapTile next = new ServerMapTile {
            Light = mapTile.Light,
            Type = MapTypeConvert.Instance.ToServerType(tile, mapTile.Type)
        };

        return next;
    }

    public readonly MapTile Create() {
        ushort type = MapTypeConvert.Instance.ToClientType(Type);

        return MapTile.Create(type, Light, 0);
    }

    public readonly bool ApplyToClient(int x, int y) {
        if (Main.netMode == NetmodeID.Server) {
            return false;
        }

        bool usesUnknownTile = false;
        MapTile tile = Main.Map[x, y];
        if (tile.Light <= 0 && Light > 0) {
            tile.Color = 0;
            tile.Type = MapTypeConvert.Instance.ToClientType(Type);
            usesUnknownTile = true;
        }
        tile.Light = Math.Max(tile.Light, Light);

        Main.Map.SetTile(x, y, ref tile);

        Main.refreshMap = true;

        return usesUnknownTile;
    }
}
