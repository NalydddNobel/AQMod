using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Carpentry;
public class CarpentrySystem : ModSystem {
    public static readonly BuildChallengeSaveData CompletedBounties = new("CompletedBounties");
    public static readonly BuildBuffsData BuildingBuffs = new("BuildingBuffs_{0}");

    public override void Load() {
        CompletedBounties.Clear();
        BuildingBuffs.Clear();
    }

    public override void ClearWorld() {
        BuildingBuffs.Clear();
    }

    public override void SaveWorldData(TagCompound tag) {
        BuildingBuffs.SaveData(tag);
        BuildingBuffs.Clear();
    }

    public override void LoadWorldData(TagCompound tag) {
        CompletedBounties.LoadData(tag);
        BuildingBuffs.LoadData(tag);
    }

    public static bool AddBuildingBuffLocation(int buildChallengeId, Rectangle rectangle, bool quiet = false) {
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            var p = Aequus.GetPacket(PacketType.AddBuilding);
            p.Write(buildChallengeId);
            p.Write(rectangle.X);
            p.Write(rectangle.Y);
            p.Write(rectangle.Width);
            p.Write(rectangle.Height);
            p.Send();
        }
        return BuildingBuffs.AddBuild(buildChallengeId, rectangle);
    }

    public static bool RemoveBuildingBuffLocation(int bountyID, int x, int y, bool quiet = false) {
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            var p = Aequus.GetPacket(PacketType.RemoveBuilding);
            p.Write(bountyID);
            p.Write(x);
            p.Write(y);
            p.Send();
        }
        return BuildingBuffs.RemoveBuild(bountyID, x, y);
    }

    public override void NetSend(BinaryWriter writer) {
        SendCompletedBounties();
        BuildingBuffs.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader) {
        BuildingBuffs.NetReceive(reader);
    }

    public static void SendCompletedBounties() {
        var p = Aequus.GetPacket(PacketType.CarpenterBountiesCompleted);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            p.Send();
            return;
        }
        CompletedBounties.SendData(p);
        p.Send();
    }

    public static void ReceiveCompletedBounties(BinaryReader reader) {
        if (Main.netMode == NetmodeID.Server) {
            SendCompletedBounties();
            return;
        }

        CompletedBounties.RecieveData(reader);
    }

    public static void ResetBounties() {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            Aequus.GetPacket(PacketType.ResetCarpenterBounties).Send();
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            SendCompletedBounties();
        }
        else {
            CompletedBounties.TrimData();
        }
    }

    public static void Complete(BuildChallenge buildChallenge) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            var p = Aequus.GetPacket(PacketType.CompleteCarpenterBounty);
            p.Write(buildChallenge.Type);
            p.Send();
            return;
        }
        CompletedBounties.Add(buildChallenge);
        CompletedBounties.TrimData();
        if (Main.netMode == NetmodeID.Server) {
            SendCompletedBounties();
        }
    }
}