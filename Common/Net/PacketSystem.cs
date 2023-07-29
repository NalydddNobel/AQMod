using Aequus.Common;
using Aequus.Common.Carpentry;
using Aequus.Common.Net.Sounds;
using Aequus.Content.DronePylons;
using Aequus.Content.Events.DemonSiege;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Items.Misc;
using Aequus.Items.Tools.Cameras.MapCamera;
using Aequus.Items.Tools.Cameras.MapCamera.Clip;
using Aequus.Items.Tools.Cameras.MapCamera.Common;
using Aequus.Items.Tools.Cameras.MapCamera.Tile;
using Aequus.NPCs.BossMonsters.OmegaStarite;
using Aequus.NPCs.Town.OccultistNPC;
using Aequus.NPCs.Town.PhysicistNPC;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Projectiles.Misc;
using Aequus.Tiles.Furniture.Gravity;
using Aequus.Unused.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Net;

public partial class PacketSystem : ModSystem {
    private static HashSet<PacketType> logPacketType;

    public static ModPacket NewPacket => Aequus.Instance.GetPacket();

    private static Dictionary<PacketType, PacketHandler> handlerByLegacyType = new();

    public static void Register(PacketHandler handler) {
        if (handlerByLegacyType.ContainsKey(handler.LegacyPacketType)) {
            throw new Exception($"Handler of {handler.LegacyPacketType} was registered twice. ({handler.Mod.Name}, {handler.Name})");
        }

        handlerByLegacyType[handler.LegacyPacketType] = handler;
    }

    public override void Load() {
        logPacketType = new() {
            PacketType.SpawnHostileOccultist,
            PacketType.PhysicsGunBlock,
            PacketType.RequestGlimmerEvent,
            PacketType.GlimmerStatus,
            PacketType.RemoveDemonSiege,
            PacketType.Unused5,
            PacketType.SpawnOmegaStarite,
            PacketType.StartDemonSiege,
            PacketType.RequestAnalysisQuest,
            PacketType.SpawnShutterstockerClip,
            PacketType.SpawnPixelCameraClip,
            PacketType.PlacePixelPainting,
            PacketType.RegisterPhotoClip,
            PacketType.AddBuilding,
            PacketType.RemoveBuilding,
        };
    }

    public override void Unload() {
        handlerByLegacyType?.Clear();
    }

    public static void Send(PacketType type, int capacity = 256, int to = -1, int ignore = -1) {
        var packet = Aequus.Instance.GetPacket(capacity);
        packet.Write((byte)type);
    }

    public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1) {
        var packet = Aequus.Instance.GetPacket(capacity);
        packet.Write((byte)type);
        action(packet);
        packet.Send(to, ignore);
    }

    public static void SyncNecromancyOwner(int npc, int player) {
        var p = Aequus.GetPacket(PacketType.SyncNecromancyOwner);
        p.Write(npc);
        p.Write(player);
        p.Send();
    }

    public static PacketType ReadPacketType(BinaryReader reader) {
        return (PacketType)reader.ReadByte();
    }

    public static void SyncNPC(NPC npc) {
        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
    }

    public static void HandlePacket(BinaryReader reader, int whoAmI) {
        var type = ReadPacketType(reader);

        var l = Aequus.Instance.Logger;
        if (logPacketType.Contains(type)) {
            l.Debug("Recieving Packet: " + type);
        }

        switch (type) {
            case PacketType.SendDebuffFlatDamage: {
                    int npc = reader.ReadInt32();
                    var amt = reader.ReadByte();
                    if (Main.npc[npc].active)
                        Main.npc[npc].Aequus().debuffDamage = Math.Max(Main.npc[npc].Aequus().debuffDamage, amt);
                }
                break;

            case PacketType.Unused4: {
                }
                break;

            case PacketType.RemoveBuilding: {
                    int bountyID = reader.ReadInt32();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    CarpentrySystem.RemoveBuildingBuffLocation(bountyID, x, y, quiet: Main.netMode == NetmodeID.MultiplayerClient);
                }
                break;

            case PacketType.AddBuilding: {
                    int bountyID = reader.ReadInt32();
                    var rectangle = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    CarpentrySystem.AddBuildingBuffLocation(bountyID, rectangle, quiet: Main.netMode == NetmodeID.MultiplayerClient);
                }
                break;

            case PacketType.Unused3: {
                }
                break;

            case PacketType.RegisterPhotoClip:
                //CarpenterSystem.RecieveClip(reader);
                break;

            case PacketType.PumpinatorWindSpeed: {
                    Main.windSpeedTarget = reader.ReadSingle();
                    Main.windSpeedCurrent = reader.ReadSingle();
                    Main.windCounter = reader.ReadInt32();
                    if (Main.netMode == NetmodeID.Server) {
                        if (Main.windSpeedCurrent > 0.6f) {
                            Sandstorm.StartSandstorm();
                        }
                        var p = Aequus.GetPacket(PacketType.PumpinatorWindSpeed);
                        p.Write(Main.windSpeedTarget);
                        p.Write(Main.windSpeedCurrent);
                        p.Write(Main.windCounter);
                        p.Send();
                    }
                }
                break;

            case PacketType.PlacePixelPainting: {
                    if (Main.netMode == NetmodeID.Server) {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        long timeCreated = reader.ReadInt64();
                        var map = PixelPaintingData.NetReceive(reader);
                        if (WorldGen.InWorld(x, y) && !TileEntity.ByPosition.ContainsKey(new Point16(x, y))) {
                            TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEPixelPainting>());
                            if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out var te) && te is TEPixelPainting painting) {
                                painting.timeCreated = timeCreated;
                                painting.mapCache = map;
                                NetMessage.SendData(MessageID.TileEntitySharing, number: painting.ID, number2: painting.Position.X, number3: painting.Position.Y);
                            }
                        }
                    }
                }
                break;

            case PacketType.SpawnHostileOccultist: {
                    OccultistHostile.CheckSpawn(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                }
                break;

            case PacketType.GravityChestPickupEffect: {
                    var itemPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    int left = reader.ReadInt32();
                    int top = reader.ReadInt32();
                    if (Main.netMode != NetmodeID.Server) {
                        GravityChestTile.ItemPickupEffect(itemPos, new Vector2(left * 16f + 16f, top * 16f + 16f));
                    }
                }
                break;

            case PacketType.PhysicsGunBlock: {
                    if (Main.netMode == NetmodeID.Server) {
                        int plr = reader.ReadInt32();
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        if (!Main.tile[x, y].IsFullySolid() || Main.tileFrameImportant[Main.tile[x, y].TileType] || PhysicsGunProj.TilePickupBlacklist.Contains(Main.tile[x, y].TileType) || !WorldGen.CanKillTile(x, y)) {
                            return;
                        }
                        WorldGen.KillTile(x, y, noItem: true);
                        NetMessage.SendTileSquare(-1, x, y, 3);
                        Aequus.GetPacket(PacketType.PhysicsGunBlock).Send(toClient: plr);
                    }
                    else {
                        for (int i = 0; i < Main.maxProjectiles; i++) {
                            if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].ModProjectile is PhysicsGunProj physGun) {
                                physGun.realBlock = true;
                            }
                        }
                    }
                }
                break;

            case PacketType.RequestGlimmerEvent: {
                    if (!GlimmerZone.EventTechnicallyActive) {
                        GlimmerSystem.BeginEvent();
                    }
                    else {
                        GlimmerSystem.SendGlimmerStatus();
                    }
                }
                break;

            case PacketType.Unused: {
                }
                break;

            case PacketType.AnalysisRarity: {
                    int rare = reader.ReadInt32();
                    int value = reader.ReadInt32();
                    if (AnalysisSystem.RareTracker.TryGetValue(rare, out var val)) {
                        val.highestValueObtained = Math.Max(val.highestValueObtained, value);
                    }
                    else {
                        AnalysisSystem.RareTracker[rare] = new TrackedItemRarity() { rare = rare, highestValueObtained = value };
                    }
                }
                break;

            case PacketType.SpawnPixelCameraClip: {
                    int player = reader.ReadInt32();
                    int i = Item.NewItem(Main.player[player].GetSource_ItemUse_WithPotentialAmmo(Main.player[player].HeldItem, Main.player[player].HeldItem.useAmmo), Main.player[player].getRect(),
                        ModContent.ItemType<PixelCameraClip>());
                    if (i == -1) {
                        return;
                    }
                    Main.item[i].ModItem<PixelCameraClip>().photoState = reader.ReadInt32();
                    Main.item[i].ModItem<PixelCameraClip>().NetReceive(reader);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                }
                break;
            case PacketType.SpawnShutterstockerClip: {
                    int player = reader.ReadInt32();
                    int i = Item.NewItem(Main.player[player].GetSource_ItemUse_WithPotentialAmmo(Main.player[player].HeldItem, Main.player[player].HeldItem.useAmmo), Main.player[player].getRect(),
                        ModContent.ItemType<ShutterstockerClip>());
                    if (i == -1) {
                        return;
                    }
                    Main.item[i].ModItem<ShutterstockerClip>().NetReceive(reader);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                }
                break;

            case PacketType.RequestAnalysisQuest: {
                    int player = reader.ReadInt32();
                    var modPlayer = Main.player[player].GetModPlayer<AnalysisPlayer>();
                    if (Main.netMode == NetmodeID.Server) {
                        int completed = reader.ReadInt32();
                        modPlayer.RefreshQuest(completed);
                        var quest = modPlayer.quest;
                        if (quest.isValid) {
                            var p = Aequus.GetPacket(PacketType.RequestAnalysisQuest);
                            p.Write(player);
                            quest.NetSend(p);
                            p.Send(toClient: player);
                        }
                    }
                    else {
                        modPlayer.quest = QuestInfo.NetRecieve(reader);
                        if (player == Main.myPlayer && Physicist.awaitQuest > 0) {
                            Physicist.QuestButtonPressed();
                        }
                    }
                }
                break;

            case PacketType.RequestChestItems: {
                    if (Main.netMode == NetmodeID.Server) {
                        int player = reader.ReadInt32();
                        int chestID = reader.ReadInt32();
                        if (Main.chest[chestID] != null) {
                            var p = Aequus.GetPacket(PacketType.RequestChestItems);
                            p.Write(chestID);
                            for (int i = 0; i < Chest.maxItems; i++) {
                                ItemIO.Send(Main.chest[chestID].item[i], p, writeStack: true);
                            }
                            p.Send(toClient: player);
                        }
                    }
                    else {
                        int chestID = reader.ReadInt32();
                        if (Main.chest[chestID].item == null) {
                            Main.chest[chestID].item = new Item[Chest.maxItems];
                        }
                        for (int i = 0; i < Chest.maxItems; i++) {
                            Main.chest[chestID].item[i] = ItemIO.Receive(reader, readStack: true);
                        }
                    }
                }
                break;

            case PacketType.ApplyNameTagToNPC: {
                    int i = reader.ReadInt32();
                    var nameTag = reader.ReadString();
                    NameTag.ApplyNametagToNPC(i, nameTag);
                    if (Main.netMode == NetmodeID.Server) {
                        var p = Aequus.GetPacket(PacketType.ApplyNameTagToNPC);
                        p.Write(i);
                        p.Write(nameTag);
                        p.Send();
                    }
                }
                break;

            case PacketType.OnKillEffect: {
                    var player = Main.player[reader.ReadInt32()];
                    var info = EnemyKillInfo.ReceiveData(reader);
                    player.Aequus().OnKillEffect(info);
                }
                break;

            case PacketType.CompleteCarpenterBounty: {
                    CarpentrySystem.Complete(BuildChallengeLoader.registeredBuildChallenges[reader.ReadInt32()]);
                }
                break;
            case PacketType.ResetCarpenterBounties: {
                    CarpentrySystem.ResetBounties();
                }
                break;
            case PacketType.CarpenterBountiesCompleted: {
                    CarpentrySystem.ReceiveCompletedBounties(reader);
                }
                break;

            case PacketType.RequestTileSectionFromServer: {
                    int plr = reader.ReadInt32();
                    int sectionX = reader.ReadInt32();
                    int sectionY = reader.ReadInt32();
                    if (Main.netMode == NetmodeID.Server) {
                        NetMessage.SendSection(plr, sectionX, sectionY);
                    }
                }
                break;

            case PacketType.SyncDronePoint: {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();

                    DroneWorld.RecievePacket(reader, new Point(x, y));
                    if (Main.netMode == NetmodeID.Server && DroneWorld.TryGetDroneData(x, y, out var drone)) {
                        drone.Sync();
                    }
                }
                break;

            case PacketType.Unused2: {
                }
                break;

            case PacketType.SpawnOmegaStarite:
                if (!NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()))
                    NPC.SpawnBoss(reader.ReadInt32(), reader.ReadInt32() - 1600, ModContent.NPCType<OmegaStarite>(), reader.ReadInt32());
                AequusWorld.downedEventCosmic = true;
                break;

            case PacketType.Unused5:
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

            case PacketType.SyncSound: {
                    byte soundID = reader.ReadByte();
                    byte plr = reader.ReadByte();
                    NetSoundLoader.ByID(soundID)?.NetPlay(reader, plr);
                }
                break;

            case PacketType.SyncAequusPlayer: {
                    if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus)) {
                        aequus.RecieveChanges(reader);
                    }
                }
                break;

            case PacketType.SyncNecromancyOwner: {
                    byte npc = reader.ReadByte();
                    Main.npc[npc].Aequus().PlayerOwner = reader.ReadByte();
                }
                break;

            default:
                break;
        }

        if (handlerByLegacyType != null && handlerByLegacyType.TryGetValue(type, out var handler)) {
            handler.Receive(reader, whoAmI);
        }
    }

    public static T Get<T>() where T : PacketHandler {
        return ModContent.GetInstance<T>();
    }
}