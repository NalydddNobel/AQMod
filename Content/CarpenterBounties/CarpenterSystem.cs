using Aequus.Common;
using Microsoft.Xna.Framework;
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

        public override void Unload()
        {
            BountiesByID?.Clear();
            BountiesByName?.Clear();
            CraftableTileLookup?.Clear();
        }

        public static void RegisterBounty(CarpenterBounty bounty)
        {
            if (BountiesByID == null || BountiesByName == null)
            {
                BountiesByID = new List<CarpenterBounty>();
                BountiesByName = new Dictionary<string, CarpenterBounty>();
            }

            string name = bounty.FullName;
            if (BountiesByName.ContainsKey(name))
                throw new System.Exception($"{bounty.Mod.Name} added two bounties with the same name ({bounty.Name})");
            bounty.Type = BountyCount;
            ModTypeLookup<CarpenterBounty>.Register(bounty);
            BountiesByID.Add(bounty);
            BountiesByName.Add(name, bounty);
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

        public static CarpenterBounty GetBounty(int type)
        {
            return BountiesByID[type];
        }

        public static CarpenterBounty GetBounty(string mod, string name)
        {
            return BountiesByName[mod + "." + name];
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