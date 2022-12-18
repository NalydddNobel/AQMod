using Aequus.Content.CarpenterBounties.Steps;
using Aequus.Items.Placeable;
using Aequus.Items.Tools;
using Aequus.Items.Tools.Misc;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterSystem : ModSystem
    {
        internal static List<CarpenterBounty> BountiesByID;
        internal static Dictionary<string, CarpenterBounty> BountiesByName;

        public static Dictionary<int, bool> CraftableTileLookup { get; private set; }

        public static int BountyCount => BountiesByID.Count;

        public override void Load()
        {
            CraftableTileLookup = new Dictionary<int, bool>();
        }

        public override void SetupContent()
        {
            new CarpenterBounty("FountainBounty")
                .SetReward<AdvancedRuler>()
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

        public override void Unload()
        {
            BountiesByID?.Clear();
            BountiesByName?.Clear();
            CraftableTileLookup?.Clear();
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
    }
}