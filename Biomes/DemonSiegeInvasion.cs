using Aequus.Common.Networking;
using Aequus.Items;
using Aequus.Particles.Dusts;
using Aequus.Sounds;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Audio;

namespace Aequus.Biomes
{
    public class DemonSiegeInvasion : ModBiome
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

            public Vector2 Center => new Vector2(TileX * 16f + 24f, TileY * 16f);

            public int MaxItems = 1;
            public float Range = 800f;
            public int TimeLeftMax = 3600;
            public int TimeLeft = 3600;
            public int PreStart = 300;
            public int NetUpdate;

            public readonly List<Item> Items;

            public float _auraScale;
            public bool _playedSound;

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
                int time = 600;
                foreach (var i in Items)
                {
                    if (registeredSacrifices.TryGetValue(i.netID, out var value))
                    {
                        int newTime = 5400 + 1800 * (int)value.Progression;
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
                if (!OnValidTile())
                {
                    InnerUpdate_OnFail();
                    return;
                }
                if (!_playedSound)
                {
                    _playedSound = true;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, new Vector2(TileX * 16f + 24f, TileY * 16f));
                    }
                }
                if (PreStart > 0)
                {
                    PreStart--;
                    if (PreStart == 0)
                    {
                        InnerUpdate_OnStart();
                    }
                    return;
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetUpdate++;
                    if (NetUpdate > 120 && Main.netMode == NetmodeID.Server)
                    {
                        PacketHandler.Send((p) =>
                        {
                            SendStatusPacket(p);
                        }, PacketType.DemonSiegeSacrificeStatus);
                        NetUpdate = 0;
                    }
                    if (NetUpdate > 300)
                    {
                        InnerUpdate_OnFail(clientOnly: true);
                        return;
                    }
                }
                if (TimeLeft > 0)
                {
                    if (_auraScale < 1f)
                    {
                        _auraScale = MathHelper.Lerp(_auraScale, 1f, 0.05f);
                        if (_auraScale > 1f)
                        {
                            _auraScale = 1f;
                        }
                    }
                    if (Items.Count == 0)
                    {
                        InnerUpdate_OnEnd();
                        return;
                    }
                    var center = Center;
                    InnerUpdate_TimeLeft(center);
                    return;
                }
                InnerUpdate_OnEnd();
            }
            public void InnerUpdate_OnStart()
            {
                TimeLeftMax = TimeLeft = DetermineLength();
            }
            public void InnerUpdate_TimeLeft(Vector2 center)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(center) < Range)
                    {
                        TimeLeft--;
                        return;
                    }
                }
                TimeLeft++;
                if (TimeLeft > TimeLeftMax)
                {
                    InnerUpdate_OnFail();
                }
            }
            public void InnerUpdate_OnEnd()
            {
                Vector2 itemSpawn = Center;
                itemSpawn.Y -= 20f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var source = new EntitySource_TileBreak(TileX, TileY, "GoreNest");
                    //AequusText.Broadcast("Should be spawning these items: " + AequusText.ItemText(Items[0].type), Color.Red);
                    foreach (var i in Items)
                    {
                        TryFromID(i.type, out var value);
                        var item = value.Convert(i);
                        int newItem = AequusItem.NewItemCloned(source, itemSpawn, item);
                        Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                        }
                    }
                    DemonSiegeSystem.RemovalQueue.Add(new Point(TileX, TileY));
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        var d = Dust.NewDustPerfect(itemSpawn + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(20f, 100f)), ModContent.DustType<MonoSparkleDust>(),
                            newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 200) * Main.rand.NextFloat(0.9f, 1.5f), Scale: Main.rand.NextFloat(1f, 2.5f));
                        d.velocity = (d.position - itemSpawn) / 20f;
                        d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                    }
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, new Vector2(TileX * 16f + 24f, TileY * 16f));
                }
            }
            public void InnerUpdate_OnFail(bool clientOnly = false)
            {
                if (!clientOnly && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return;
                }
                string itemList = "";
                var source = new EntitySource_TileBreak(TileX, TileY, "GoreNest_MPFail");
                foreach (var i in Items)
                {
                    int newItem = AequusItem.NewItemCloned(source, new Vector2(TileX * 16f + 32f, TileY * 16f - 20f), i);
                    Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: newItem, number2: 1f);
                    }
                    if (itemList != "")
                        itemList += ", ";
                    itemList += AequusText.ItemText(i.type);
                }
                if (!clientOnly)
                {
                    AequusText.Broadcast("ChatBroadcast.DemonSiegeFail", new Color(255, 210, 25, 255), itemList);
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
                    if (Main.netMode != NetmodeID.Server)
                    {
                        s.NetUpdate = -300;
                    }
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
                Items.Clear();
                for (int i = 0; i < itemCount; i++)
                {
                    Items.Add(ItemIO.Receive(reader, true, false));
                }
            }
        }

        public static Asset<Texture2D> AuraTexture { get; private set; }

        public static Dictionary<Point, EventSacrifice> Sacrifices { get; private set; }

        internal static Dictionary<int, SacrificeData> registeredSacrifices;
        internal static Dictionary<int, int> upgradeToOriginal;

        public const string ExtraScreenFilter = "Aequus:DemonSiegeFilter";

        public override int Music => MusicData.DemonSiegeEvent.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => "Assets/UI/BestiaryIcons/DemonSiege";

        public override void Load()
        {
            registeredSacrifices = new Dictionary<int, SacrificeData>();
            upgradeToOriginal = new Dictionary<int, int>();
            Sacrifices = new Dictionary<Point, EventSacrifice>();
            if (!Main.dedServ)
            {
                Filters.Scene[ExtraScreenFilter] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(1f, -0.46f, -0.2f), EffectPriority.High); ;
            }
            On.Terraria.Main.DrawUnderworldBackground += Main_DrawUnderworldBackground;
        }

        private void Main_DrawUnderworldBackground(On.Terraria.Main.orig_DrawUnderworldBackground orig, Main self, bool flat)
        {
            orig(self, flat);
            if (Filters.Scene[ExtraScreenFilter].Opacity > 0f)
            {
                Main.spriteBatch.Draw(Images.Pixel.Value, new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20), new Color(20, 2, 2, 180) * Filters.Scene[ExtraScreenFilter].Opacity);
            }
        }

        public override void Unload()
        {
            AuraTexture = null;
            registeredSacrifices?.Clear();
            registeredSacrifices = null;
            upgradeToOriginal?.Clear();
            upgradeToOriginal = null;
            Sacrifices?.Clear();
            Sacrifices = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            return player.Aequus().eventDemonSiege.X != 0;
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
                PacketHandler.Send((p) =>
                {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write((byte)player);
                    if (player != 255)
                    {
                        InnerWritePlayerSpecificRequest(Main.player[player], p);
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
        public static void InnerWritePlayerSpecificRequest(Player player, BinaryWriter writer)
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

        public static void HandleStartRequest(BinaryReader reader)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            byte player = reader.ReadByte();
            if (player != 255)
            {
                InnerReadPlayerSpecificRequest(reader);
            }
            var s = new EventSacrifice(x, y);
            var sacrifice = new Item();
            ItemIO.Receive(sacrifice, reader, readStack: true, readFavorite: false);
            s.Items.Add(sacrifice);
            Sacrifices.Add(new Point(x, y), s);

            if (Main.netMode == NetmodeID.Server)
            {
                PacketHandler.Send((p) =>
                {
                    p.Write((ushort)x);
                    p.Write((ushort)y);
                    p.Write(player);
                    if (player != 255)
                    {
                        InnerWritePlayerSpecificRequest(Main.player[player], p);
                    }
                    ItemIO.Send(sacrifice, p, writeStack: true, writeFavorite: false);
                }, PacketType.RequestDemonSiege, ignore: player);
            }
        }
        public static void InnerReadPlayerSpecificRequest(BinaryReader reader)
        {
        }

        /// <summary>
        /// Finds and returns the closest demon siege
        /// </summary>
        /// <returns></returns>
        public static Point FindDemonSiege(Vector2 location)
        {
            foreach (var s in Sacrifices)
            {
                if (Vector2.Distance(location, new Vector2(s.Value.TileX * 16f + 24f, s.Value.TileY * 16f)) < s.Value.Range)
                {
                    return s.Key;
                }
            }
            return Point.Zero;
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
                        PacketHandler.Send((packet) =>
                        {
                            packet.Write((ushort)p.X);
                            packet.Write((ushort)p.Y);
                        }, PacketType.RemoveDemonSiege);
                    }
                }
                RemovalQueue.Clear();
            }

            public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
            {
                layers.Insert(0, new LegacyGameInterfaceLayer("Aequus: DemonSiege", DrawDemonSiegeRange, InterfaceScaleType.Game));
            }
            public bool DrawDemonSiegeRange()
            {
                foreach (var v in GoreNestTile.RenderPoints)
                {
                    if (Sacrifices.TryGetValue(v, out var sacrifice) && sacrifice._auraScale > 0f)
                    {
                        if (AuraTexture == null)
                        {
                            AuraTexture = ModContent.Request<Texture2D>("Aequus/Assets/GoreNestAura");
                            return true;
                        }
                        if (AuraTexture.Value == null)
                        {
                            return true;
                        }
                        var texture = AuraTexture.Value;
                        var origin = texture.Size() / 2f;
                        var drawCoords = (sacrifice.Center - Main.screenPosition).Floor();
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
        }
        public class DemonSiegeScene : ModSceneEffect
        {
            public override SceneEffectPriority Priority => SceneEffectPriority.Event;

            public override bool IsSceneEffectActive(Player player)
            {
                return true;
            }

            public override void SpecialVisuals(Player player)
            {
                var invasion = player.Aequus().eventDemonSiege;
                if (invasion.X != 0)
                {
                    if (!Filters.Scene[ExtraScreenFilter].Active)
                    {
                        Filters.Scene.Activate(ExtraScreenFilter, invasion.ToWorldCoordinates(), null);
                    }
                }
                else
                {
                    if (Filters.Scene[ExtraScreenFilter].Active)
                    {
                        Filters.Scene.Deactivate(ExtraScreenFilter);
                    }
                }
            }
        }
    }
}