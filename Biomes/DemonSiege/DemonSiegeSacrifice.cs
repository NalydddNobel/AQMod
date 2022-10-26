using Aequus.Items;
using Aequus.Items.Boss.Summons;
using Aequus.NPCs.Boss;
using Aequus.Particles.Dusts;
using Aequus.Tiles.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Biomes.DemonSiege
{
    public class DemonSiegeSacrifice
    {
        public int TileX { get; internal set; }
        public int TileY { get; internal set; }

        public Vector2 WorldCenter => new Vector2(TileX * 16f + 24f, TileY * 16f);

        public int MaxItems = 1;
        public float Range = 800f;
        public int TimeLeftMax = 3600;
        public int TimeLeft = 3600;
        public int PreStart = 300;
        public int NetUpdate;

        public bool unholyCoreUsed;

        public readonly List<Item> Items;

        public float _auraScale;
        public bool _playedSound;

        public DemonSiegeSacrifice(int x, int y)
        {
            TileX = x;
            TileY = y;
            Items = new List<Item>();
        }
        public void OnPlayerActivate(Player player)
        {
            //if (Items.ContainsAny((i) => i.type == ModContent.ItemType<VoidRing>()))
            //{
            //    SummonBoss1(voidRing: true);
            //}
            // else if
            if (Items.ContainsAny((i) => i.type == ModContent.ItemType<UnholyCore>()) || player.ConsumeItem(ModContent.ItemType<UnholyCore>()))
            {
                unholyCoreUsed = true;
            }
        }
        public int DetermineLength()
        {
            int time = 600;
            foreach (var i in Items)
            {
                if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.netID, out var value))
                {
                    int newTime = 3600 * (int)(value.Progression + 1);
                    if (!unholyCoreUsed)
                    {
                        newTime = (int)(newTime * 1.33f);
                    }
                    if (!NPC.downedBoss3)
                    {
                        newTime *= 2;
                    }
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
            return GoreNestTile.IsGoreNest(TileX, TileY);
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
                    PacketSystem.Send((p) =>
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
            if (TimeLeft > 0 || DemonSiegeSystem.DemonSiegePause > 0)
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
                if (DemonSiegeSystem.DemonSiegePause <= 0)
                {
                    var center = WorldCenter;
                    InnerUpdate_TimeLeft(center);
                }
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
            if (!AequusWorld.downedArcubus)
            {
                SummonBoss1(voidRing: false);
                return;
            }
            Vector2 itemSpawn = WorldCenter;
            itemSpawn.Y -= 20f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = new EntitySource_TileBreak(TileX, TileY, "Aequus:GoreNest");
                //AequusText.Broadcast("Should be spawning these items: " + AequusText.ItemText(Items[0].type), Color.Red);
                foreach (var i in Items)
                {
                    DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.type, out var value);
                    if (i.type == value.NewItem)
                        continue;

                    var item = value.Convert(i);
                    int newItem = AequusItem.NewItemCloned(source, itemSpawn, item);
                    Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }
                DemonSiegeSystem.SacrificeRemovalQueue.Add(new Point(TileX, TileY));

                AequusWorld.downedEventDemon = true;
                NetMessage.SendData(MessageID.WorldData);
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 40; i++)
                {
                    var d = Dust.NewDustPerfect(itemSpawn + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f, 100f), ModContent.DustType<MonoSparkleDust>(),
                        newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f), Scale: Main.rand.NextFloat(1f, 2.5f));
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

            OnFail_PukeItems(clientOnly);
            OnFail_EatItems(clientOnly);

            DemonSiegeSystem.SacrificeRemovalQueue.Add(new Point(TileX, TileY));
        }
        public void OnFail_PukeItems(bool clientOnly)
        {
            string itemList = "";
            var source = new EntitySource_TileBreak(TileX, TileY, "GoreNest_MPFail");
            foreach (var i in Items)
            {
                if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.type, out var val) && val.OriginalItem == val.NewItem)
                {
                    continue;
                }
                if (!clientOnly)
                {
                    int newItem = AequusItem.NewItemCloned(source, new Vector2(TileX * 16f + 32f, TileY * 16f - 20f), i);
                    Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: newItem, number2: 1f);
                    }
                }
                if (itemList != "")
                    itemList += ", ";
                itemList += AequusText.ItemCommand(i.type);
            }
            if (!clientOnly && !string.IsNullOrEmpty(itemList))
            {
                AequusText.Broadcast("ChatBroadcast.DemonSiegeFail", new Color(255, 210, 25, 255), itemList);
            }
        }
        public void OnFail_EatItems(bool clientOnly)
        {
            string itemList = "";
            foreach (var i in Items)
            {
                if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.type, out var val) && (val.OriginalItem != val.NewItem/* || val.OriginalItem == ModContent.ItemType<VoidRing>()*/))
                {
                    continue;
                }
                if (itemList != "")
                    itemList += ", ";
                itemList += AequusText.ItemCommand(i.type);
            }
            if (!clientOnly && !string.IsNullOrEmpty(itemList))
            {
                AequusText.Broadcast("ChatBroadcast.DemonSiegeFailEat", new Color(255, 210, 25, 255), itemList);
            }
        }
        public void SummonBoss1(bool voidRing)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var worldPosition = WorldCenter;
                //NPC.SpawnBoss((int)worldPosition.X, (int)worldPosition.Y, ModContent.NPCType<Arcubus>(), Player.FindClosest(worldPosition, 2, 2));
                DemonSiegeSystem.DemonSiegePause = 120;
            }
            TimeLeft = 0;
            PreStart = 0;
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

        public static void ReceiveStatus(BinaryReader reader)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            DemonSiegeSacrifice s;
            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(new Point(x, y), out var value))
            {
                s = value;
                if (Main.netMode != NetmodeID.Server)
                {
                    s.NetUpdate = -300;
                }
            }
            else
            {
                s = new DemonSiegeSacrifice(x, y);
                DemonSiegeSystem.ActiveSacrifices.Add(new Point(x, y), s);
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
}