/*
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Net;
using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Aequus.Content.Maps.CartographyTable;

// Rushed.


public class CartographyTable : ModTile {
    private const string TimerName = nameof(CartographyTable);

    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this));
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.Origin = new(1, 3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.addTile(Type);
        AddMapEntry(Color.SaddleBrown);
        HitSound = SoundID.Dig;
        DustType = DustID.WoodFurniture;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        int offset = Math.Clamp(WorldGen.GetWorldSize(), 0, 2);
        drawData.addFrX = offset * 72;
    }

    public override bool RightClick(int i, int j) {
        TimerPlayer cd = Main.LocalPlayer.GetModPlayer<TimerPlayer>();
        if (Main.netMode == NetmodeID.MultiplayerClient && !cd.IsTimerActive(TimerName)) {
            PacketSystem.Get<CartographyTableGetDataPacket>().Send(0, Main.myPlayer);
            cd.SetTimer(TimerName, 1200);
        }
        return true;
    }
}

public class CartographyTableSystem : ModSystem {
    private byte[] _mapLights = [];

    internal int NextSection;

    public const int SectionWidth = 50;
    public const int SectionHeight = 50;

    private static int TileArea => Main.maxTilesX * Main.maxTilesY;

    public override void ClearWorld() {
        _mapLights = new byte[TileArea];
    }

    public override void SaveWorldData(TagCompound tag) {
        tag["Map"] = _mapLights;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("Map", out byte[] loadedMap)) {
            _mapLights = loadedMap;
            if (_mapLights.Length != TileArea) {
                Array.Resize(ref _mapLights, TileArea);
            }
        }
    }

    public void SetLight(int i, byte value) {
        _mapLights[i] = Math.Max(value, _mapLights[i]);
    }

    public void SetLight(int x, int y, byte value) {
        SetLight(GetIndex(x, y), value);
    }

    public byte GetLight(int i) {
        return _mapLights[i];
    }

    public byte GetLight(int x, int y) {
        return GetLight(GetIndex(x, y));
    }

    private int GetIndex(int x, int y) {
        return Math.Clamp(x + y * Main.maxTilesX, 0, _mapLights.Length);
    }

    // Garbage.
    public void GetSection(int section, out int x, out int y, out int w, out int h) {
        int sectionsRow = Main.maxTilesX / SectionWidth;
        if (SectionWidth * sectionsRow < Main.maxTilesX) {
            sectionsRow++;
        }

        x = section % sectionsRow * SectionWidth;
        y = section / sectionsRow * SectionHeight;
        w = Math.Min(SectionWidth, Main.maxTilesX - x);
        h = Math.Min(SectionHeight, Main.maxTilesY - y);
    }

    public override void PostUpdateEverything() {
        if (NextSection == 0) {
            return;
        }
    }
}

public class CartographyTableGetDataPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CartographyTable;

    const byte WallOffset = 100;
    const byte WaterOffset = 200;

    public void Send(int section, int player) {
        CartographyTableSystem map = ModContent.GetInstance<CartographyTableSystem>();
        map.GetSection(section, out int x, out int y, out int w, out int h);

        if (w <= 0 || h <= 0) {
            map.NextSection = 0;
            return;
        }

        ModPacket packet = GetPacket();

        packet.Write(section);
        packet.Write(player);

        bool any = false;
        for (int i = x; i < x + w; i++) {
            for (int j = y; j < y + h; j++) {
                byte light = map.GetLight(i, j);
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    light = Math.Max(Main.Map[i, j].Light, light);
                }

                packet.Write(light);
                if (light <= 0) {
                    continue;
                }
                any = true;

                if (Main.netMode == NetmodeID.Server) {
                    // Update tile info aswell.

                    Tile tile = Main.tile[i, j];

                    // Send tile if it is visible.
                    if (tile.HasTile && !tile.IsTileInvisible) {
                        packet.Write(tile.TileColor);
                        packet.Write(tile.TileType);
                    }
                    // Otherwise, check if the wall is visible
                    // Send the liquid too because of polluted ocean blocks
                    else if (tile.WallType > WallID.None && !tile.IsWallInvisible) {
                        packet.Write((byte)(WallOffset + tile.WallColor));
                        packet.Write(tile.WallType);
                        packet.Write(tile.LiquidAmount);
                        packet.Write((byte)tile.LiquidType);
                    }
                    // If there is no wall, just send liquid info.
                    else {
                        packet.Write((byte)(WaterOffset + tile.LiquidType));
                        packet.Write(tile.LiquidAmount);
                    }

                    // Does this even work???
                    //MapTile tile = MapHelper.CreateMapTile(i, j, light);
                    //packet.Write(tile.Type);
                    //packet.Write(tile.Color);
                }
            }
        }

        if (!any) {
            map.NextSection = section + 1;
            return;
        }
        if (Main.netMode == NetmodeID.Server) {
            packet.Send(toClient: player);
        }
        else {
            packet.Send();
            map.NextSection++;
        }
    }

    public override void Receive(BinaryReader reader, int sender) {
        int section = reader.ReadInt32();
        int player = reader.ReadInt32();

        CartographyTableSystem map = ModContent.GetInstance<CartographyTableSystem>();
        map.GetSection(section, out int x, out int y, out int w, out int h);

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Mod.Logger.Info($"[{Main.player[player].name}] Receiving cartography section: {section}");
            Main.NewText($"Receiving cartography section: {section}");
        }
        else {
            Mod.Logger.Info($"[Server] Receiving cartography section: {section}");
        }

        for (int i = x; i < x + w; i++) {
            for (int j = y; j < y + h; j++) {
                byte light = reader.ReadByte();
                if (light <= 0) {
                    continue;
                }

                if (Main.netMode != NetmodeID.Server) {
                    Tile testTile = Main.tile[0, 0];
                    testTile.ClearEverything();

                    byte clr = reader.ReadByte();

                    switch (clr) {
                        case < WallOffset:
                            testTile.TileColor = clr;
                            testTile.TileType = reader.ReadUInt16();
                            break;

                        case < WaterOffset:
                            testTile.LiquidAmount = reader.ReadByte();
                            testTile.LiquidType = reader.ReadByte();
                            testTile.WallType = reader.ReadUInt16();
                            testTile.WallColor = (byte)(clr - WallOffset);
                            break;

                        default:
                            testTile.LiquidAmount = reader.ReadByte();
                            testTile.LiquidType = clr - WaterOffset;
                            break;
                    }

                    light = Math.Max(Main.Map[i, j].Light, light);
                    MapTile tile = MapHelper.CreateMapTile(0, 0, light);
                    tile.IsChanged = true;
                    //Main.NewText(tile.Type);
                    Main.Map.SetTile(i, j, ref tile);
                    WorldGen.UpdateMapTile(i, j);
                    //Main.clearMap = true;
                    //Helper.DebugDustRectangle(i, j);
                }
                map.SetLight(i, j, light);
            }
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(section, player);
        }
        else {
            Send(map.NextSection, player);
        }
    }
}
*/