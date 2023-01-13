using Ionic.Zlib;
using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Biomes.Glimmer;
using Aequus.Buffs.Debuffs;
using Aequus.Common;
using Aequus.Content.AnalysisQuests;
using Aequus.Content.Carpentery;
using Aequus.Content.DronePylons;
using Aequus.Content.ExporterQuests;
using Aequus.Content.Necromancy;
using Aequus.Items.Consumables;
using Aequus.NPCs.Boss;
using Aequus.NPCs.Friendly.Town;
using Aequus.Projectiles.Misc;
using Aequus.Projectiles.Summon;
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
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Items.Misc.Carpentry.Rewards;
using Aequus.Items.Misc.Carpentry;
using Aequus.Projectiles.Magic;
using Aequus.Items.Accessories.Summon;
using Aequus.Items.Accessories.Debuff;

namespace Aequus
{
    public class PacketSystem : ModSystem
    {
        private static HashSet<PacketType> logPacketType;

        public static ModPacket NewPacket => Aequus.Instance.GetPacket();

        public static Point[] playerTilePosCache;

        public override void Load()
        {
            logPacketType = new HashSet<PacketType>()
            {
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
                PacketType.RegisterPhotoClip,
                PacketType.AddBuilding,
                PacketType.RemoveBuilding,
            };
            playerTilePosCache = new Point[Main.maxPlayers];
        }

        public void ClearWorld()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                playerTilePosCache[i] = Point.Zero;
            }
        }

        public override void OnWorldLoad()
        {
            ClearWorld();
        }

        public override void OnWorldUnload()
        {
            ClearWorld();
        }

        public override void PostUpdateEverything()
        {
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
                case PacketType.WabbajackNecromancyKill:
                    {
                        int npc = reader.ReadInt32();
                        int player = reader.ReadInt32();
                        if (Main.npc[npc].active)
                            WabbajackProj.ButcherNPC(Main.npc[npc], player);
                    }
                    break;

                case PacketType.RemoveBuilding:
                    {
                        int bountyID = reader.ReadInt32();
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        CarpenterSystem.RemoveBuildingBuffLocation(bountyID, x, y, quiet: Main.netMode == NetmodeID.MultiplayerClient);
                    }
                    break;

                case PacketType.AddBuilding:
                    {
                        int bountyID = reader.ReadInt32();
                        var rectangle = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        CarpenterSystem.AddBuildingBuffLocation(bountyID, rectangle, quiet: Main.netMode == NetmodeID.MultiplayerClient);
                    }
                    break;

                case PacketType.BrainCauliflowerNecromancyKill:
                    {
                        int npc = reader.ReadInt32();
                        int player = reader.ReadInt32();
                        if (Main.npc[npc].active)
                            BrainCauliflowerBlast.ButcherNPC(Main.npc[npc], player);
                    }
                    break;

                case PacketType.RegisterPhotoClip:
                    //CarpenterSystem.RecieveClip(reader);
                    break;

                case PacketType.PumpinatorWindSpeed:
                    {
                        Main.windSpeedTarget = reader.ReadSingle();
                        Main.windSpeedCurrent = reader.ReadSingle();
                        Main.windCounter = reader.ReadInt32();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            if (Main.windSpeedCurrent > 0.6f)
                            {
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

                case PacketType.PlacePixelPainting:
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            long timeCreated = reader.ReadInt64();
                            var map = PixelPaintingData.NetReceive(reader);
                            if (WorldGen.InWorld(x, y) && !TileEntity.ByPosition.ContainsKey(new Point16(x, y)))
                            {
                                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEPixelPainting>());
                                if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out var te) && te is TEPixelPainting painting)
                                {
                                    painting.timeCreated = timeCreated;
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
                        int i = reader.ReadInt32();
                        var nameTag = reader.ReadString();
                        NameTag.ApplyNametagToNPC(i, nameTag);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var p = Aequus.GetPacket(PacketType.ApplyNameTagToNPC);
                            p.Write(i);
                            p.Write(nameTag);
                            p.Send();
                        }
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

                case PacketType.CompleteCarpenterBounty:
                    {
                        CarpenterSystem.CompleteCarpenterBounty(CarpenterSystem.BountiesByID[reader.ReadInt32()]);
                    }
                    break;
                case PacketType.ResetCarpenterBounties:
                    {
                        CarpenterSystem.ResetBounties();
                    }
                    break;
                case PacketType.CarpenterBountiesCompleted:
                    {
                        CarpenterSystem.ReceiveCompletedBounties(reader);
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
                        if (Main.netMode == NetmodeID.Server && DroneWorld.TryGetDroneData(x, y, out var drone))
                        {
                            drone.Sync();
                        }
                    }
                    break;

                case PacketType.SyncNecromancyNPC:
                    {
                        Main.npc[reader.ReadByte()].GetGlobalNPC<NecromancyNPC>().Receive(reader);
                    }
                    break;

                case PacketType.SpawnOmegaStarite:
                    if (!NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()))
                        NPC.SpawnBoss(reader.ReadInt32(), reader.ReadInt32() - 1600, ModContent.NPCType<OmegaStarite>(), reader.ReadInt32());
                    AequusWorld.downedEventCosmic = true;
                    break;

                case PacketType.ExporterQuestsCompleted:
                    ExporterQuestSystem.QuestsCompleted = reader.ReadUInt16();
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
                            case SoundPacket.BlackPhial:
                                BlackPhial.EmitSound(position);
                                break;

                            case SoundPacket.WarHorn:
                                WarHorn.EmitSound(position);
                                break;

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

                            case SoundPacket.InflictWeakness:
                                SoundEngine.PlaySound(BoneRingWeakness.InflictDebuffSound, position);
                                break;

                            case SoundPacket.InflictAetherFire:
                                SoundEngine.PlaySound(AethersWrath.InflictDebuffSound, position);
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