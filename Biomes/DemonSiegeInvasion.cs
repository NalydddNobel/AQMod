using Aequus.Common.Networking;
using Aequus.Items;
using Aequus.Sounds;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Biomes
{
    public sealed class DemonSiegeInvasion : ModBiome
    {
        public enum UpgradeProgression : byte
        {
            PreHardmode = 0,
        }
        public struct SacrificeData 
        {
            public readonly int OriginalItem;
            public readonly int NewItem;
            public UpgradeProgression Progression;

            public SacrificeData(int oldItem, int newItem, UpgradeProgression progression)
            {
                OriginalItem = oldItem;
                NewItem = newItem;
                Progression = progression;
            }

            public Item Convert(Item original)
            {
                int stack = original.stack;
                int prefix = original.prefix;
                original = original.Clone();
                original.SetDefaults(NewItem);
                original.stack = stack;
                original.Prefix(prefix);
                // TODO: Find a way to preserve global item content?
                return original;
            }
        }
        public sealed class EventSacrifice
        {
            public int TileX { get; internal set; }
            public int TileY { get; internal set; }

            public int MaxItems = 1;
            public float Range = 480f;
            public int TimeLeft = 3600;
            public int PreStart = 300;

            public List<Item> Items;

            public EventSacrifice(int x, int y)
            {
                TileX = x;
                TileY = y;
                Items = new List<Item>();
            }
            public void OnPlayerActivate(Player player)
            {

            }
            public int DetermineLength()
            {
                int time = 360;
                foreach (var i in Items)
                {
                    if (registeredSacrifices.TryGetValue(i.netID, out var value))
                    {
                        int newTime = 3600 + 1800 * (int)value.Progression;
                        if (time < newTime)
                        {
                            time = newTime;
                        }
                    }
                }
                return time;
            }

            public Rectangle ProtectedTiles()
            {
                return new Rectangle(TileX, TileY, 3, 4);
            }
            public bool OnValidTile()
            {
                return IsGoreNest(TileX, TileY);
            }

            public void Update()
            {
                if (PreStart > 0)
                {
                    PreStart--;
                    if (PreStart == 0)
                    {
                        InnerUpdate_OnStart();
                    }
                    return;
                }

                if (TimeLeft > 0)
                {
                    if (Items.Count == 0)
                    {
                        InnerUpdate_OnEnd();
                        return;
                    }
                    TimeLeft--;
                    if (TimeLeft % 120 == 0)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            PacketSender.Send((p) =>
                            {
                                SendStatusPacket(p);
                            }, PacketType.DemonSiegeSacrificeStatus);
                        }
                        //else
                        //{
                        //    Main.NewText(TimeLeft);
                        //    foreach (var i in Items)
                        //    {
                        //        Main.NewText(AequusText.ItemText(i.netID));
                        //    }
                        //}
                    }
                    return;
                }
                InnerUpdate_OnEnd();
            }
            public void InnerUpdate_OnStart()
            {
                TimeLeft = DetermineLength();
            }
            public void InnerUpdate_OnEnd()
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var source = new EntitySource_TileBreak(TileX, TileY, "GoreNest");
                    foreach (var i in Items)
                    {
                        TryFromID(i.netID, out var value);
                        var item = value.Convert(i);
                        int newItem = AequusItem.NewItemCloned(source, new Vector2(TileX * 16f + 32f, TileY * 16f - 20f), item);
                        Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                        }
                    }
                }
                DemonSiegeSystem.RemovalQueue.Add(new Point(TileX, TileY));
            }

            public void SendStatusPacket(BinaryWriter writer)
            {
                writer.Write((ushort)TileX);
                writer.Write((ushort)TileY);
                writer.Write((ushort)PreStart);
                writer.Write((ushort)TimeLeft);
                writer.Write((byte)MaxItems);
                writer.Write(Range);
                writer.Write((byte)Items.Count);
                for (int i = 0; i < Items.Count; i++)
                {
                    ItemIO.Send(Items[i], writer, true, false);
                }
            }

            public static void ReadPacket(BinaryReader reader)
            {
                int x = reader.ReadUInt16();
                int y = reader.ReadUInt16();
                EventSacrifice s;
                if (Sacrifices.TryGetValue(new Point(x, y), out var value))
                {
                    s = value;
                }
                else
                {
                    s = new EventSacrifice(x, y);
                    Sacrifices.Add(new Point(x, y), s);
                }
                s.InnerReadPacket(reader);
            }
            private void InnerReadPacket(BinaryReader reader)
            {
                PreStart = reader.ReadUInt16();
                TimeLeft = reader.ReadUInt16();
                MaxItems = reader.ReadByte();
                Range = reader.ReadSingle();
                int itemCount = reader.ReadByte();
                for (int i = 0; i < itemCount; i++)
                {
                    ItemIO.Receive(Items[i], reader, true, false);
                }
            }
        }

        public static Dictionary<Point, EventSacrifice> Sacrifices { get; private set; }

        internal static Dictionary<int, SacrificeData> registeredSacrifices;
        internal static Dictionary<int, int> upgradeToOriginal;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override int Music => MusicData.DemonSiegeEvent.GetID();

        public override string BestiaryIcon => "Assets/UI/BestiaryIcons/DemonSiege";

        public override void Load()
        {
            registeredSacrifices = new Dictionary<int, SacrificeData>();
            upgradeToOriginal = new Dictionary<int, int>();
            Sacrifices = new Dictionary<Point, EventSacrifice>();
        }

        public override void Unload()
        {
            registeredSacrifices?.Clear();
            registeredSacrifices = null;
            upgradeToOriginal?.Clear();
            upgradeToOriginal = null;
            Sacrifices?.Clear();
            Sacrifices = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            return false;
        }

        public static bool NewInvasion(int x, int y, Item sacrifice, int player = byte.MaxValue, bool checkIsValidSacrifice = true, bool allowAdding = true, bool allowAdding_IgnoreMax = false)
        {
            sacrifice = sacrifice.Clone();
            sacrifice.stack = 1;
            if (Sacrifices.TryGetValue(new Point(x, y), out var value))
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
            if (!registeredSacrifices.TryGetValue(sacrifice.netID, out var sacrificeData))
            {
                if (checkIsValidSacrifice)
                {
                    return false;
                }
                sacrificeData = Funny(sacrifice.netID);
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                PacketSender.Send((p) =>
                {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write((byte)player);
                    if (player != 255)
                    {
                        InnerWritePlayerSpecificData(Main.player[player], p);
                    }
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.RequestDemonSiege);
            }
            var s = new EventSacrifice(x, y);
            if (player != 255)
            {
                s.OnPlayerActivate(Main.player[player]);
            }
            s.Items.Add(sacrifice);
            Sacrifices.Add(new Point(x, y), s);
            return true;
        }
        public static void InnerWritePlayerSpecificData(Player player, BinaryWriter writer)
        {

        }

        public static bool TryFromID(int netID, out SacrificeData value)
        {
            if (registeredSacrifices.TryGetValue(netID, out value))
            {
                return true;
            }
            value = Funny(netID);
            return false;
        }
        public static SacrificeData FromID(int netID)
        {
            return registeredSacrifices[netID];
        }
        public static void Register(SacrificeData sacrifice)
        {
            registeredSacrifices.Add(sacrifice.OriginalItem, sacrifice);
            upgradeToOriginal.Add(sacrifice.NewItem, sacrifice.OriginalItem);
        }
        public static SacrificeData PHM(int original, int newItem)
        {
            return new SacrificeData(original, newItem, UpgradeProgression.PreHardmode);
        }
        public static SacrificeData Funny(int netID)
        {
            return new SacrificeData(netID, netID + 1, UpgradeProgression.PreHardmode);
        }

        public static bool IsGoreNest(int x, int y)
        {
            return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<GoreNestTile>();
        }

        public static void ReadRequest(BinaryReader reader)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            byte player = reader.ReadByte();
            if (player != 255)
            {
                InnerReadPlayerSpecificData(reader);
            }
            var s = new EventSacrifice(x, y);
            var sacrifice = new Item();
            ItemIO.Receive(sacrifice, reader, readStack: true, readFavorite: false);
            s.Items.Add(sacrifice);
            Sacrifices.Add(new Point(x, y), s);
        }
        public static void InnerReadPlayerSpecificData(BinaryReader reader)
        {
        }

        public class DemonSiegeBlockProtector : GlobalTile
        {
            public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
            {
                foreach (var s in Sacrifices)
                {
                    if (s.Value.ProtectedTiles().Contains(i, j))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public class DemonSiegeSystem : ModSystem
        {
            public static List<Point> RemovalQueue;

            public override void Load()
            {
                RemovalQueue = new List<Point>();
            }

            public override void Unload()
            {
                RemovalQueue = null;
            }

            public override void OnWorldLoad()
            {
                RemovalQueue?.Clear();
                Sacrifices?.Clear();
            }

            public override void OnWorldUnload()
            {
                RemovalQueue?.Clear();
                Sacrifices?.Clear();
            }

            public override void PostUpdateNPCs()
            {
                foreach (var s in Sacrifices)
                {
                    s.Value.TileX = s.Key.X;
                    s.Value.TileY = s.Key.Y;
                    s.Value.Update();
                }
                foreach (var p in RemovalQueue)
                {
                    Sacrifices.Remove(p);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        PacketSender.Send((packet) =>
                        {
                            packet.Write((ushort)p.X);
                            packet.Write((ushort)p.Y);
                        }, PacketType.RemoveDemonSiege);
                    }
                }
                RemovalQueue.Clear();
            }
        }
    }
}