using Aequus.Buffs.Buildings;
using Aequus.Content.CarpenterBounties.Steps;
using Aequus.Items.Placeable;
using Aequus.Items.Tools;
using Aequus.Items.Tools.Camera;
using Aequus.Items.Tools.Misc;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterSystem : ModSystem
    {
        internal static List<CarpenterBounty> BountiesByID;
        internal static Dictionary<string, CarpenterBounty> BountiesByName;

        public static Dictionary<int, bool> CraftableTileLookup { get; private set; }

        public static Dictionary<int, List<Rectangle>> BuildingBuffLocations { get; private set; }

        public static int BountyCount => BountiesByID.Count;

        public static List<string> CompletedBounties;

        public override void Load()
        {
            CraftableTileLookup = new Dictionary<int, bool>();
            BuildingBuffLocations = new Dictionary<int, List<Rectangle>>();
            CompletedBounties = new List<string>();
        }

        public override void SetupContent()
        {
            new CarpenterBounty("FountainBounty")
                .SetReward<AdvancedRuler>()
                .SetBuff<FountainBountyBuff>()
                .AddStep(new WaterfallSearchStep(liquidWanted: LiquidID.Water)
                    .AfterSuccess((i, s) =>
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = i.GetInterest<WaterfallSearchStep.WaterfallInterest>().resultRectangle))
                .AddStep(new WaterfallHeightStep(minHeight: 7))
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0f)
                    .AfterSuccess((i, s) =>
                    {
                        var crafted = i.GetInterest<CraftableTilesStep.Interest>();
                        var symmetric = i.GetInterest<SymmetricHorizontalStep.Interest>();
                        symmetric.givenRectangle = crafted.resultRectangle;
                        symmetric.givenPoints = crafted.craftableTiles;
                    }))
                .AddStep(new SymmetricHorizontalStep())
                .Register();

            new CarpenterBounty("PirateShipBounty")
                .SetReward<WhiteFlag>()
                .SetBuff<PirateBountyBuff>()
                .AddStep(new FindHousesStep(minHouses: 2)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<WaterLineStep.Interest>().givenHouses = houses.housingWalls;
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = houses.housingWalls;
                    }))
                .AddStep(new WaterLineStep(minWaterLine: 5))
                .AddStep(new FurnitureCountStep(minFurniture: 15))
                .Register();

            new CarpenterBounty("BiomePaletteBounty")
                .SetReward<OmniPaint>()
                .SetBuff<PaletteBountyBuff>()
                .AddStep(new FindHousesStep(minHouses: 1)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<BiomePaletteStep.Interest>().givenHouses = houses.housingWalls;
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = houses.housingWalls;
                    }))
                .AddStep(new FurnitureCountStep(minFurniture: 5))
                .AddStep(new BiomePaletteStep(minCredit: 0.5f))
                .Register();

            new CarpenterBounty("PondBridgeBounty")
                .SetReward<FishSign>()
                .SetBuff<BridgeBountyBuff>()
                .AddStep(new FindBridgeStep(waterTilesNeeded: 50, waterHeightNeeded: 4, liquidIDWanted: LiquidID.Water, bridgeLengthWanted: 12)
                    .AfterSuccess((i, s) =>
                    {
                        var bridge = i.GetInterest<FindBridgeStep.Interest>();
                        var dict = new Dictionary<Point, List<Point>>() { [new Point(bridge.bridgeLocation.X, bridge.bridgeLocation.Y)] = TurnRectangleIntoUnoptimizedPointMess(bridge.bridgeLocation) };
                        i.GetInterest<FurnitureCountStep.Interest>().givenHouses = dict;
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = bridge.bridgeLocation;
                    }))
                .AddStep(new FurnitureCountStep(minFurniture: 8))
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0f)
                    .AfterSuccess((i, s) =>
                    {
                        var crafted = i.GetInterest<CraftableTilesStep.Interest>();
                        var symmetric = i.GetInterest<SymmetricHorizontalStep.Interest>();
                        symmetric.givenRectangle = crafted.resultRectangle;
                        symmetric.givenPoints = crafted.craftableTiles;
                    }))
                .AddStep(new SymmetricHorizontalStep())
                .Register();

            new CarpenterBounty("ActuatorDoorBounty")
                .SetReward<PixelCamera>()
                .AddMiscUnlock<PixelCameraClipAmmo>()
                .AddStep(new FindHousesStep(minHouses: 1)
                    .AfterSuccess((i, s) =>
                    {
                        var houses = i.GetInterest<FindHousesStep.Interest>();
                        i.GetInterest<ActuatorDoorStep.Interest>().givenHouses = houses.housingWalls;
                        var mess = houses.housingWalls.Count > 0 ? houses.housingWalls.Values.First() : new List<Point>() { new Point(0, 0), };
                        var r = TurnPointMessIntoRectangleBounds(mess); 
                        r.Inflate(1, 1);
                        i.GetInterest<CraftableTilesStep.Interest>().givenRectangle = r;
                    }))
                .AddStep(new ActuatorDoorStep())
                .AddStep(new CraftableTilesStep(minTiles: 12, ratioTiles: 0.5f))
                .Register();
        }

        public override void OnWorldLoad()
        {
            BuildingBuffLocations?.Clear();
            CompletedBounties?.Clear();
        }

        public override void OnWorldUnload()
        {
            BuildingBuffLocations?.Clear();
            CompletedBounties?.Clear();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["CompletedBounties"] = CompletedBounties;
            foreach (var pair in BuildingBuffLocations)
            {
                if (BuildingBuffLocations.Count > 0)
                {
                    var b = BountiesByID[pair.Key];
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        if (!b.CheckConditions(new StepInfo(pair.Value[i])).success)
                        {
                            pair.Value.RemoveAt(i);
                            i--;
                        }
                    }
                    if (pair.Value.Count > 0)
                        tag[$"Buildings_{b.Name}"] = pair.Value;
                }
                continue;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CompletedBounties = tag.Get<List<string>>("CompletedBounties");
            if (CompletedBounties == null)
            {
                CompletedBounties = new List<string>();
            }
            foreach (var b in BountiesByID)
            {
                if (tag.TryGet<List<Rectangle>>($"Buildings_{b.Name}", out var value))
                {
                    for (int i = 0; i < value.Count; i++)
                        AddBuildingBuffLocation(b.Type, value[i], quiet: true);
                }
            }
        }

        public static List<Point> TurnRectangleIntoUnoptimizedPointMess(Rectangle rectangle)
        {
            var p = new List<Point>();
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
            {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                {
                    p.Add(new Point(i, j));
                }
            }
            return p;
        }
        public static Rectangle TurnPointMessIntoRectangleBounds(List<Point> points)
        {
            var r = new Rectangle();
            foreach (var p in points)
            {
                r.Width = Math.Max(r.Width, p.X);
                r.Height = Math.Max(r.Height, p.Y);
            }
            r.X = r.Width;
            r.Y = r.Height;
            foreach (var p in points)
            {
                r.X = Math.Min(r.X, p.X);
                r.Y = Math.Min(r.Y, p.Y);
            }
            r.Width -= r.X;
            r.Height -= r.Y;
            return r;
        }

        public override void Unload()
        {
            BountiesByID?.Clear();
            BountiesByName?.Clear();
            CraftableTileLookup?.Clear();
        }

        public static void ScanForBuilderBuffs(Rectangle r)
        {
            var map = new TileMapCache(r);
            
            foreach (var b in BountiesByID)
            {
                if (b.BuildingBuff <= 0)
                {
                    return;
                }
                var result = b.CheckConditions(new StepInfo(map, r));
                //Main.NewText($"{b.Name}:{result.success}");
                if (result.success)
                {
                    AddBuildingBuffLocation(b.Type, r);
                }
                else
                {
                    RemoveBuildingBuffLocation(b.Type, r.X, r.Y);
                }
            }
        }

        public static void AddBuildingBuffLocation(int bountyID, Rectangle rectangle, bool quiet = false)
        {
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet)
            {
                var p = Aequus.GetPacket(PacketType.AddBuilding);
                p.Write(bountyID);
                p.Write(rectangle.X);
                p.Write(rectangle.Y);
                p.Write(rectangle.Width);
                p.Write(rectangle.Height);
                p.Send();
            }
            if (!BuildingBuffLocations.ContainsKey(bountyID))
            {
                BuildingBuffLocations[bountyID] = new List<Rectangle>() { rectangle };
                return;
            }
            var l = BuildingBuffLocations[bountyID];
            bool addRectangle = true;
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].Intersects(rectangle))
                {
                    var r = l[i];
                    r.X = Math.Min(rectangle.X, r.X);
                    r.Y = Math.Min(rectangle.Y, r.Y);
                    int maxX = Math.Max(rectangle.X + rectangle.Width, l[i].X + l[i].Width);
                    int maxY = Math.Max(rectangle.Y + rectangle.Height, l[i].Y + l[i].Height);
                    r.Width = maxX - r.X;
                    r.Height = maxY - r.Y;
                    l[i] = r;
                    rectangle = r;
                    if (!addRectangle)
                    {
                        l.RemoveAt(i);
                        i--;
                    }
                    addRectangle = false;
                }
            }
            if (addRectangle)
                BuildingBuffLocations[bountyID].Add(rectangle);
        }

        public static void RemoveBuildingBuffLocation(int bountyID, int x, int y, bool quiet = false)
        {
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet)
            {
                var p = Aequus.GetPacket(PacketType.RemoveBuilding);
                p.Write(bountyID);
                p.Write(x);
                p.Write(y);
                p.Send();
            }
            if (!BuildingBuffLocations.ContainsKey(bountyID))
            {
                return;
            }
            var l = BuildingBuffLocations[bountyID];
            lock (l)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].Contains(x, y))
                    {
                        l.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static int RegisterBounty(CarpenterBounty bounty)
        {
            if (BountiesByID == null || BountiesByName == null)
            {
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

        public static bool IsTileIDCraftable(int tileID)
        {
            if (CraftableTileLookup.TryGetValue(tileID, out var val))
            {
                return val;
            }

            foreach (var rec in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.createTile == tileID))
            {
                foreach (var i in rec.requiredItem)
                {
                    foreach (var rec2 in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.type == i.type))
                    {
                        foreach (var i2 in rec2.requiredItem)
                        {
                            if (i2.type == rec.createItem.type)
                            {
                                goto Continue;
                            }
                        }
                    }
                }
                CraftableTileLookup.Add(tileID, true);
                return true;

            Continue:
                continue;
            }
            CraftableTileLookup.Add(tileID, false);
            return false;
        }

        public static bool TryGetBounty(string mod, string name, out CarpenterBounty bounty)
        {
            return BountiesByName.TryGetValue(mod + "/" + name, out bounty);
        }

        public static CarpenterBounty GetBounty(int type)
        {
            return BountiesByID[type];
        }

        public static CarpenterBounty GetBounty(string mod, string name)
        {
            return BountiesByName[mod + "/" + name];
        }

        public static CarpenterBounty GetBounty(Mod mod, string name)
        {
            return GetBounty(mod.Name, name);
        }

        public static CarpenterBounty GetBounty<T>() where T : CarpenterBounty
        {
            return ModContent.GetInstance<T>();
        }

        public static List<Point> FindWallTiles(TileMapCache map, int x, int y)
        {
            var addPoints = new List<Point>();
            var checkedPoints = new List<Point>() { new Point(x, y) };
            var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
            for (int k = 0; k < 1000; k++)
            {
                checkedPoints.AddRange(addPoints);
                addPoints.Clear();
                bool addedAny = false;
                if (checkedPoints.Count > 1000)
                {
                    return checkedPoints;
                }
                for (int l = 0; l < checkedPoints.Count; l++)
                {
                    for (int m = 0; m < offsets.Length; m++)
                    {
                        var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                        if (map.InSceneRenderedMap(newPoint.X, newPoint.Y) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) && map[newPoint].WallType != WallID.None && Main.wallHouse[map[newPoint].WallType])
                        {
                            var slopeType = SlopeType.Solid;
                            if (offsets[m].X != 0)
                            {
                                slopeType = map[newPoint].Slope;
                                if (TileID.Sets.Platforms[map[newPoint].TileType])
                                {
                                    if (map[newPoint].TileFrameX == 144)
                                    {
                                        slopeType = SlopeType.SlopeDownLeft;
                                    }
                                    if (map[newPoint].TileFrameX == 180)
                                    {
                                        slopeType = SlopeType.SlopeDownRight;
                                    }
                                }
                            }

                            if (!map[newPoint].HasTile || ((!map[newPoint].IsSolid || map[newPoint].IsSolidTop) && (!map[newPoint].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor) || slopeType != SlopeType.Solid)))
                            {
                                for (int n = 0; n < offsets.Length; n++)
                                {
                                    var checkWallPoint = newPoint + offsets[n];
                                    if (!map.InSceneRenderedMap(checkWallPoint) || (!map[checkWallPoint].IsFullySolid && (map[checkWallPoint].WallType == WallID.None || !Main.wallHouse[map[checkWallPoint].WallType])))
                                    {
                                        return new List<Point>() { new Point(x, y) };
                                    }
                                }
                                addPoints.Add(newPoint);
                                addedAny = true;
                            }
                        }
                    }
                }
                if (!addedAny)
                {
                    return checkedPoints;
                }
            }
            return checkedPoints;
        }

        public static int CountDecorInsideHouse(TileMapCache map, List<Point> house, Dictionary<int, List<int>> tileStyleData = null)
        {
            int decorAmt = 0;
            if (tileStyleData == null)
                tileStyleData = new Dictionary<int, List<int>>();

            foreach (var p in house)
            {
                if (map[p].HasTile)
                {
                    if (!map[p].IsSolid)
                    {
                        int style = AequusHelpers.GetTileStyle(map[p].TileType, map[p].TileFrameX, map[p].TileFrameY);
                        if (tileStyleData.TryGetValue(map[p].TileType, out List<int> compareStyle))
                        {
                            if (compareStyle.Contains(style))
                            {
                                continue;
                            }
                            compareStyle.Add(style);
                        }
                        else
                        {
                            tileStyleData.Add(map[p].TileType, new List<int>() { style });
                        }

                        decorAmt++;
                    }
                }
            }
            return decorAmt;
        }

        public override void NetSend(BinaryWriter writer)
        {
            SendCompletedBounties();
            int count = 0;
            foreach (var b in BuildingBuffLocations.Values)
            {
                if (b.Count > 0)
                    count++;
            }
            writer.Write(count);
            foreach (var pair in BuildingBuffLocations)
            {
                var b = pair.Value;
                if (b.Count < 0)
                {
                    continue;
                }
                var l = b;
                writer.Write(pair.Key);
                writer.Write(l.Count);
                for (int j = 0; j < l.Count; j++)
                {
                    writer.Write(l[j].X);
                    writer.Write(l[j].Y);
                    writer.Write(l[j].Width);
                    writer.Write(l[j].Height);
                }
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            int buildingBuffLocationsCount = reader.ReadInt32();
            BuildingBuffLocations?.Clear();
            for (int i = 0; i < buildingBuffLocationsCount; i++)
            {
                int type = reader.ReadInt32();
                int listCount = reader.ReadInt32();
                for (int j = 0; j < listCount; j++)
                {
                    var r = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                    AddBuildingBuffLocation(type, r, quiet: true);
                }
            }
        }

        public static void SendCompletedBounties()
        {
            var p = Aequus.GetPacket(PacketType.CarpenterBountiesCompleted);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                p.Send();
                return;
            }
            TrimCompletedBounties();

            var l = new List<int>();
            for (int i = 0; i < CompletedBounties.Count; i++)
            {
                if (BountiesByName.ContainsKey(CompletedBounties[i]))
                    l.Add(BountiesByName[CompletedBounties[i]].Type);
            }

            p.Write(l.Count);
            for (int i = 0; i < CompletedBounties.Count; i++)
            {
                p.Write(l[i]);
            }
            p.Send();
        }

        public static void ReceiveCompletedBounties(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                SendCompletedBounties();
                return;
            }
            CompletedBounties?.Clear();
            int amt = reader.ReadInt32();
            var bounties = new List<int>();
            for (int i = 0; i < amt; i++)
            {
                var b = BountiesByID[reader.ReadInt32()];
                CompletedBounties.Add(b.FullName);
            }
        }

        public static void ResetBounties()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Aequus.GetPacket(PacketType.ResetCarpenterBounties).Send();
                return;
            }
            CompletedBounties?.Clear();
            if (Main.netMode == NetmodeID.Server)
            {
                SendCompletedBounties();
            }
            else
            {
                TrimCompletedBounties();
            }
        }

        public static void CompleteCarpenterBounty(CarpenterBounty b)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
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

        public static void TrimCompletedBounties()
        {
            for (int i = 0; i < CompletedBounties.Count - 1; i++)
            {
                for (int j = i + 1; j < CompletedBounties.Count; j++)
                {
                    if (CompletedBounties[i] == CompletedBounties[j])
                    {
                        CompletedBounties.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}