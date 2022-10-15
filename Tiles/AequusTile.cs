using Aequus.Biomes.DemonSiege;
using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Tiles.Ambience;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.PhysicistBlocks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class AequusTile : GlobalTile, IPostSetupContent
    {
        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

        public const int ShadowOrbDrops_Aequus = 5;

        public struct IndestructibleCircle
        {
            private Vector2 measurementCenterPoint;
            private Point centerPoint;
            public Point CenterPoint
            {
                get => centerPoint; 

                set
                {
                    centerPoint = value;
                    measurementCenterPoint = value.ToVector2();
                }
            }
            public float tileRadius;

            public bool InPoint(int i, int j)
            {
                return Vector2.Distance(measurementCenterPoint, new Vector2(i, j)) < tileRadius;
            }
        }

        private static List<IndestructibleCircle> CheckCircles;
        public static List<IndestructibleCircle> Circles { get; private set; }

        public static Dictionary<Point, Color> PylonColors { get; private set; }
        public static Dictionary<TileKey, int> TileIDToItemID { get; private set; }

        public override void Load()
        {
            TileIDToItemID = new Dictionary<TileKey, int>();
            CheckCircles = new List<IndestructibleCircle>();
            Circles = new List<IndestructibleCircle>();
            PylonColors = new Dictionary<Point, Color>()
            {
                [new Point(TileID.TeleportationPylon, 0)] = new Color(100, 255, 128, 255),
                [new Point(TileID.TeleportationPylon, 1)] = new Color(200, 255, 65, 255),
                [new Point(TileID.TeleportationPylon, 2)] = Color.HotPink * 1.5f,
                [new Point(TileID.TeleportationPylon, 3)] = new Color(230, 165, 255, 255),
                [new Point(TileID.TeleportationPylon, 4)] = Color.SkyBlue * 1.125f,
                [new Point(TileID.TeleportationPylon, 5)] = new Color(255, 222, 120, 255),
                [new Point(TileID.TeleportationPylon, 6)] = new Color(120, 222, 255, 255),
                [new Point(TileID.TeleportationPylon, 7)] = new Color(100, 128, 255, 255),
                [new Point(TileID.TeleportationPylon, 8)] = Color.FloralWhite,
            };
            LoadHooks();
        }

        #region Hooks
        private static void LoadHooks()
        {
            On.Terraria.WorldGen.CanCutTile += WorldGen_CanCutTile;
            On.Terraria.WorldGen.QuickFindHome += WorldGen_QuickFindHome;
        }

        private static bool WorldGen_CanCutTile(On.Terraria.WorldGen.orig_CanCutTile orig, int x, int y, Terraria.Enums.TileCuttingContext context)
        {
            if (orig(x, y, context))
            {
                return !Main.tile[x, y].Get<AequusTileData>().Uncuttable;
            }
            return false;
        }

        private static void WorldGen_QuickFindHome(On.Terraria.WorldGen.orig_QuickFindHome orig, int npc)
        {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            orig(npc);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
        }
        #endregion

        public void PostSetupContent(Aequus aequus)
        {
            foreach (var i in ContentSamples.ItemsByType)
            {
                if (i.Value.createTile > -1)
                {
                    var tileID = new TileKey((ushort)i.Value.createTile, i.Value.placeStyle);
                    if (TileIDToItemID.ContainsKey(tileID))
                    {
                        if (!i.Value.consumable || i.Key == TileIDToItemID[tileID])
                        {
                            continue;
                        }

                        aequus.Logger.Info($"Duplicate block placement detected: (Current: {Lang.GetItemName(TileIDToItemID[tileID])}, Duplicate: {Lang.GetItemName(i.Key)})");
                        continue;
                    }
                }
            }
        }

        public override void Unload()
        {
            TileIDToItemID?.Clear();
            TileIDToItemID = null;
            CheckCircles?.Clear();
            CheckCircles = null;
            Circles?.Clear();
            Circles = null;
        }

        internal static void UpdateIndestructibles()
        {
            CheckCircles.Clear();
            CheckCircles.AddRange(Circles);
            Circles.Clear();
        }

        public void GrowPearl(int i, int j)
        {
            for (int k = -3; k <= 3; k++)
            {
                for (int l = -10; l <= 10; l++)
                {
                    if (Main.tile[i + k, j + l].HasTile && Main.tile[i + k, j + l].TileType == ModContent.TileType<PearlsTile>())
                    {
                        return;
                    }
                }
            }

            var p = new List<Point>();
            if (!Main.tile[i + 1, j].HasTile && WorldGen.genRand.NextBool(4))
            {
                p.Add(new Point(i + 1, j));
            }
            if (!Main.tile[i - 1, j].HasTile && WorldGen.genRand.NextBool(4))
            {
                p.Add(new Point(i - 1, j));
            }
            if (!Main.tile[i, j + 1].HasTile && WorldGen.genRand.NextBool(4))
            {
                p.Add(new Point(i, j + 1));
            }
            if (!Main.tile[i, j - 1].HasTile)
            {
                p.Add(new Point(i, j - 1));
            }

            if (p.Count > 0)
            {
                var chosen = WorldGen.genRand.Next(p);
                if (ModContent.GetInstance<PearlsTile>().CanPlace(chosen.X, chosen.Y))
                {
                    WorldGen.PlaceTile(chosen.X, chosen.Y, ModContent.TileType<PearlsTile>(), mute: true);
                    if (Main.tile[chosen].TileType == ModContent.TileType<PearlsTile>())
                    {
                        Main.tile[chosen.X, chosen.Y].TileFrameX = (short)(WorldGen.genRand.Next(3) * 18);
                        if (WorldGen.genRand.NextBool(4))
                        {
                            Main.tile[chosen.X, chosen.Y].TileFrameX += 18;
                        }
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendTileSquare(-1, chosen.X, chosen.Y);
                        }
                    }
                }
            }
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            switch (type)
            {
                case TileID.Meteorite:
                    if (AequusWorld.downedOmegaStarite && j < Main.rockLayer && WorldGen.genRand.NextBool(150))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Meteorite, }, ModContent.TileType<MoonflowerTile>());
                    }
                    break;
            }
            if (Main.tile[i, j].WallType == ModContent.WallType<SedimentaryRockWallWall>() && WorldGen.genRand.NextBool(20))
            {
                GrowPearl(i, j);
            }
        }
        public static bool TryPlaceHerb(int i, int j, int[] validTile, int tile)
        {
            for (int y = j - 1; y > 20; y--)
            {
                if (!Main.tile[i, y].HasTile && Main.tile[i, y + 1].HasTile)
                {
                    for (int k = 0; k < validTile.Length; k++)
                    {
                        if (Main.tile[i, y + 1].TileType == validTile[k] && !CheckForType(new Rectangle(i - 6, y - 6, 12, 12).Fluffize(20), tile))
                        {
                            WorldGen.PlaceTile(i, y, tile, mute: true, forced: true);
                            return Main.tile[i, y].TileType == tile;
                        }
                    }
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (WorldGen.gen)
                return true;

            foreach (var c in CheckCircles)
            {
                if (c.InPoint(i, j))
                    return false;
            }
            foreach (var s in DemonSiegeSystem.ActiveSacrifices)
            {
                if (s.Value.ProtectedTiles().Contains(i, j))
                    return false;
            }
            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                Main.tile[i, j].Get<AequusTileData>().OnKillTile();
                if (ArmFloaties.EquippedCache.Count > 0)
                {
                    int closestPlayer = -1;
                    float distance = 240f;
                    foreach (var p in ArmFloaties.EquippedCache)
                    {
                        if (Main.player[p].active && !Main.player[p].dead && !Main.player[p].ghost
                            && Main.player[p].breath < Main.player[p].breathMax && Main.player[p].Aequus().accArmFloaties > 0)
                        {
                            float d = Main.player[p].Distance(new Vector2(i * 16f + 8f, j * 16f + 8f));
                            if (d < distance)
                            {
                                closestPlayer = p;
                                distance = d;
                            }
                        }
                    }
                    if (closestPlayer != -1)
                    {
                        Main.player[closestPlayer].breath += Main.player[closestPlayer].breathMax / 15 * Main.player[closestPlayer].Aequus().accArmFloaties;
                        if (Main.player[closestPlayer].breath > Main.player[closestPlayer].breathMax - 1)
                        {
                            Main.player[closestPlayer].breath = Main.player[closestPlayer].breathMax - 1;
                        }
                    }
                }
            }
        }

        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 36 == 0)
            {
                if (Main.tile[i, j].TileFrameX < 36)
                {
                    CorruptionOrbDrops(i, j);
                }
                else
                {
                    CrimsonOrbDrops(i, j);
                }
                AequusWorld.shadowOrbsBrokenTotal++;
            }
            return true;
        }
        public void CrimsonOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CrimsonCandle>());
                    break;
            }
        }
        public void CorruptionOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CorruptionCandle>());
                    break;
            }
        }
        public int OrbDrop()
        {
            return AequusWorld.shadowOrbsBrokenTotal < ShadowOrbDrops_Aequus ? AequusWorld.shadowOrbsBrokenTotal : WorldGen.genRand.Next(ShadowOrbDrops_Aequus);
        }

        public static bool CheckForType(Rectangle rect, ArrayInterpreter<int> type)
        {
            return !CheckTiles(rect, (i, j, tile) => !type.Arr.ContainsAny(tile.TileType));
        }
        public static bool CheckTiles(Rectangle rect, Func<int, int, Tile, bool> function)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    if (!function(i, j, Main.tile[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void GemFrame(int i, int j)
        {
            var tile = Framing.GetTileSafely(i, j);
            var top = Main.tile[i, j - 1];
            var bottom = Framing.GetTileSafely(i, j + 1);
            var left = Main.tile[i - 1, j];
            var right = Main.tile[i + 1, j];
            if (top != null && top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                if (tile.TileFrameY < 54 || tile.TileFrameY > 90)
                {
                    tile.TileFrameY = (short)(54 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }
            if (bottom != null && bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                if (tile.TileFrameY < 0 || tile.TileFrameY > 36)
                {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);
                }
                return;
            }
            if (left != null && left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                if (tile.TileFrameY < 108 || tile.TileFrameY > 54)
                {
                    tile.TileFrameY = (short)(108 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }
            if (right != null && right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                if (tile.TileFrameY < 162 || tile.TileFrameY > 198)
                {
                    tile.TileFrameY = (short)(162 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }
            WorldGen.KillTile(i, j);
        }
    }
}