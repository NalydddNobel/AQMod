using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Aequus.Content.Town.ExporterNPC.Quest
{
    public class ExporterQuestSystem : ModSystem
    {
        public static Dictionary<int, IPlacementData> TilePlacements { get; private set; }
        public static Dictionary<int, IThieveryItemInfo> QuestItems { get; private set; }
        public static HashSet<int> NPCTypesNoSpawns { get; private set; }

        public static int placeCheck;
        [SaveData("QuestsCompleted")]
        public static int QuestsCompleted;

        public override void Load()
        {
            TilePlacements = new Dictionary<int, IPlacementData>();
            QuestItems = new Dictionary<int, IThieveryItemInfo>();
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
            ResetPlaceCheck();
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
            placeCheck--;

            if (placeCheck == 0)
            {
                ResetPlaceCheck();
                if (CanCheckPlacement())
                {
                    CheckPlacement();
                }
            }
        }
        public static void CheckPlacement()
        {
            var n = GetSelectableNPCs();
            if (n.Count <= 0)
            {
                return;
            }

            if (!Place(n[WorldGen.genRand.Next(n.Count)], WorldGen.genRand))
            {
                placeCheck = Math.Min(placeCheck, 1280);
            }
        }
        public static bool CanCheckPlacement()
        {
            return NPC.AnyNPCs(ModContent.NPCType<Exporter>());
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
                    NetMessage.SendTileSquare(-1, points[selectedPoint].X - 1, points[selectedPoint].Y - 1, 3, 3);
                    return true;
                }
                points.RemoveAt(selectedPoint);
            }

            return false;
        }
        public static bool InnerPlace_CheckQuestTiles(NPC townNPC)
        {
            const int size = 150;
            var rectangle = new Rectangle(townNPC.homeTileX - size / 2, townNPC.homeTileY - size / 2, size, size).Fluffize();

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
        public static void ResetPlaceCheck()
        {
            placeCheck = WorldGen.genRand.Next(1200, 3600) * 10;
        }
    }
}