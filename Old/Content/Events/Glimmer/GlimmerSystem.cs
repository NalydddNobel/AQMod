using Aequus.Core;
using Aequus.Core.Networking;
using Aequus.Old.Content.Events.Glimmer.Peaceful;
using System;
using System.IO;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Events.Glimmer;
public class GlimmerSystem : ModSystem {
    public static int EndEventDelay { get; set; }

    public override void Load() {
        On_Main.UpdateTime_StartNight += GlimmerNightTransition;
    }

    public static void DeleteFallenStarsWithin(int x) {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.FallingStar && Main.projectile[i].Center.X > x * 16f - 1000f && Main.projectile[i].Center.X < x * 16f + 1000f) {
                Main.projectile[i].damage = 0;
                Main.projectile[i].noDropItem = true;
                Main.projectile[i].Kill();
            }
        }
    }

    public override void PreUpdatePlayers() {
        if (GlimmerSceneEffect.cantTouchThis > 0) {
            GlimmerSceneEffect.cantTouchThis--;
        }

        if (GlimmerZone.EventTechnicallyActive) {
            bool endEvent = Main.dayTime;
            if (EndEventDelay > 0) {
                EndEventDelay--;
                if (EndEventDelay <= 0) {
                    endEvent = true;
                }
            }
            PeacefulGlimmerZone.TileLocationX = 0;
            if (endEvent) {
                if (EndEvent() && Main.netMode != NetmodeID.MultiplayerClient) {
                    WorldGen.BroadcastText(NetworkText.FromKey("Mods.Aequus.Announcement.Glimmer.End"), CommonColor.TEXT_EVENT);
                }
                return;
            }

            var x = GlimmerZone.TileLocation.X;
            if (GlimmerZone.TileLocation.Y == -1 || GlimmerZone.TileLocation.Y == (int)Main.worldSurface) {
                GlimmerZone.TileLocation = CheckGround(GlimmerZone.TileLocation);
            }
            else if (TileHelper.IsSectionLoaded(GlimmerZone.TileLocation)) {
                if (!Main.tile[GlimmerZone.TileLocation].IsSolid()) {
                    GlimmerZone.TileLocation = CheckGround(GlimmerZone.TileLocation);
                }
            }
            GlimmerZone.TileLocation = CheckGround(GlimmerZone.TileLocation);
            DeleteFallenStarsWithin(GlimmerZone.TileLocation.X);
            if (Main.netMode == NetmodeID.Server && GlimmerZone.TileLocation.X != x) {
                SendGlimmerStatus();
            }
        }
        else {
            EndEventDelay = 0;
        }
        if (PeacefulGlimmerZone.EventActive) {
            DeleteFallenStarsWithin(PeacefulGlimmerZone.TileLocationX);
        }

        if (Main.dayTime || Main.bloodMoon || Main.snowMoon || Main.pumpkinMoon) {
            EndEvent();
            PeacefulGlimmerZone.TileLocationX = 0;
        }
    }

    public override void PostUpdateEverything() {
        if (GlimmerZone.omegaStarite != -1 && (!Main.npc[GlimmerZone.omegaStarite].active || !Main.npc[GlimmerZone.omegaStarite].boss)) {
            GlimmerZone.omegaStarite = -1;
        }
    }

    public static void BeginEvent(Point where) {
        GlimmerZone.TileLocation = CheckGround(where);

        WorldGen.BroadcastText(NetworkText.FromKey($"Mods.Aequus.Announcement.Glimmer.Start{(where.X * 2 > Main.maxTilesX ? "East" : "West")}"), CommonColor.TEXT_EVENT);
        if (Main.netMode != NetmodeID.SinglePlayer) {
            SendGlimmerStatus();
        }
    }

    public static bool BeginEvent() {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Aequus.GetPacket<RequestRandomGlimmerEventPacket>().Send();
            return false;
        }

        for (int i = 0; i < 1000; i++) {
            int x = Main.rand.Next(200, Main.maxTilesX - 200);
            if (Math.Abs(x - Main.spawnTileX) < GlimmerZone.SuperStariteTile) {
                continue;
            }

            if (i < 100) {
                bool nearBed = false;
                for (int j = 0; j < Main.maxPlayers; j++) {
                    if (Main.player[j].active && !Main.player[j].dead && Main.player[j].GetSpawnY() <= Main.worldSurface) {
                        //AequusText.Broadcast($"{j}/{(x - Main.player[j].GetSpawnX()).Abs()}", Color.Red);
                        if (Math.Abs(x - Main.player[j].GetSpawnX()) <= GlimmerZone.HyperStariteTile) {
                            nearBed = true;
                            break;
                        }
                    }
                }
                if (nearBed) {
                    continue;
                }
            }
            //AequusText.Broadcast((x - Main.spawnTileX).Abs().ToString(), Main.DiscoColor);
            BeginEvent(new Point(x, -1));
            return true;
        }
        return false;
    }

    public static bool EndEvent() {
        if (!GlimmerZone.EventTechnicallyActive) {
            return false;
        }

        EndEventDelay = 0;
        GlimmerZone.TileLocation = Point.Zero;
        if (Main.netMode == NetmodeID.Server) {
            SendGlimmerStatus();
        }
        return true;
    }

    public static int GetTileDistance(Player player) {
        return Math.Abs((int)((player.position.X + player.width) / 16 - GlimmerZone.TileLocation.X));
    }

    public static Point CheckGround(Point p) {
        ushort min = (ushort)(90 * (Main.maxTilesY / (WorldGen.WorldSizeSmallY / 2)));
        min -= 50;

        if (Main.remixWorld) {
            if (TileHelper.ScanDown(new Point(p.X, Main.UnderworldLayer + 40), 50, out var airPoint, TileHelper.IsNotSolid, TileHelper.HasNoLiquid)) {
                min = (ushort)airPoint.Y;
            }
            else {
                min = (ushort)(Main.UnderworldLayer + 40);
            }
        }

        p.Y = Math.Max(p.Y, min);

        for (ushort j = min; j <= Main.worldSurface; j++) {
            if (!TileHelper.IsSectionLoaded(p.X, j)) {
                continue;
            }

            if (Main.tile[p.X, j].IsSolid() || Main.tile[p.X, j].HasAnyLiquid()) {
                p.Y = j;
                break;
            }
        }
        if (p.Y != (int)Main.worldSurface) {
            for (ushort j = (ushort)p.Y; j > min; j--) {
                for (ushort k = 0; k < 10; k++) {
                    if (!TileHelper.IsSectionLoaded(p.X, j - k)) {
                        continue;
                    }

                    if (Main.tile[p.X, j - k].IsSolid()) {
                        goto FoundInvalidSpot;
                    }
                }
                p.Y = j + 1;
                return p;

            FoundInvalidSpot:
                continue;
            }
            return p;
        }
        p.Y = (ushort)Main.worldSurface;
        return p;
    }

    public static void ResetWorldData() {
        GlimmerZone.TileLocation = Point.Zero;
        PeacefulGlimmerZone.TileLocationX = 0;
        EndEventDelay = 0;
    }

    public override void OnWorldLoad() {
        ResetWorldData();
    }

    public override void OnWorldUnload() {
        ResetWorldData();
    }

    public override void SaveWorldData(TagCompound tag) {
        if (PeacefulGlimmerZone.EventActive) {
            tag["PeacefulGlimmerX"] = PeacefulGlimmerZone.TileLocationX;
        }
        if (!GlimmerZone.EventActive) {
            return;
        }
        tag["GlimmerX"] = GlimmerZone.TileLocation.X;
        tag["GlimmerY"] = GlimmerZone.TileLocation.Y;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("PeacefulGlimmerX", out int peacefulX)) {
            PeacefulGlimmerZone.TileLocationX = peacefulX;
        }

        if (tag.TryGet("GlimmerX", out int x) && tag.TryGet("GlimmerY", out int y)) {
            GlimmerZone.TileLocation = new Point(x, y);
        }
    }

    public override void NetSend(BinaryWriter writer) {
        var bb = new BitsByte(GlimmerZone.EventActive, PeacefulGlimmerZone.EventActive);
        writer.Write(bb);
        if (bb[0]) {
            writer.Write((ushort)GlimmerZone.TileLocation.X);
            writer.Write((ushort)GlimmerZone.TileLocation.Y);
        }
        if (bb[1]) {
            writer.Write((ushort)PeacefulGlimmerZone.TileLocationX);
        }
    }

    public override void NetReceive(BinaryReader reader) {
        var bb = (BitsByte)reader.ReadByte();
        if (bb[0]) {
            GlimmerZone.TileLocation = new Point(reader.ReadUInt16(), reader.ReadUInt16());
        }
        if (bb[1]) {
            PeacefulGlimmerZone.TileLocationX = reader.ReadUInt16();
        }
    }

    public override void PostUpdateTime() {
        if (!CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled && GlimmerZone.EventActive) {
            int playersInTimeWound = 0;
            int maxPlayers = 0;
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && !Main.player[i].dead) {
                    maxPlayers++;
                    if (Main.player[i].Distance(GlimmerZone.TileLocation.ToWorldCoordinates()) < 250f * 16f) {
                        playersInTimeWound++;
                    }
                }
            }
            if (maxPlayers > 0 && playersInTimeWound / (float)maxPlayers > 0.5f) {
                Main.time -= 2.0d * CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
                if (Main.time <= 1.0d) {
                    Main.time = 1.0d;
                }
            }
        }
    }

    public static void SendGlimmerStatus() {
        Aequus.GetPacket<GlimmerEventLocationPacket>().Send();
    }

    public static void ReadGlimmerStatus(BinaryReader r) {
        if (r.ReadBoolean()) {
            GlimmerZone.TileLocation = new Point(r.ReadUInt16(), r.ReadUInt16());
            return;
        }
        GlimmerZone.TileLocation = Point.Zero;
    }

    private static void GlimmerNightTransition(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents) {
        orig(ref stopEvents);
    }

    public static void OnTransitionToNight(ref bool stopEvents) {
        int chance = 9;
        if (Main.tenthAnniversaryWorld) {
            chance = 6;
        }

        if (Main.GetMoonPhase() == MoonPhase.Full || !NPC.downedBoss2 || Main.bloodMoon) {
            return;
        }

        if (!WorldGen.spawnEye && Main.rand.NextBool(chance)) {
            for (int i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].active && Main.player[i].ConsumedLifeCrystals > 1) {
                    BeginEvent();
                    break;
                }
            }
        }

        if (GlimmerZone.EventTechnicallyActive) {
            Main.sundialCooldown = 0;
            Main.moondialCooldown = 0;
            stopEvents = true;
        }
        else {
            if ((WorldState.DownedCosmicBoss || WorldState.DownedTrueCosmicBoss) && Main.rand.NextBool()) {
                PeacefulGlimmerZone.TileLocationX = Main.rand.Next(100, Main.maxTilesX - 100);
            }
        }
    }
}

public class GlimmerEventLocationPacket : PacketHandler {
    public void Send() {
        ModPacket p = GetPacket();
        p.Write(GlimmerZone.EventActive);
        if (GlimmerZone.EventActive) {
            p.Write((ushort)GlimmerZone.TileLocation.X);
            p.Write((ushort)GlimmerZone.TileLocation.Y);
        }
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (reader.ReadBoolean()) {
            GlimmerZone.TileLocation = new Point(reader.ReadUInt16(), reader.ReadUInt16());
        }

        if (Main.netMode == NetmodeID.Server) {
            Send();
        }
    }
}

public class RequestRandomGlimmerEventPacket : PacketHandler {
    public void Send() {
        GetPacket().Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        GlimmerSystem.BeginEvent();
    }
}