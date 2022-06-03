using Aequus.Common.IO;
using Aequus.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Aequus.Content
{
    public class ExporterQuests : ModSystem
    {
        public interface IPlacementData
        {
            List<Point> ScanRoom(NPC townNPC);

            public static Point GetStartingPoint(NPC townNPC)
            {
                var home = new Point(townNPC.homeTileX, townNPC.homeTileY);
                home.Y--;
                while (home.Y > 10 && !Main.tile[home].IsSolid())
                {
                    home.Y--;
                }
                home.Y++;
                return home;
            }
        }
        public class SolidTopPlacement : IPlacementData
        {
            public List<Point> ScanRoom(NPC townNPC)
            {
                var home = IPlacementData.GetStartingPoint(townNPC);
                var p = new List<Point>();

                InnerScanRoom(home, p, 1);
                InnerScanRoom(home, p, -1);

                return p;
            }
            public static void InnerScanRoom(Point home, List<Point> p, int dir)
            {
                int wallUp = 0;
                int wallDown = 0;
                int oldY = home.Y;
                for (int i = 0; i < 20; i++)
                {
                    int x = home.X + i * dir;

                    if (Main.tile[x, home.Y].IsSolid() || TileID.Sets.RoomNeeds.CountsAsDoor.ContainsAny(
                        (type) => type == Main.tile[x, home.Y].TileType))
                    {
                        if (wallUp > 10 || Main.tile[x - dir, home.Y].IsSolid())
                        {
                            break;
                        }
                        home.Y++;
                        wallUp++;
                        i--;
                        continue;
                    }
                    wallUp = 0;

                    if (!Main.tile[x, home.Y - 1].IsSolid())
                    {
                        wallDown++;
                        home.Y--;
                        if (home.Y < 10 || wallDown > 10)
                        {
                            break;
                        }
                        i--;
                        continue;
                    }
                    wallDown = 0;

                    InnerScanRoom_Downwards(x, home.Y, p);
                }
            }
            public static void InnerScanRoom_Downwards(int x, int y, List<Point> points)
            {
                for (int endY = y + 20; y < endY; y++)
                {
                    if (Main.tile[x, y].HasTile)
                    {
                        if (Main.tile[x, y].SolidTop() && !Main.tile[x, y - 1].HasTile)
                        {
                            points.Add(new Point(x, y));
                        }
                        else if (Main.tile[x, y].Solid())
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static Dictionary<int, IPlacementData> TilePlacements { get; private set; }
        public static HashSet<int> NPCTypesNoSpawns { get; private set; }

        public static int PlaceCheck;
        [SaveData("QuestsCompleted")]
        public static int QuestsCompleted;

        public override void Load()
        {
            TilePlacements = new Dictionary<int, IPlacementData>();
            NPCTypesNoSpawns = new HashSet<int>()
            {
                NPCID.Guide,
                NPCID.Angler,
                NPCID.BestiaryGirl,
                NPCID.Golfer,
                NPCID.Dryad,
                NPCID.Pirate,
                NPCID.Cyborg,
                NPCID.Truffle,
                NPCID.Princess,
                NPCID.SantaClaus,
            };
        }

        public override void OnWorldLoad()
        {
            PlaceCheck = RollPlaceCheck();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            SaveDataAttribute.SaveData(tag, this);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SaveDataAttribute.LoadData(tag, this);
        }

        public override void PostUpdateWorld()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Exporter>()))
                PlaceCheck--;

            if (PlaceCheck == 0)
            {
                PlaceCheck = RollPlaceCheck();
                var n = GetSelectableNPCs();
                if (n.Count <= 0)
                {
                    return;
                }

                if (!Place(n[WorldGen.genRand.Next(n.Count)], WorldGen.genRand))
                {
                    PlaceCheck = Math.Min(PlaceCheck, 1280);
                }
            }
        }
        public static List<NPC> GetSelectableNPCs()
        {
            var list = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC && !Main.npc[i].homeless && 
                    !NPCTypesNoSpawns.Contains(Main.npc[i].type))
                {
                    list.Add(Main.npc[i]);
                }
            }
            return list;
        }
        public static bool Place(NPC townNPC, UnifiedRandom rand)
        {
            if (InnerPlace_CheckQuestTiles(townNPC))
            {
                return false;
            }

            if (InnerPlace_CheckPlayers(townNPC))
            {
                return false;
            }

            var selectableTiles = TilePlacements.Keys.ToList();
            int tileID = selectableTiles[rand.Next(selectableTiles.Count)];
            var data = TilePlacements[tileID];

            var points = data.ScanRoom(townNPC);

            if (points.Count == 0)
            {
                return false;
            }


            while (points.Count > 0)
            {
                int selectedPoint = rand.Next(points.Count);

                WorldGen.PlaceTile(points[selectedPoint].X, points[selectedPoint].Y - 1, tileID, mute: true);

                if (Main.tile[points[selectedPoint].X, points[selectedPoint].Y - 1].TileType == tileID)
                {
                    return true;
                }
                points.RemoveAt(selectedPoint);
            }

            return false;
        }
        public static bool InnerPlace_CheckQuestTiles(NPC townNPC)
        {
            var rectangle = new Rectangle(townNPC.homeTileX - 35, townNPC.homeTileY - 35, 70, 70).Fluffize();

            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
            {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tileFrameImportant[Main.tile[i, j].TileType]
                        && TilePlacements.ContainsKey(Main.tile[i, j].TileType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool InnerPlace_CheckPlayers(NPC townNPC)
        {
            var homePosition = new Vector2(townNPC.homeTileX * 16f, townNPC.homeTileY * 16f);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Vector2.Distance(Main.player[i].Center, homePosition) < 1000f)
                {
                    return true;
                }
            }
            return false;
        }
        public static int RollPlaceCheck()
        {
            return WorldGen.genRand.Next(1200, 3600);
        }
    }
}