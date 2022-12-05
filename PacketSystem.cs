using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Biomes.Glimmer;
using Aequus.Buffs.Debuffs;
using Aequus.Common;
using Aequus.Content;
using Aequus.Content.AnalysisQuests;
using Aequus.Content.CarpenterBounties;
using Aequus.Content.DronePylons;
using Aequus.Content.Necromancy;
using Aequus.Items.Consumables;
using Aequus.Items.Tools.Camera;
using Aequus.NPCs.Boss;
using Aequus.NPCs.Friendly.Town;
using Aequus.Projectiles.Misc;
using Aequus.Tiles;
using Aequus.Tiles.Furniture;
using Aequus.Tiles.Furniture.Gravity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus
{
    public class PacketSystem : ModSystem
    {
        private static HashSet<PacketType> logPacketType;

        public static ModPacket NewPacket => Aequus.Instance.GetPacket();

        public static List<Rectangle> TileCoatingSync { get; private set; }

        public override void Load()
        {
            logPacketType = new HashSet<PacketType>()
            {
                PacketType.Unused,
                PacketType.SpawnHostileOccultist,
                PacketType.PhysicsGunBlock,
                PacketType.RequestGlimmerEvent,
                PacketType.GlimmerStatus,
                PacketType.RemoveDemonSiege,
                PacketType.ExporterQuestsCompleted,
                PacketType.SpawnOmegaStarite,
                PacketType.StartDemonSiege,
                PacketType.RequestAnalysisQuest,
                PacketType.SpawnShutterstockerClip,
                PacketType.SpawnPixelCameraClip,
                PacketType.PlacePixelPainting,
            };
            TileCoatingSync = new List<Rectangle>();
        }

        public override void OnWorldLoad()
        {
            TileCoatingSync.Clear();
        }

        public override void OnWorldUnload()
        {
            TileCoatingSync.Clear();
        }

        public override void PostUpdateEverything()
        {
            if (TileCoatingSync.Count > 0)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    TileCoatingSync.Clear();
                    return;
                }

                try
                {
                    //Send((p) =>
                    //{
                    //    AequusTileData.SendSquare(p, TileCoatingSync[0]);
                    //}, PacketType.AequusTileSquare);
                }
                catch
                {
                    // split into 4 rectangles
                    var r = TileCoatingSync[0];
                    int halfW1 = r.Width / 2;
                    int halfW2 = r.Width - halfW1;
                    int halfH1 = r.Height / 2;
                    int halfH2 = r.Height - halfH1;
                    TileCoatingSync.Add(new Rectangle(r.X, r.Y, halfW1, halfH1));
                    TileCoatingSync.Add(new Rectangle(r.X + halfW1, r.Y, halfW2, halfH1));
                    TileCoatingSync.Add(new Rectangle(r.X, r.Y + halfH1, halfW1, halfH2));
                    TileCoatingSync.Add(new Rectangle(r.X + halfW1, r.Y + halfH1, halfW2, halfH2));
                }
                TileCoatingSync.RemoveAt(0);
            }
        }

        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (msgType == MessageID.TileSquare)
            {
                TileCoatingSync.Add(new Rectangle(number, (int)number2, (int)number3, (int)number4));
            }
            return false;
        }

        public static void Send(PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
        }

        public static void Send(Func<ModPacket, bool> func, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            if (func(packet))
                packet.Send(to, ignore);
        }

        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
        }

        public static void SyncSound(SoundPacket soundID, Vector2 location)
        {
            var p = Aequus.GetPacket(PacketType.SyncSound);
            p.Write((byte)soundID);
            p.Write(location.X);
            p.Write(location.Y);
            p.Send();
        }

        public static void LegacyFlaggedWrite(bool flag, Action<ModPacket> writeAction, ModPacket p)
        {
            p.Write(flag);
            if (flag)
            {
                writeAction(p);
            }
        }

        public static void SyncNecromancyOwner(int npc, int player)
        {
            Send((p) =>
                {
                    p.Write(npc);
                    p.Write(player);
                },
                PacketType.SyncNecromancyOwner);
        }

        public static void WriteNullableItem(Item item, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (item != null)
            {
                writer.Write(true);
                ItemIO.Send(item, writer, writeStack, writeFavorite);
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item ReadNullableItem(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item();
                ItemIO.Receive(item, reader, readStack, readFavorite);
                return item;
            }
            else
            {
                return null;
            }
        }

        public static void WriteNullableItemList(Item[] items, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (items != null)
            {
                writer.Write(true);
                if (items.Length < 0 || items.Length > byte.MaxValue)
                {
                    throw new Exception("Length of item list is invalid, must not go below 0 nor be greater than 255");
                }
                writer.Write((byte)items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    WriteNullableItem(items[i], writer, writeStack, writeFavorite);
                }
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item[] ReadNullableItemList(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item[reader.ReadByte()];
                for (int i = 0; i < item.Length; i++)
                {
                    item[i] = ReadNullableItem(reader, readStack, readFavorite);
                }
                return item;
            }
            else
            {
                return null;
            }
        }

        public static PacketType ReadPacketType(BinaryReader reader)
        {
            return (PacketType)reader.ReadByte();
        }

        public static void SyncNPC(NPC npc)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void HandlePacket(BinaryReader reader)
        {
            var type = ReadPacketType(reader);

            var l = Aequus.Instance.Logger;
            if (logPacketType.Contains(type))
            {
                l.Debug("Recieving Packet: " + type);
            }
            switch (type)
            {
                case PacketType.PlacePixelPainting:
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            var map = MapTileCache.NetReceive(reader);
                            if (WorldGen.InWorld(x, y) && !TileEntity.ByPosition.ContainsKey(new Point16(x, y)))
                            {
                                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEPixelPainting>());
                                if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out var te) && te is TEPixelPainting painting)
                                {
                                    painting.mapCache = map;
                                    NetMessage.SendData(MessageID.TileEntitySharing, number: painting.ID, number2: painting.Position.X, number3: painting.Position.Y);
                                }
                            }
                        }
                    }
                    break;

                case PacketType.SpawnHostileOccultist:
                    {
                        OccultistHostile.CheckSpawn(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    }
                    break;

                case PacketType.GravityChestPickupEffect:
                    {
                        var itemPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        int left = reader.ReadInt32();
                        int top = reader.ReadInt32();
                        if (Main.netMode != NetmodeID.Server)
                        {
                            GravityChestTile.ItemPickupEffect(itemPos, new Vector2(left * 16f + 16f, top * 16f + 16f));
                        }
                    }
                    break;

                case PacketType.PhysicsGunBlock:
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int plr = reader.ReadInt32();
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            if (!Main.tile[x, y].IsFullySolid() || Main.tileFrameImportant[Main.tile[x, y].TileType] || PhysicsGunProj.TilePickupBlacklist.Contains(Main.tile[x, y].TileType) || !WorldGen.CanKillTile(x, y))
                            {
                                return;
                            }
                            WorldGen.KillTile(x, y, noItem: true);
                            NetMessage.SendTileSquare(-1, x, y, 3);
                            Aequus.GetPacket(PacketType.PhysicsGunBlock).Send(toClient: plr);
                        }
                        else
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].ModProjectile is PhysicsGunProj physGun)
                                {
                                    physGun.realBlock = true;
                                }
                            }
                        }
                    }
                    break;

                case PacketType.RequestGlimmerEvent:
                    {
                        if (!GlimmerBiome.EventActive)
                        {
                            GlimmerSystem.BeginEvent();
                        }
                        else
                        {
                            GlimmerSystem.SendGlimmerStatus();
                        }
                    }
                    break;

                case PacketType.ZombieConvertEffects:
                    {
                        NecromancyNPC.ConvertEffects(new Vector2(reader.ReadSingle(), reader.ReadSingle()), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    }
                    break;

                case PacketType.AnalysisRarity:
                    {
                        int rare = reader.ReadInt32();
                        int value = reader.ReadInt32();
                        if (AnalysisSystem.RareTracker.TryGetValue(rare, out var val))
                        {
                            val.highestValueObtained = Math.Max(val.highestValueObtained, value);
                        }
                        else
                        {
                            AnalysisSystem.RareTracker[rare] = new TrackedItemRarity() { rare = rare, highestValueObtained = value };
                        }
                    }
                    break;

                case PacketType.SpawnPixelCameraClip:
                    {
                        int player = reader.ReadInt32();
                        int i = Item.NewItem(Main.player[player].GetSource_ItemUse_WithPotentialAmmo(Main.player[player].HeldItem, Main.player[player].HeldItem.useAmmo), Main.player[player].getRect(),
                            ModContent.ItemType<PixelCameraClip>());
                        if (i == -1)
                        {
                            return;
                        }
                        Main.item[i].ModItem<PixelCameraClip>().photoState = reader.ReadInt32();
                        Main.item[i].ModItem<PixelCameraClip>().NetReceive(reader);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                    }
                    break;
                case PacketType.SpawnShutterstockerClip:
                    {
                        int player = reader.ReadInt32();
                        int i = Item.NewItem(Main.player[player].GetSource_ItemUse_WithPotentialAmmo(Main.player[player].HeldItem, Main.player[player].HeldItem.useAmmo), Main.player[player].getRect(),
                            ModContent.ItemType<ShutterstockerClip>());
                        if (i == -1)
                        {
                            return;
                        }
                        Main.item[i].ModItem<ShutterstockerClip>().NetReceive(reader);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                    }
                    break;

                case PacketType.RequestAnalysisQuest:
                    {
                        int player = reader.ReadInt32();
                        var modPlayer = Main.player[player].GetModPlayer<AnalysisPlayer>();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int completed = reader.ReadInt32();
                            modPlayer.RefreshQuest(completed);
                            var quest = modPlayer.quest;
                            if (quest.isValid)
                            {
                                var p = Aequus.GetPacket(PacketType.RequestAnalysisQuest);
                                p.Write(player);
                                quest.NetSend(p);
                                p.Send(toClient: player);
                            }
                        }
                        else
                        {
                            modPlayer.quest = QuestInfo.NetRecieve(reader);
                            if (player == Main.myPlayer && Physicist.awaitQuest > 0)
                            {
                                Physicist.QuestButtonPressed();
                            }
                        }
                    }
                    break;

                case PacketType.RequestChestItems:
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int player = reader.ReadInt32();
                            int chestID = reader.ReadInt32();
                            if (Main.chest[chestID] != null)
                            {
                                var p = Aequus.GetPacket(PacketType.RequestChestItems);
                                p.Write(chestID);
                                for (int i = 0; i < Chest.maxItems; i++)
                                {
                                    ItemIO.Send(Main.chest[chestID].item[i], p, writeStack: true);
                                }
                                p.Send(toClient: player);
                            }
                        }
                        else
                        {
                            int chestID = reader.ReadInt32();
                            if (Main.chest[chestID].item == null)
                            {
                                Main.chest[chestID].item = new Item[Chest.maxItems];
                            }
                            for (int i = 0; i < Chest.maxItems; i++)
                            {
                                Main.chest[chestID].item[i] = ItemIO.Receive(reader, readStack: true);
                            }
                        }
                    }
                    break;

                case PacketType.ApplyNameTagToNPC:
                    {
                        NameTag.ApplyNametagToNPC(reader.ReadInt32(), reader.ReadString());
                    }
                    break;

                case PacketType.OnKillEffect:
                    {
                        var player = Main.player[reader.ReadInt32()];
                        var info = EnemyKillInfo.ReceiveData(reader);
                        player.Aequus().OnKillEffect(info);
                    }
                    break;

                case PacketType.AequusTileSquare:
                    {
                        AequusTileData.ReadSquare(reader);
                    }
                    break;

                case PacketType.CarpenterBountiesCompleted:
                    {
                        Main.player[reader.ReadInt32()].GetModPlayer<CarpenterBountyPlayer>().RecieveClientChanges(reader);
                    }
                    break;

                case PacketType.RequestTileSectionFromServer:
                    {
                        int plr = reader.ReadInt32();
                        int sectionX = reader.ReadInt32();
                        int sectionY = reader.ReadInt32();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendSection(plr, sectionX, sectionY);
                        }
                    }
                    break;

                case PacketType.SyncDronePoint:
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();

                        DroneWorld.RecievePacket(reader, new Point(x, y));
                    }
                    break;

                case PacketType.SyncNecromancyNPC:
                    {
                        Main.npc[reader.ReadByte()].GetGlobalNPC<NecromancyNPC>().Receive(reader);
                    }
                    break;

                case PacketType.SpawnOmegaStarite:
                    NPC.SpawnBoss(reader.ReadInt32(), reader.ReadInt32() - 1600, ModContent.NPCType<OmegaStarite>(), reader.ReadInt32());
                    AequusWorld.downedEventCosmic = true;
                    break;

                case PacketType.ExporterQuestsCompleted:
                    ExporterQuests.QuestsCompleted = reader.ReadUInt16();
                    break;

                case PacketType.GlimmerStatus:
                    GlimmerSystem.ReadGlimmerStatus(reader);
                    break;

                case PacketType.RemoveDemonSiege:
                    DemonSiegeSystem.ActiveSacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
                    break;

                case PacketType.StartDemonSiege:
                    DemonSiegeSystem.ReceiveStartRequest(reader);
                    break;

                case PacketType.DemonSiegeSacrificeStatus:
                    DemonSiegeSacrifice.ReceiveStatus(reader);
                    break;

                case PacketType.SyncSound:
                    {
                        var soundID = (SoundPacket)reader.ReadByte();
                        var position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        if (Main.netMode == NetmodeID.Server)
                        {
                            SyncSound(soundID, position);
                            break;
                        }
                        switch (soundID)
                        {
                            case SoundPacket.InflictBleeding:
                                SoundEngine.PlaySound(BattleAxeBleeding.InflictDebuffSound, position);
                                break;

                            case SoundPacket.InflictBurning:
                                SoundEngine.PlaySound(BlueFire.InflictDebuffSound, position);
                                break;

                            case SoundPacket.InflictBurning2:
                                SoundEngine.PlaySound(BlueFire.InflictDebuffSound.WithPitch(-0.2f), position);
                                break;

                            case SoundPacket.InflictNightfall:
                                SoundEngine.PlaySound(NightfallDebuff.InflictDebuffSound, position);
                                break;
                        }
                    }
                    break;

                case PacketType.SyncAequusPlayer:
                    {
                        if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus))
                        {
                            aequus.RecieveChanges(reader);
                        }
                    }
                    break;

                case PacketType.SyncNecromancyOwner:
                    {
                        int npc = reader.ReadInt32();
                        Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}