using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Items.Tools;
using Aequus.Tiles;
using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Biomes.DemonSiege
{
    public class DemonSiegeSystem : ModSystem
    {
        public static Dictionary<Point, DemonSiegeSacrifice> ActiveSacrifices { get; private set; }
        public static List<Point> SacrificeRemovalQueue { get; private set; }
        public static Dictionary<int, SacrificeData> RegisteredSacrifices { get; private set; }
        public static Dictionary<int, int> SacrificeResultItemIDToOriginalItemID { get; private set; }

        public override void Load()
        {
            RegisteredSacrifices = new Dictionary<int, SacrificeData>();
            SacrificeResultItemIDToOriginalItemID = new Dictionary<int, int>();
            ActiveSacrifices = new Dictionary<Point, DemonSiegeSacrifice>();
            SacrificeRemovalQueue = new List<Point>();

            if (!Main.dedServ)
            {
                EventProgressBarLoader.AddBar(new DemonSiegeProgressBar()
                {
                    EventKey = "Mods.Aequus.BiomeName.DemonSiegeBiome",
                    Icon = Aequus.AssetsPath + "UI/EventIcons/DemonSiege",
                    backgroundColor = new Color(180, 100, 20, 128),
                });
            }
        }

        public override void Unload()
        {
            RegisteredSacrifices?.Clear();
            RegisteredSacrifices = null;
            SacrificeResultItemIDToOriginalItemID?.Clear();
            SacrificeResultItemIDToOriginalItemID = null;
            ActiveSacrifices?.Clear();
            ActiveSacrifices = null;
            SacrificeRemovalQueue?.Clear();
            SacrificeRemovalQueue = null;
        }

        public override void OnWorldLoad()
        {
            SacrificeRemovalQueue?.Clear();
            ActiveSacrifices?.Clear();
        }

        public override void OnWorldUnload()
        {
            SacrificeRemovalQueue?.Clear();
            ActiveSacrifices?.Clear();
        }

        public override void PostUpdateNPCs()
        {
            foreach (var s in ActiveSacrifices)
            {
                s.Value.TileX = s.Key.X;
                s.Value.TileY = s.Key.Y;
                s.Value.Update();
            }
            foreach (var p in SacrificeRemovalQueue)
            {
                ActiveSacrifices.Remove(p);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketHandler.Send((packet) =>
                    {
                        packet.Write((ushort)p.X);
                        packet.Write((ushort)p.Y);
                    }, PacketType.RemoveDemonSiege);
                }
            }
            SacrificeRemovalQueue.Clear();
        }

        public override void PostDrawTiles()
        {
            if (GoreNestTile.RenderPoints.Count > 0)
            {
                var auraTexture = ModContent.Request<Texture2D>("Aequus/Assets/GoreNestAura");
                if (!auraTexture.IsLoaded)
                    return;
                Begin.GeneralEntities.Begin(Main.spriteBatch);
                try
                {
                    DrawDemonSiegeRanges(auraTexture.Value);
                }
                catch
                {

                }
                Main.spriteBatch.End();
            }
        }

        public bool DrawDemonSiegeRanges(Texture2D auraTexture)
        {
            foreach (var v in GoreNestTile.RenderPoints)
            {
                if (ActiveSacrifices.TryGetValue(v, out var sacrifice) && sacrifice._auraScale > 0f)
                {
                    var texture = auraTexture;
                    var origin = texture.Size() / 2f;
                    var drawCoords = (sacrifice.WorldCenter - Main.screenPosition).Floor();
                    float scale = sacrifice.Range * 2f / texture.Width;
                    float opacity = 1f;

                    if (sacrifice.TimeLeft < 360)
                    {
                        opacity = sacrifice.TimeLeft / 360f;
                    }

                    opacity /= 5f;

                    var color = Color.Lerp(Color.Red * 0.5f, Color.OrangeRed * 0.75f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f)) * opacity;
                    foreach (var c in AequusHelpers.CircularVector(4))
                    {
                        Main.spriteBatch.Draw(texture, drawCoords + c, null, color,
                            0f, origin, scale * sacrifice._auraScale, SpriteEffects.None, 0f);
                    }
                    Main.spriteBatch.Draw(texture, drawCoords, null, color,
                        0f, origin, scale * sacrifice._auraScale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public static void RegisterSacrifice(SacrificeData sacrifice)
        {
            RegisteredSacrifices.Add(sacrifice.OriginalItem, sacrifice);
            SacrificeResultItemIDToOriginalItemID.Add(sacrifice.NewItem, sacrifice.OriginalItem);
        }

        public static bool NewInvasion(int x, int y, Item sacrifice, int player = byte.MaxValue, bool checkIsValidSacrifice = true, bool allowAdding = true, bool allowAdding_IgnoreMax = false)
        {
            sacrifice = sacrifice.Clone();
            sacrifice.stack = 1;
            if (ActiveSacrifices.TryGetValue(new Point(x, y), out var value))
            {
                if (allowAdding)
                {
                    if (allowAdding_IgnoreMax || value.MaxItems < value.Items.Count)
                    {
                        value.Items.Add(sacrifice);
                        return true;
                    }
                }
                return false;
            }
            if (!RegisteredSacrifices.TryGetValue(sacrifice.netID, out var sacrificeData))
            {
                if (checkIsValidSacrifice)
                {
                    return false;
                }
                sacrificeData = new SacrificeData(sacrifice.netID, sacrifice.netID + 1, UpgradeProgressionType.PreHardmode);
            }
            var s = new DemonSiegeSacrifice(x, y);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                PacketHandler.Send((p) =>
                {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write((byte)player);
                    if (player != 255)
                    {
                        InnerWritePlayerSpecificRequest(s, Main.player[player], p);
                    }
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.StartDemonSiege);
            }
            if (player != 255)
            {
                s.OnPlayerActivate(Main.player[player]);
            }
            s.Items.Add(sacrifice);
            ActiveSacrifices.Add(new Point(x, y), s);
            return true;
        }
        public static void InnerWritePlayerSpecificRequest(DemonSiegeSacrifice s, Player player, BinaryWriter writer)
        {
        }

        public static void ReceiveStartRequest(BinaryReader reader)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            var s = new DemonSiegeSacrifice(x, y);
            byte player = reader.ReadByte();
            if (player != 255)
            {
                InnerReadPlayerSpecificRequest(s, reader);
            }
            var sacrifice = new Item();
            ItemIO.Receive(sacrifice, reader, readStack: true, readFavorite: false);
            s.Items.Add(sacrifice);
            ActiveSacrifices.Add(new Point(x, y), s);

            if (Main.netMode == NetmodeID.Server)
            {
                PacketHandler.Send((p) =>
                {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write(player);
                    if (player != 255)
                    {
                        InnerWritePlayerSpecificRequest(s, Main.player[player], p);
                    }
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.StartDemonSiege, ignore: player);
            }
        }
        public static void InnerReadPlayerSpecificRequest(DemonSiegeSacrifice s, BinaryReader reader)
        {
        }

        /// <summary>
        /// Finds and returns the closest demon siege
        /// </summary>
        /// <returns></returns>
        public static Point FindDemonSiege(Vector2 location)
        {
            foreach (var s in ActiveSacrifices)
            {
                if (Vector2.Distance(location, new Vector2(s.Value.TileX * 16f + 24f, s.Value.TileY * 16f)) < s.Value.Range)
                {
                    return s.Key;
                }
            }
            return Point.Zero;
        }
    }
}