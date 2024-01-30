using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Events.DemonSiege;

public class DemonSiegeSystem : ModSystem {
    public static readonly Color TextColor = new Color(255, 210, 25, 255);

    public static readonly Dictionary<Point, DemonSiegeSacrificeInfo> ActiveSacrifices = new();
    public static readonly List<Point> SacrificeRemovalQueue = new();

    public static Int32 DemonSiegePause { get; set; }

    public override void Unload() {
        ActiveSacrifices.Clear();
        SacrificeRemovalQueue.Clear();
    }

    public override void ClearWorld() {
        SacrificeRemovalQueue.Clear();
        ActiveSacrifices.Clear();
    }

    public override void PostUpdateInput() {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
        Single screenSize = MathF.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight + Main.screenHeight);
        foreach (var sacrifice in ActiveSacrifices) {
            var v = sacrifice.Value;
            var center = v.WorldCenter;
            v._visible = Vector2.Distance(v.WorldCenter, screenCenter) < screenSize + v.Range * 2f;
        }
    }

    public override void PostUpdateNPCs() {
        if (DemonSiegePause > 0) {
            DemonSiegePause--;
        }

        foreach (var s in ActiveSacrifices) {
            s.Value.TileX = s.Key.X;
            s.Value.TileY = s.Key.Y;
            s.Value.Update();
        }
        foreach (var p in SacrificeRemovalQueue) {
            ActiveSacrifices.Remove(p);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                Aequus.GetPacket<RemoveDemonSiegePacket>().Send(p.X, p.Y);
            }
        }
        SacrificeRemovalQueue.Clear();
    }

    private static void DrawSacrificeRings() {
        if (ActiveSacrifices.Count <= 0) {
            return;
        }

        var auraTexture = AequusTextures.GoreNestAura.Value;
        var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
        Single screenSize = MathF.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight + Main.screenHeight);
        Main.spriteBatch.BeginWorld(shader: false);
        try {
            foreach (var sacrifice in ActiveSacrifices) {
                var v = sacrifice.Value;
                var center = v.WorldCenter;
                if (!v.Renderable) {
                    continue;
                }
                var origin = auraTexture.Size() / 2f;
                var drawCoords = (center - Main.screenPosition).Floor();
                Single scale = v.Range * 2f / auraTexture.Width;
                Single opacity = 1f;

                if (v.TimeLeft < 360) {
                    opacity = v.TimeLeft / 360f;
                }

                Single auraScale = v.AuraScale;
                var color = Color.Lerp(Color.Red * 0.75f, Color.OrangeRed, Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 1f)) * opacity;
                Main.spriteBatch.Draw(auraTexture, drawCoords, null, color,
                    0f, origin, scale * auraScale, SpriteEffects.None, 0f);
            }
        }
        catch {

        }
        Main.spriteBatch.End();
    }
    public override void PostDrawTiles() {
        DrawSacrificeRings();
    }

    public static Boolean NewInvasion(Int32 x, Int32 y, Item sacrifice, Int32 player = Byte.MaxValue, Boolean checkIsValidSacrifice = true, Boolean allowAdding = true, Boolean allowAdding_IgnoreMax = false) {
        sacrifice = sacrifice.Clone();
        sacrifice.stack = 1;
        if (ActiveSacrifices.TryGetValue(new Point(x, y), out var value)) {
            if (allowAdding) {
                if (allowAdding_IgnoreMax || value.MaxItems < value.Items.Count) {
                    value.Items.Add(sacrifice);
                    return true;
                }
            }
            return false;
        }
        if (!AltarSacrifices.OriginalToConversion.TryGetValue(sacrifice.netID, out var sacrificeData)) {
            if (checkIsValidSacrifice) {
                return false;
            }
            sacrificeData = new Conversion(sacrifice.netID, sacrifice.netID + 1, EventTier.PreHardmode);
        }
        var s = new DemonSiegeSacrificeInfo(x, y) {
            player = (Byte)player
        };
        s.Items.Add(sacrifice);
        if (Main.netMode != NetmodeID.SinglePlayer) {
            Aequus.GetPacket<StartDemonSiegePacket>().Send(x, y, player, sacrifice);
        }
        if (player != 255) {
            s.OnPlayerActivate(Main.player[player]);
        }
        ActiveSacrifices.Add(new Point(x, y), s);
        return true;
    }

    public class StartDemonSiegePacket : PacketHandler {
        public void Send(Int32 x, Int32 y, Int32 player, Item item) {
            ModPacket p = GetPacket();
            p.Write((UInt16)x);
            p.Write((UInt16)y);
            p.Write((Byte)player);
            ItemIO.Send(item, p, writeStack: true, writeFavorite: false);
            p.Send(ignoreClient: player);
        }

        public override void Receive(BinaryReader reader, Int32 sender) {
            UInt16 X = reader.ReadUInt16();
            UInt16 Y = reader.ReadUInt16();
            Byte Player = reader.ReadByte();
            Item Item = ItemIO.Receive(reader, readStack: true, readFavorite: false);
            DemonSiegeSacrificeInfo s = new DemonSiegeSacrificeInfo(X, Y) {
                player = Player,
            };
            s.Items.Add(Item);

            ActiveSacrifices.Add(new Point(X, Y), s);

            if (Main.netMode == NetmodeID.Server) {
                Send(X, Y, Player, Item);
            }
        }
    }

    public class RemoveDemonSiegePacket : PacketHandler {
        public void Send(Int32 x, Int32 y) {
            ModPacket p = GetPacket();
            p.Write((UInt16)x);
            p.Write((UInt16)y);
            p.Send();
        }

        public override void Receive(BinaryReader reader, Int32 sender) {
            UInt16 X = reader.ReadUInt16();
            UInt16 Y = reader.ReadUInt16();

            lock (ActiveSacrifices) {
                try {
                    ActiveSacrifices.Remove(new Point(X, Y));
                }
                catch (Exception ex) {
                    Mod.Logger.Error(ex);
                }
            }
        }
    }
}