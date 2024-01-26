using Aequus.Content.Events.DemonSiege;
using Aequus.Core;
using Aequus.Core.Networking;
using Aequus.Old.Content.Events.DemonSiege.Spawners;
using Aequus.Old.Content.Events.DemonSiege.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Events.DemonSiege;

public class DemonSiegeSacrificeInfo {
    public int TileX { get; internal set; }
    public int TileY { get; internal set; }

    public Vector2 WorldCenter => new Vector2(TileX * 16f + 24f, TileY * 16f);

    public int MaxItems = 1;
    public float Range = 800f;
    public int TimeLeftMax = 3600;
    public int TimeLeft = 3600;
    public int PreStart = 300;
    public int NetUpdate;
    public byte player;

    public bool unholyCoreUsed;

    public readonly List<Item> Items;

    private float _auraScale;
    private bool _playedSound;
    internal bool _visible;

    public float AuraScale => MathF.Pow(_auraScale, 2f);
    public bool Renderable => _visible;

    public DemonSiegeSacrificeInfo(int x, int y) {
        TileX = x;
        TileY = y;
        Items = new List<Item>();
    }
    public void OnPlayerActivate(Player player) {
        //if (Items.ContainsAny((i) => i.type == ModContent.ItemType<VoidRing>()))
        //{
        //    SummonBoss1(voidRing: true);
        //}
        // else if
        if (Items.Exists((i) => i.type == ModContent.ItemType<UnholyCore>()) || player.ConsumeItem(ModContent.ItemType<UnholyCore>())) {
            unholyCoreUsed = true;
        }
    }
    public int DetermineLength() {
        int time = 0;
        foreach (var i in Items) {
            if (AltarSacrifices.OriginalToConversion.TryGetValue(i.netID, out var value)) {
                int newTime = 7200 * (int)(value.Progression + 1);
                if (unholyCoreUsed) {
                    newTime = (int)(newTime * 0.75f);
                }
                if (!NPC.downedBoss3) {
                    newTime *= 2;
                }
                time = Math.Max(time, newTime);
            }
        }
        return time;
    }

    public Rectangle ProtectedTiles() {
        return new Rectangle(TileX, TileY, 3, 4);
    }
    public bool OnValidTile() {
        return OblivionAltar.IsGoreNest(TileX, TileY);
    }

    public void Update() {
        if (!OnValidTile()) {
            InnerUpdate_OnFail();
            return;
        }
        if (!_playedSound) {
            _playedSound = true;
            if (Main.netMode != NetmodeID.Server) {
                if (player != 255) {
                    string text = Language.GetTextValueWith("Mods.Aequus.Announcement.DemonSiege.GiveItem." + Main.rand.Next(8), new {
                        Player = Main.player[player].name,
                        Item = Items[0].Name
                    });
                    // Runs on each individual client, not needing the server to send the message to everyone.
                    Main.NewText(text, DemonSiegeSystem.TextColor);
                }
                SoundEngine.PlaySound(AequusSounds.BeginDemonSiege, new Vector2(TileX * 16f + 24f, TileY * 16f));
            }
        }

        if (!Main.player[player].active) {
            player = 255;
        }

        if (PreStart > 0) {
            PreStart--;
            if (PreStart == 0) {
                InnerUpdate_OnStart();
            }
            return;
        }

        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetUpdate++;
            if (NetUpdate > 120 && Main.netMode == NetmodeID.Server) {
                Aequus.GetPacket<DemonSiegeStatusPacket>().Send(this);
                NetUpdate = 0;
            }
            // Timeout
            if (NetUpdate > 300) {
                InnerUpdate_OnFail(clientOnly: true);
                return;
            }
        }
        if (TimeLeft > 0 || DemonSiegeSystem.DemonSiegePause > 0) {
            if (_auraScale < 1f) {
                _auraScale += 0.02f - _auraScale * 0.0195f;
                if (_auraScale > 1f) {
                    _auraScale = 1f;
                }
            }
            if (Items.Count == 0) {
                InnerUpdate_OnEnd();
                return;
            }
            if (DemonSiegeSystem.DemonSiegePause <= 0) {
                var center = WorldCenter;
                InnerUpdate_TimeLeft(center);
            }
            return;
        }
        InnerUpdate_OnEnd();
    }
    public void InnerUpdate_OnStart() {
        TimeLeftMax = TimeLeft = DetermineLength();
        if (Main.netMode == NetmodeID.Server) {
            Aequus.GetPacket<DemonSiegeStatusPacket>().Send(this);
        }

        if (Main.netMode != NetmodeID.Server && player != 255) {
            string text = Language.GetTextValueWith("Mods.Aequus.Announcement.DemonSiege.EventStart." + Main.rand.Next(8), new {
                Player = Main.player[player].name,
                Item = Items[0].Name
            });
            // Runs on each individual client, not needing the server to send the message to everyone.
            Main.NewText(text, DemonSiegeSystem.TextColor);
        }
    }
    public void InnerUpdate_TimeLeft(Vector2 center) {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(center) < Range) {
                TimeLeft -= Helper.GetTimeScale();
                return;
            }
        }
        TimeLeft += Helper.GetTimeScale();
        if (TimeLeft > TimeLeftMax) {
            InnerUpdate_OnFail();
        }
    }
    public void InnerUpdate_OnEnd() {
        Vector2 itemSpawn = WorldCenter;
        itemSpawn.Y -= 20f;
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            var source = new EntitySource_TileBreak(TileX, TileY, "Aequus:GoreNest");
            //AequusText.Broadcast("Should be spawning these items: " + AequusText.ItemText(Items[0].type), Color.Red);
            foreach (var i in Items) {
                AltarSacrifices.OriginalToConversion.TryGetValue(i.type, out var value);
                if (i.type == value.NewItem) {
                    continue;
                }

                var item = value.Convert(i);
                int newItem = Item.NewItem(source, itemSpawn, item);
                Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                }
            }
            DemonSiegeSystem.SacrificeRemovalQueue.Add(new Point(TileX, TileY));

            // Mark this event as cleared, and initiate a Lantern Night tommorow night.
            NPC.SetEventFlagCleared(ref World.DownedDemonSiegeT1, -1);
        }
        if (Main.netMode != NetmodeID.Server) {
            for (int i = 0; i < 40; i++) {
                var d = Dust.NewDustPerfect(itemSpawn + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f, 100f), DustID.SilverFlame,
                    newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f), Scale: Main.rand.NextFloat(1f, 2.5f));
                d.velocity = (d.position - itemSpawn) / 20f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
            }
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, new Vector2(TileX * 16f + 24f, TileY * 16f));
        }
    }
    public void InnerUpdate_OnFail(bool clientOnly = false) {
        if (!clientOnly && Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        OnFail_PukeItems(clientOnly);
        OnFail_EatItems(clientOnly);

        DemonSiegeSystem.SacrificeRemovalQueue.Add(new Point(TileX, TileY));
    }
    public void OnFail_PukeItems(bool clientOnly) {
        string itemList = "";
        var source = new EntitySource_TileBreak(TileX, TileY, "GoreNest_MPFail");
        foreach (var i in Items) {
            if (AltarSacrifices.OriginalToConversion.TryGetValue(i.type, out var val) && val.OriginalItem == val.NewItem) {
                continue;
            }
            if (!clientOnly) {
                int newItem = Item.NewItem(source, new Vector2(TileX * 16f + 32f, TileY * 16f - 20f), i);
                Main.item[newItem].velocity += Main.rand.NextVector2Unit(-MathHelper.PiOver4 * 3f, MathHelper.PiOver2) * Main.rand.NextFloat(1f, 3f);
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: newItem, number2: 1f);
                }
            }
            if (itemList != "") {
                itemList += ", ";
            }

            itemList += i.Name;
        }
        if (!clientOnly && !string.IsNullOrEmpty(itemList)) {
            TextBroadcast.NewText("Mods.Aequus.Announcement.DemonSiege.Fail", DemonSiegeSystem.TextColor, itemList);
        }
    }
    public void OnFail_EatItems(bool clientOnly) {
        string itemList = "";
        foreach (var i in Items) {
            if (AltarSacrifices.OriginalToConversion.TryGetValue(i.type, out var val) && val.OriginalItem != val.NewItem/* || val.OriginalItem == ModContent.ItemType<VoidRing>()*/) {
                continue;
            }
            if (itemList != "") {
                itemList += ", ";
            }

            itemList += ChatCommandInserts.ItemCommand(i.type);
        }
        if (!clientOnly && !string.IsNullOrEmpty(itemList)) {
            TextBroadcast.NewText("Mods.Aequus.Announcement.DemonSiege.FailEat", new Color(255, 210, 25, 255), itemList);
        }
    }
    public void SummonBoss1(bool voidRing) {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            var worldPosition = WorldCenter;
            //NPC.SpawnBoss((int)worldPosition.X, (int)worldPosition.Y, ModContent.NPCType<Arcubus>(), Player.FindClosest(worldPosition, 2, 2));
            DemonSiegeSystem.DemonSiegePause = 120;
        }
        TimeLeft = 0;
        PreStart = 0;
    }

    public class DemonSiegeStatusPacket : PacketHandler {
        public void Send(DemonSiegeSacrificeInfo sacrifice) {
            ModPacket p = GetPacket();
            p.Write((ushort)sacrifice.TileX);
            p.Write((ushort)sacrifice.TileY);
            p.Write((ushort)sacrifice.PreStart);
            p.Write((ushort)sacrifice.TimeLeft);
            p.Write((ushort)sacrifice.TimeLeftMax);
            p.Write((byte)sacrifice.MaxItems);
            p.Write(sacrifice.Range);
            p.Write(sacrifice.player);
            p.Write((byte)sacrifice.Items.Count);
            for (int i = 0; i < sacrifice.Items.Count; i++) {
                ItemIO.Send(sacrifice.Items[i], p, true, false);
            }
            p.Send();
        }

        public override void Receive(BinaryReader reader, int sender) {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            DemonSiegeSacrificeInfo sacrifice;
            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(new Point(x, y), out var value)) {
                sacrifice = value;
                if (Main.netMode != NetmodeID.Server) {
                    sacrifice.NetUpdate = -300;
                }
            }
            else {
                sacrifice = new DemonSiegeSacrificeInfo(x, y);
                DemonSiegeSystem.ActiveSacrifices.Add(new Point(x, y), sacrifice);
            }

            sacrifice.PreStart = reader.ReadUInt16();
            sacrifice.TimeLeft = reader.ReadUInt16();
            sacrifice.TimeLeftMax = reader.ReadUInt16();
            sacrifice.MaxItems = reader.ReadByte();
            sacrifice.Range = reader.ReadSingle();
            sacrifice.player = reader.ReadByte();
            int itemCount = reader.ReadByte();
            sacrifice.Items.Clear();
            for (int i = 0; i < itemCount; i++) {
                sacrifice.Items.Add(ItemIO.Receive(reader, true, false));
            }
        }
    }
}