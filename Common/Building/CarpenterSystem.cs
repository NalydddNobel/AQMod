using Aequus.Common.DataSets;
using Aequus.Common.Tiles;
using Aequus.Content.Building.old.Quest.Bounties;
using Aequus.Content.Building.Passes.Steps;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Building {
    public class CarpenterSystem : ModSystem {
        internal static List<CarpenterBounty> BountiesByID;
        internal static Dictionary<string, CarpenterBounty> BountiesByName;

        public static Dictionary<int, List<Rectangle>> BuildingBuffLocations { get; private set; }

        public static int BountyCount => BountiesByID.Count;

        public static List<string> CompletedBounties { get; private set; }

        public override void Load() {
            BuildingBuffLocations = new();
            CompletedBounties = new();
        }

        public override void SetupContent() {
            BountiesLoader.SetupBounties();
        }

        public override void ClearWorld() {
            BuildingBuffLocations?.Clear();
            CompletedBounties?.Clear();
        }

        public override void SaveWorldData(TagCompound tag) {
            tag["CompletedBounties"] = CompletedBounties;
            CheckBuildingBuffsInWorld();
            foreach (var pair in BuildingBuffLocations) {
                if (BuildingBuffLocations.Count > 0) {
                    if (pair.Value.Count > 0)
                        tag[$"Buildings_{BountiesByID[pair.Key].Name}"] = pair.Value;
                }
            }
        }

        public override void LoadWorldData(TagCompound tag) {
            CompletedBounties = tag.Get<List<string>>("CompletedBounties");
            CompletedBounties ??= new List<string>();
            foreach (var b in BountiesByID) {
                if (tag.TryGet<List<Rectangle>>($"Buildings_{b.Name}", out var value)) {
                    for (int i = 0; i < value.Count; i++)
                        AddBuildingBuffLocation(b.Type, value[i], quiet: true);
                }
            }
        }

        public static List<Point> TurnRectangleIntoUnoptimizedPointMess(Rectangle rectangle) {
            var p = new List<Point>();
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++) {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++) {
                    p.Add(new Point(i, j));
                }
            }
            return p;
        }
        public static Rectangle TurnPointMessIntoRectangleBounds(List<Point> points) {
            var r = new Rectangle();
            foreach (var p in points) {
                r.Width = Math.Max(r.Width, p.X);
                r.Height = Math.Max(r.Height, p.Y);
            }
            r.X = r.Width;
            r.Y = r.Height;
            foreach (var p in points) {
                r.X = Math.Min(r.X, p.X);
                r.Y = Math.Min(r.Y, p.Y);
            }
            r.Width -= r.X;
            r.Height -= r.Y;
            return r;
        }

        public override void Unload() {
            BountiesByID?.Clear();
            BountiesByName?.Clear();
        }

        public static void CheckBuildingBuffsInWorld() {
            foreach (var pair in BuildingBuffLocations) {
                if (BuildingBuffLocations.Count > 0) {
                    var b = BountiesByID[pair.Key];
                    for (int i = 0; i < pair.Value.Count; i++) {
                        try {
                            if (!b.CheckConditions(new StepInfo(pair.Value[i])).Success()) {
                                pair.Value.RemoveAt(i);
                                i--;
                            }
                        }
                        catch (Exception ex) {
                            Aequus.Instance.Logger.Error($"Error from {b.FullName} when evaluating.");
                            Aequus.Instance.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
            }
        }

        public static void ScanForBuilderBuffs(Rectangle r) {
            var map = new TileMapCache(r);

            foreach (var b in BountiesByID) {
                if (b.BuildingBuff <= 0) {
                    return;
                }
                var result = b.CheckConditions(new StepInfo(map, r));
                //Main.NewText($"{b.Name}:{result.success}");
                if (result.Success()) {
                    AddBuildingBuffLocation(b.Type, r);
                }
                else {
                    RemoveBuildingBuffLocation(b.Type, r.X, r.Y);
                }
            }
        }

        public static void AddBuildingBuffLocation(int bountyID, Rectangle rectangle, bool quiet = false) {
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
                var p = Aequus.GetPacket(PacketType.AddBuilding);
                p.Write(bountyID);
                p.Write(rectangle.X);
                p.Write(rectangle.Y);
                p.Write(rectangle.Width);
                p.Write(rectangle.Height);
                p.Send();
            }
            if (!BuildingBuffLocations.ContainsKey(bountyID)) {
                BuildingBuffLocations[bountyID] = new List<Rectangle>() { rectangle };
                return;
            }
            var l = BuildingBuffLocations[bountyID];
            bool addRectangle = true;
            for (int i = 0; i < l.Count; i++) {
                if (l[i].Intersects(rectangle)) {
                    var r = l[i];
                    r.X = Math.Min(rectangle.X, r.X);
                    r.Y = Math.Min(rectangle.Y, r.Y);
                    int maxX = Math.Max(rectangle.X + rectangle.Width, l[i].X + l[i].Width);
                    int maxY = Math.Max(rectangle.Y + rectangle.Height, l[i].Y + l[i].Height);
                    r.Width = maxX - r.X;
                    r.Height = maxY - r.Y;
                    l[i] = r;
                    rectangle = r;
                    if (!addRectangle) {
                        l.RemoveAt(i);
                        i--;
                    }
                    addRectangle = false;
                }
            }
            if (addRectangle)
                BuildingBuffLocations[bountyID].Add(rectangle);
        }

        public static void RemoveBuildingBuffLocation(int bountyID, int x, int y, bool quiet = false) {
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
                var p = Aequus.GetPacket(PacketType.RemoveBuilding);
                p.Write(bountyID);
                p.Write(x);
                p.Write(y);
                p.Send();
            }
            if (!BuildingBuffLocations.ContainsKey(bountyID)) {
                return;
            }
            var l = BuildingBuffLocations[bountyID];
            lock (l) {
                for (int i = 0; i < l.Count; i++) {
                    if (l[i].Contains(x, y)) {
                        l.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static int RegisterBounty(CarpenterBounty bounty) {
            if (BountiesByID == null || BountiesByName == null) {
                BountiesByID = new List<CarpenterBounty>();
                BountiesByName = new Dictionary<string, CarpenterBounty>();
            }

            string name = bounty.FullName;
            if (BountiesByName.ContainsKey(name))
                throw new Exception($"{bounty.Mod.Name} added two bounties with the same name: ({bounty.Name})");
            if (bounty.ItemReward == 0 || bounty.ItemReward >= ItemLoader.ItemCount || bounty.ItemStack <= 0)
                throw new Exception($"{bounty.Mod.Name} added a bounty ({bounty.Name}) without a reward.");
            bounty.Type = BountyCount;
            BountiesByID.Add(bounty);
            BountiesByName.Add(name, bounty);
            return bounty.Type;
        }

        public static bool TryGetBounty(string fullName, out CarpenterBounty bounty) {
            return BountiesByName.TryGetValue(fullName, out bounty);
        }
        public static bool TryGetBounty(string mod, string name, out CarpenterBounty bounty) {
            return TryGetBounty($"{mod}/{name}", out bounty);
        }
        public static bool TryGetBounty(Mod mod, string name, out CarpenterBounty bounty) {
            return TryGetBounty(mod.Name, name, out bounty);
        }

        public static CarpenterBounty GetBounty(int type) {
            return BountiesByID[type];
        }

        public static CarpenterBounty GetBounty(string fullName) {
            return BountiesByName[fullName];
        }

        public static CarpenterBounty GetBounty(string mod, string name) {
            return GetBounty($"{mod}/{name}");
        }

        public static CarpenterBounty GetBounty(Mod mod, string name) {
            return GetBounty(mod.Name, name);
        }

        public static CarpenterBounty GetBounty<T>() where T : CarpenterBounty {
            return ModContent.GetInstance<T>();
        }

        public static List<Point> FindWallTiles(TileMapCache map, int x, int y) {
            var addPoints = new List<Point>();
            var checkedPoints = new List<Point>() { new Point(x, y) };
            var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
            for (int k = 0; k < 1000; k++) {
                checkedPoints.AddRange(addPoints);
                addPoints.Clear();
                bool addedAny = false;
                if (checkedPoints.Count > 1000) {
                    return checkedPoints;
                }
                for (int l = 0; l < checkedPoints.Count; l++) {
                    for (int m = 0; m < offsets.Length; m++) {
                        var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                        if (map.InSceneRenderedMap(newPoint.X, newPoint.Y) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) && map[newPoint].WallType != WallID.None && Main.wallHouse[map[newPoint].WallType]) {
                            var slopeType = SlopeType.Solid;
                            if (offsets[m].X != 0) {
                                slopeType = map[newPoint].Slope;
                                if (TileID.Sets.Platforms[map[newPoint].TileType]) {
                                    if (map[newPoint].TileFrameX == 144) {
                                        slopeType = SlopeType.SlopeDownLeft;
                                    }
                                    if (map[newPoint].TileFrameX == 180) {
                                        slopeType = SlopeType.SlopeDownRight;
                                    }
                                }
                            }

                            if (!map[newPoint].HasTile || (!map[newPoint].IsSolid || map[newPoint].IsSolidTop) && (!map[newPoint].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor) || slopeType != SlopeType.Solid)) {
                                for (int n = 0; n < offsets.Length; n++) {
                                    var checkWallPoint = newPoint + offsets[n];
                                    if (!map.InSceneRenderedMap(checkWallPoint) || !map[checkWallPoint].IsFullySolid && (map[checkWallPoint].WallType == WallID.None || !Main.wallHouse[map[checkWallPoint].WallType])) {
                                        return new List<Point>() { new Point(x, y) };
                                    }
                                }
                                addPoints.Add(newPoint);
                                addedAny = true;
                            }
                        }
                    }
                }
                if (!addedAny) {
                    return checkedPoints;
                }
            }
            return checkedPoints;
        }

        public override void NetSend(BinaryWriter writer) {
            SendCompletedBounties();
            int count = 0;
            foreach (var b in BuildingBuffLocations.Values) {
                if (b.Count > 0)
                    count++;
            }
            writer.Write(count);
            foreach (var pair in BuildingBuffLocations) {
                var b = pair.Value;
                if (b.Count < 0) {
                    continue;
                }
                var l = b;
                writer.Write(pair.Key);
                writer.Write(l.Count);
                for (int j = 0; j < l.Count; j++) {
                    writer.Write(l[j].X);
                    writer.Write(l[j].Y);
                    writer.Write(l[j].Width);
                    writer.Write(l[j].Height);
                }
            }
        }

        public override void NetReceive(BinaryReader reader) {
            int buildingBuffLocationsCount = reader.ReadInt32();
            BuildingBuffLocations?.Clear();
            for (int i = 0; i < buildingBuffLocationsCount; i++) {
                int type = reader.ReadInt32();
                int listCount = reader.ReadInt32();
                for (int j = 0; j < listCount; j++) {
                    var r = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    AddBuildingBuffLocation(type, r, quiet: true);
                }
            }
        }

        public static void SendCompletedBounties() {
            var p = Aequus.GetPacket(PacketType.CarpenterBountiesCompleted);
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                p.Send();
                return;
            }
            TrimCompletedBounties();

            var l = new List<int>();
            for (int i = 0; i < CompletedBounties.Count; i++) {
                if (BountiesByName.ContainsKey(CompletedBounties[i]))
                    l.Add(BountiesByName[CompletedBounties[i]].Type);
            }

            p.Write(l.Count);
            for (int i = 0; i < CompletedBounties.Count; i++) {
                p.Write(l[i]);
            }
            p.Send();
        }

        public static void ReceiveCompletedBounties(BinaryReader reader) {
            if (Main.netMode == NetmodeID.Server) {
                SendCompletedBounties();
                return;
            }
            CompletedBounties?.Clear();
            int amt = reader.ReadInt32();
            var bounties = new List<int>();
            for (int i = 0; i < amt; i++) {
                var b = BountiesByID[reader.ReadInt32()];
                CompletedBounties.Add(b.FullName);
            }
        }

        public static void ResetBounties() {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                Aequus.GetPacket(PacketType.ResetCarpenterBounties).Send();
                return;
            }
            CompletedBounties?.Clear();
            if (Main.netMode == NetmodeID.Server) {
                SendCompletedBounties();
            }
            else {
                TrimCompletedBounties();
            }
        }

        public static void CompleteCarpenterBounty(CarpenterBounty b) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                var p = Aequus.GetPacket(PacketType.CompleteCarpenterBounty);
                p.Write(b.Type);
                p.Send();
                return;
            }
            CompletedBounties.Add(b.FullName);
            TrimCompletedBounties();
            if (Main.netMode == NetmodeID.Server)
                SendCompletedBounties();
        }

        public static void TrimCompletedBounties() {
            for (int i = 0; i < CompletedBounties.Count - 1; i++) {
                for (int j = i + 1; j < CompletedBounties.Count; j++) {
                    if (CompletedBounties[i] == CompletedBounties[j]) {
                        CompletedBounties.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}