using Aequus;
using Aequus.Biomes.CrabCrevice.Tiles;
using Aequus.Biomes.GoreNest.Tiles;
using Aequus.Common.Utilities;
using Aequus.Events.DemonSiege;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.NPCs.CarpenterNPC.Shop;
using Aequus.NPCs.PhysicistNPC.Shop;
using Aequus.Tiles.Ambience;
using Aequus.Tiles.Moss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public class AequusTile : GlobalTile, IPostSetupContent, IAddRecipes
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

        public static Dictionary<Point, Func<Color>> PylonColors { get; private set; }
        public static Dictionary<TileKey, int> TileIDToItemID { get; private set; }
        public static Dictionary<int, int> WallIDToItemID { get; private set; }

        internal static bool[] All;
        public static bool[] IsNotProtected { get; private set; }

        public override void Load()
        {
            WallIDToItemID = new Dictionary<int, int>();
            TileIDToItemID = new Dictionary<TileKey, int>();
            CheckCircles = new List<IndestructibleCircle>();
            Circles = new List<IndestructibleCircle>();
            PylonColors = new Dictionary<Point, Func<Color>>();
            LoadHooks();
        }

        #region Hooks
        private static void LoadHooks()
        {
            //On.Terraria.GameContent.Tile_Entities.TEDisplayDoll.Draw += TEDisplayDoll_Draw;
            On.Terraria.WorldGen.PlaceTile += WorldGen_PlaceTile;
            On.Terraria.WorldGen.UpdateWorld_OvergroundTile += WorldGen_UpdateWorld_OvergroundTile;
            On.Terraria.WorldGen.UpdateWorld_UndergroundTile += WorldGen_UpdateWorld_UndergroundTile;
            On.Terraria.WorldGen.CanCutTile += WorldGen_CanCutTile;
            On.Terraria.WorldGen.QuickFindHome += WorldGen_QuickFindHome;
        }

        private static void TEDisplayDoll_Draw(On.Terraria.GameContent.Tile_Entities.TEDisplayDoll.orig_Draw orig, Terraria.GameContent.Tile_Entities.TEDisplayDoll self, int tileLeftX, int tileTopY)
        {
            var texture = ModContent.Request<Texture2D>("Aequus/Tiles/Moss/MannequinArmorOverlay").Value;
            var frame = texture.Frame(horizontalFrames: 8, frameX: 0);
            bool facingLeft = Main.tile[tileLeftX, tileTopY].TileFrameX <= 0;
            var drawCoords = new Vector2(tileLeftX * 16f, tileTopY * 16f) - Main.screenPosition + new Vector2(facingLeft ? -2f : -4f, -14f);
            var effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var color = new Color(255, 180, 180, 255);
            Main.spriteBatch.Draw(texture, drawCoords, frame.Frame(frameX: 1, 0), color, 0f, Vector2.Zero, 1f, effects, 0f);
            orig(self, tileLeftX, tileTopY);
            Main.spriteBatch.Draw(texture, drawCoords, frame, color, 0f, Vector2.Zero, 1f, effects, 0f);
        }

        private static bool WorldGen_PlaceTile(On.Terraria.WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style)
        {
            if (Type >= TileID.Count && TileLoader.GetTile(Type) is TileHooks.IOnPlaceTile onPlaceTile)
            {
                var val = onPlaceTile.OnPlaceTile(i, j, mute, forced, plr, style);
                if (val.HasValue)
                    return val.Value;
            }
            return orig(i, j, Type, mute, forced, plr, style);
        }

        private static void WorldGen_UpdateWorld_UndergroundTile(On.Terraria.WorldGen.orig_UpdateWorld_UndergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist)
        {
            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType >= TileID.Count && TileLoader.GetTile(Main.tile[i, j].TileType) is TileHooks.IDontRunVanillaRandomUpdate)
            {
                TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
                WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        private static void WorldGen_UpdateWorld_OvergroundTile(On.Terraria.WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist)
        {
            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType >= TileID.Count && TileLoader.GetTile(Main.tile[i, j].TileType) is TileHooks.IDontRunVanillaRandomUpdate)
            {
                TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
                WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        private static bool WorldGen_CanCutTile(On.Terraria.WorldGen.orig_CanCutTile orig, int x, int y, Terraria.Enums.TileCuttingContext context)
        {
            if (orig(x, y, context))
            {
                return !Main.tile[x, y].Get<AequusTileData>().Uncuttable && Main.LocalPlayer.HeldItem?.ModItem is not SilkPickaxe;
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
            All = new bool[TileLoader.TileCount];
            for (int i = 0; i < All.Length; i++)
            {
                All[i] = true;
            }
            IsNotProtected = new bool[TileLoader.TileCount];
            for (int i = 0; i < All.Length; i++)
            {
                IsNotProtected[i] = !Main.tileDungeon[i];
            }
            IsNotProtected[TileID.LihzahrdBrick] = false;
            IsNotProtected[TileID.LihzahrdAltar] = false;
            IsNotProtected[TileID.ShadowOrbs] = false;
        }

        public void AddRecipes(Aequus aequus)
        {
            LoadEchoWallSet();
            AutoCreateSilkTouchSets(aequus);
            LoadPylonColorsSet();
        }
        private static void AutoCreateSilkTouchSets(Aequus aequus)
        {
            if (Aequus.LogMore)
            {
                Aequus.Instance.Logger.Info("Auto Creating silk touch ids...");
            }
            foreach (var i in ContentSamples.ItemsByType)
            {
                if (i.Value.createTile > -1)
                {
                    int randomStyleVariation = 1;
                    var tileData = TileObjectData.GetTileData(i.Value.createTile, i.Value.placeStyle);
                    if (tileData != null)
                    {
                        randomStyleVariation = tileData.RandomStyleRange;
                    }
                    if (ItemID.Sets.flowerPacketInfo[i.Key] != null)
                    {
                        AddFlowerPacket(i.Key, ItemID.Sets.flowerPacketInfo[i.Key]);
                        continue;
                    }
                    for (int k = 0; k < randomStyleVariation; k++)
                    {
                        var tileID = new TileKey((ushort)i.Value.createTile, i.Value.placeStyle + k);
                        if (TileIDToItemID.ContainsKey(tileID))
                        {
                            if (!i.Value.consumable || i.Key == TileIDToItemID[tileID])
                            {
                                continue;
                            }

                            aequus.Logger.Info($"Duplicate block placement detected: (Current: {Lang.GetItemName(TileIDToItemID[tileID])}, Duplicate: {Lang.GetItemName(i.Key)})");
                            continue;
                        }
                        TileIDToItemID[tileID] = i.Key;
                    }
                }
                else if (i.Value.createWall > -1)
                {
                    if (WallIDToItemID.ContainsKey(i.Value.createWall))
                    {
                        if (!i.Value.consumable || i.Key == WallIDToItemID[i.Value.createWall])
                        {
                            continue;
                        }

                        aequus.Logger.Info($"Duplicate block placement detected: (Current: {Lang.GetItemName(WallIDToItemID[i.Value.createWall])}, Duplicate: {Lang.GetItemName(i.Key)})");
                        continue;
                    }
                    WallIDToItemID[i.Value.createWall] = i.Key;
                }
            }
        }
        private static void AddFlowerPacket(int itemID, FlowerPacketInfo flowerPacketInfo)
        {
            if (itemID == ItemID.FlowerPacketWild)
                return;

            foreach (var i in flowerPacketInfo.stylesOnPurity)
            {
                TileIDToItemID[new TileKey(TileID.Plants, i)] = itemID;
            }
            foreach (var i in flowerPacketInfo.stylesOnCorruption)
            {
                TileIDToItemID[new TileKey(TileID.CorruptPlants, i)] = itemID;
            }
            foreach (var i in flowerPacketInfo.stylesOnCrimson)
            {
                TileIDToItemID[new TileKey(TileID.CrimsonPlants, i)] = itemID;
            }
            foreach (var i in flowerPacketInfo.stylesOnHallow)
            {
                TileIDToItemID[new TileKey(TileID.HallowedPlants, i)] = itemID;
            }
        }
        private static void LoadEchoWallSet()
        {
            if (Aequus.LogMore)
            {
                Aequus.Instance.Logger.Info("Loading silk touch walls ids...");
            }
            var val = Aequus.GetContentFile("SilkTouchWalls");
            foreach (var modDict in val)
            {
                if (modDict.Key == "Vanilla")
                {
                    foreach (var wallID in modDict.Value)
                    {
                        WallIDToItemID[WallID.Search.GetId(wallID.Key)] = ItemID.Search.GetId(wallID.Value);
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod))
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Info($"Loading custom wall to item ID table entries for {modDict.Key}...");
                    }
                    foreach (var wallID in modDict.Value)
                    {
                        if (mod.TryFind<ModWall>(wallID.Key, out var modWall) && mod.TryFind<ModItem>(wallID.Value, out var modItem))
                        {
                            WallIDToItemID[modWall.Type] = modItem.Type;
                        }
                    }
                }
            }
        }

        public static void LoadPylonColorsSet()
        {
            if (Aequus.LogMore)
            {
                Aequus.Instance.Logger.Info("Loading pylons colors...");
            }
            var val = Aequus.GetContentFile("PylonColors");
            foreach (var modDict in val)
            {
                if (modDict.Key == "Vanilla")
                {
                    foreach (var pylonColor in modDict.Value)
                    {
                        var clr = AequusHelpers.ReadColor(pylonColor.Value);
                        PylonColors[new Point(TileID.TeleportationPylon, int.Parse(pylonColor.Key))] = () => clr;
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod))
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Info($"Loading pylon colors for {modDict.Key}...");
                    }
                    foreach (var pylonColor in modDict.Value)
                    {
                        int style = 0;
                        string pylonName = pylonColor.Key;
                        if (pylonName.Contains(';'))
                        {
                            var split = pylonName.Split(';');
                            pylonName = split[0];
                            style = int.Parse(split[1]);
                        }
                        if (mod.TryFind<ModPylon>(pylonName, out var pylon))
                        {
                            if (Aequus.LogMore)
                            {
                                Aequus.Instance.Logger.Info($"{pylonName}/{style}/{pylon.Type}: {pylonColor.Value}");
                            }
                            var clr = AequusHelpers.ReadColor(pylonColor.Value);
                            PylonColors[new Point(pylon.Type, style)] = () => clr;
                        }
                        else if (Aequus.LogMore)
                        {
                            Aequus.Instance.Logger.Error($"Could not find {pylonName}...");
                        }
                    }
                }
            }

            PylonColors[new Point(TileID.TeleportationPylon, 8)] = () => Main.DiscoColor;
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

        public static bool GrowGrass(int x, int y, int tileID)
        {
            for (int k = -1; k <= 1; k++)
            {
                for (int l = -1; l <= 1; l++)
                {
                    if (!Main.tile[x + k, y + l].IsFullySolid())
                    {
                        Main.tile[x, y].TileType = (ushort)tileID;
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool TryGrowMosshroom(int i, int j, int style)
        {
            if (Main.tile[i + 1, j].TileType != Main.tile[i, j].TileType)
            {
                return false;
            }
            for (int k = 0; k < 2; k++)
            {
                if (Main.tile[i + k, j].Slope != SlopeType.Solid || Main.tile[i + k, j].IsHalfBlock)
                {
                    return false;
                }
            }
            int checkSize = 5;
            j -= 2;
            if (CheckForType(new Rectangle(i - checkSize, j - checkSize, checkSize * 2, checkSize * 2).Fluffize(20), ModContent.TileType<EnemyBuffPlants>()))
            {
                return false;
            }
            for (int k = 0; k < 2; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    if (Main.tile[i + k, j + l].HasTile && !Main.tileCut[Main.tile[i + k, j + l].TileType])
                    {
                        return false;
                    }
                }
            }
            for (int k = 0; k < 2; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    WorldGen.KillTile(i + k, j + l);
                    if (Main.tile[i + k, j + l].HasTile)
                        return false;
                }
            }
            int frame = style;
            for (int k = 0; k < 2; k++)
            {
                for (int l = 0; l < 2; l++)
                {
                    WorldGen.KillTile(i + k, j + l);
                    Main.tile[i + k, j + l].Active(value: true);
                    Main.tile[i + k, j + l].TileType = (ushort)ModContent.TileType<EnemyBuffPlants>();
                    Main.tile[i + k, j + l].TileFrameX = (short)((frame * 2 + k) * 18);
                    Main.tile[i + k, j + l].TileFrameY = (short)(l * 18);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, i + -1, j + l - 1, 3, 3);
                }
            }

            return false;
        }
        public static void GrowPearl(int i, int j)
        {
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
                for (int k = -17; k <= 17; k++)
                {
                    for (int l = -20; l <= 20; l++)
                    {
                        if (WorldGen.InWorld(i + k, j + l) && Main.tile[i + k, j + l].HasTile && Main.tile[i + k, j + l].TileType == ModContent.TileType<PearlsTile>())
                        {
                            return;
                        }
                    }
                }
                var chosen = WorldGen.genRand.Next(p);
                if (ModContent.GetInstance<PearlsTile>().CanPlace(chosen.X, chosen.Y))
                {
                    WorldGen.PlaceTile(chosen.X, chosen.Y, ModContent.TileType<PearlsTile>(), mute: true);
                    if (Main.tile[chosen].TileType == ModContent.TileType<PearlsTile>())
                    {
                        int frame = 0;
                        if (WorldGen.genRand.NextBool(3))
                        {
                            frame = 1;
                        }
                        else if (WorldGen.genRand.NextBool(12))
                        {
                            frame = 2;
                        }
                        else if (WorldGen.genRand.NextBool(6))
                        {
                            frame = 3;
                        }
                        Main.tile[chosen.X, chosen.Y].TileFrameX = (short)(frame * 18);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendTileSquare(-1, chosen.X, chosen.Y);
                        }
                    }
                }
            }
        }
        public static bool TryPlaceHerb(int i, int j, int[] validTile, int tile, int checkSize = 6)
        {
            for (int y = j - 1; y > 20; y--)
            {
                if (WorldGen.InWorld(i, y, 30) && !Main.tile[i, y].HasTile && Main.tile[i, y + 1].HasTile)
                {
                    for (int k = 0; k < validTile.Length; k++)
                    {
                        if (Main.tile[i, y + 1].TileType == validTile[k] && !CheckForType(new Rectangle(i - checkSize, y - checkSize, checkSize * 2, checkSize * 2).Fluffize(20), tile))
                        {
                            WorldGen.PlaceTile(i, y, tile, mute: true, forced: true);
                            if (Main.tile[i, y].TileType == tile)
                            {
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendTileSquare(-1, i - 1, y - 1, 3, 3);
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (AequusHelpers.iterations != 0)
            {
                return;
            }
            switch (type)
            {
                case TileID.Grass:
                    if (j > Main.worldSurface && WorldGen.genRand.NextBool(30))
                    {
                        if (!Main.tile[i, j + 1].HasTile)
                        {
                            WorldGen.PlaceTile(i, j + 1, WorldGen.genRand.NextBool(4) ? TileID.VineFlowers : TileID.Vines);
                        }
                    }
                    break;

                case TileID.Vines:
                case TileID.VineFlowers:
                    if (j > Main.worldSurface)
                    {
                        AequusHelpers.iterations++;
                        AequusWorld.RandomUpdateTile_Surface(i, j, checkNPCSpawns: false, wallDist: 3);
                        AequusHelpers.iterations--;
                    }
                    break;

                case TileID.Ash:
                    if (AequusWorld.downedEventDemon && j > Main.UnderworldLayer && WorldGen.genRand.NextBool(2000))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Ash, TileID.Obsidian, }, ModContent.TileType<ManacleTile>(), 30);
                    }
                    break;

                case TileID.Cloud:
                    if (AequusWorld.downedDustDevil && j < Main.rockLayer && WorldGen.genRand.NextBool(1600))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Cloud, TileID.RainCloud, TileID.SnowCloud, }, ModContent.TileType<MistralTile>(), 20);
                    }
                    break;

                case TileID.Meteorite:
                    if (AequusWorld.downedOmegaStarite && j < Main.rockLayer && WorldGen.genRand.NextBool(1600))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Meteorite, }, ModContent.TileType<MoonflowerTile>());
                    }
                    break;

                case TileID.Stone:
                    if (j > Main.worldSurface && WorldGen.genRand.NextBool(Main.hardMode ? 5000 : 1600))
                    {
                        TryGrowMosshroom(i, j, WorldGen.genRand.Next(3));
                    }
                    break;

                case TileID.ArgonMoss:
                case TileID.ArgonMossBrick:
                    if (j > Main.worldSurface && WorldGen.genRand.NextBool(1000))
                        TryGrowMosshroom(i, j, EnemyBuffPlants.Argon);
                    break;

                case TileID.KryptonMoss:
                case TileID.KryptonMossBrick:
                    if (j > Main.worldSurface && WorldGen.genRand.NextBool(1000))
                        TryGrowMosshroom(i, j, EnemyBuffPlants.Krypton);
                    break;

                case TileID.XenonMoss:
                case TileID.XenonMossBrick:
                    if (j > Main.worldSurface && WorldGen.genRand.NextBool(1000))
                        TryGrowMosshroom(i, j, EnemyBuffPlants.Xenon);
                    break;
            }
            if (Main.tile[i, j].WallType == ModContent.WallType<SedimentaryRockWallWall>())
            {
                if (AequusWorld.downedCrabson && WorldGen.genRand.NextBool(1600))
                {
                    TryPlaceHerb(i, j, new int[] { TileID.Sand, TileID.HardenedSand, TileID.Sandstone, ModContent.TileType<SedimentaryRockTile>(), },
                        ModContent.TileType<MorayTile>());
                }
                if (WorldGen.genRand.NextBool(2200))
                {
                    GrowPearl(i, j);
                }
            }
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
            }
        }

        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.Heart && Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (WorldGen.genRand.NextBool(15))
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<HealthCursor>());
                }
            }
            else if (type == TileID.ShadowOrbs && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 36 == 0)
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

        public static bool CheckForType(Rectangle rect, params int[] type)
        {
            return !CheckTiles(rect, (i, j, tile) => !tile.HasTile || !type.ContainsAny(tile.TileType));
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

        public static void GemFrame(int i, int j, params int[] validTiles)
        {
            var tile = Framing.GetTileSafely(i, j);
            var top = Main.tile[i, j - 1];
            var bottom = Framing.GetTileSafely(i, j + 1);
            var left = Main.tile[i - 1, j];
            var right = Main.tile[i + 1, j];
            var obj = TileObjectData.GetTileData(Main.tile[i,j].TileType, 0);
            int coordinateFullHeight = obj?.CoordinateFullHeight ?? 18;
            if (top != null && top.HasTile && !top.BottomSlope && top.TileType >= 0 && validTiles.ContainsAny(top.TileType) && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                if (tile.TileFrameY < 54 || tile.TileFrameY > 90)
                {
                    tile.TileFrameY = (short)(coordinateFullHeight * 3 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                }
                return;
            }
            if (bottom != null && bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && validTiles.ContainsAny(bottom.TileType) && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                if (tile.TileFrameY < 0 || tile.TileFrameY > 36)
                {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(3) * coordinateFullHeight);
                }
                return;
            }
            if (left != null && left.HasTile && left.TileType >= 0 && validTiles.ContainsAny(left.TileType) && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                if (tile.TileFrameY < 108 || tile.TileFrameY > 54)
                {
                    tile.TileFrameY = (short)(coordinateFullHeight * 6 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                }
                return;
            }
            if (right != null && right.HasTile && right.TileType >= 0 && validTiles.ContainsAny(right.TileType) && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                if (tile.TileFrameY < 162 || tile.TileFrameY > 198)
                {
                    tile.TileFrameY = (short)(coordinateFullHeight * 9 + WorldGen.genRand.Next(3) * coordinateFullHeight);
                }
                return;
            }
            WorldGen.KillTile(i, j);
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

        public static sbyte GetGravityTileStatus(Vector2 location)
        {
            var tileCoords = location.ToTileCoordinates();
            if (WorldGen.InWorld(tileCoords.X, tileCoords.Y) && WorldGen.InWorld(tileCoords.X, tileCoords.Y - ForceAntiGravityBlockTile.TileHeightMax) && WorldGen.InWorld(tileCoords.X, tileCoords.Y + ForceGravityBlockTile.TileHeightMax) &&
                (!AequusHelpers.IsSectionLoaded(tileCoords.X, tileCoords.Y) || !AequusHelpers.IsSectionLoaded(tileCoords.X, tileCoords.Y - ForceAntiGravityBlockTile.TileHeightMax) || !AequusHelpers.IsSectionLoaded(tileCoords.X, tileCoords.Y + ForceGravityBlockTile.TileHeightMax)))
            {
                return 0;
            }

            for (int j = 0; j < ForceGravityBlockTile.TileHeightMax; j++)
            {
                if (WorldGen.InWorld(tileCoords.X, tileCoords.Y + j) && Main.tile[tileCoords.X, tileCoords.Y + j].HasTile
                    && Main.tile[tileCoords.X, tileCoords.Y + j].TileType == ModContent.TileType<ForceGravityBlockTile>())
                {
                    int calcHeight = ForceGravityBlockTile.GetTileHeight(tileCoords.X, tileCoords.Y + j);
                    if (j > calcHeight)
                        break;
                    return 1;
                }
            }
            for (int j = 0; j < ForceAntiGravityBlockTile.TileHeightMax; j++)
            {
                if (WorldGen.InWorld(tileCoords.X, tileCoords.Y - j) && Main.tile[tileCoords.X, tileCoords.Y - j].HasTile
                    && Main.tile[tileCoords.X, tileCoords.Y - j].TileType == ModContent.TileType<ForceAntiGravityBlockTile>())
                {
                    int calcHeight = ForceAntiGravityBlockTile.GetTileHeight(tileCoords.X, tileCoords.Y - j);
                    if (j > calcHeight)
                        break;
                    return -1;
                }
            }
            return 0;
        }

        public static void SpreadCustomGrass(int i, int j, int dirt, int grass, int spread = 0, byte color = 0)
        {
            if (!WorldGen.InWorld(i, j, 6))
            {
                return;
            }
            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (WorldGen.genRand.NextBool(8))
                    {
                        if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == dirt)
                        {
                            if (GrowGrass(k, l, grass))
                                WorldGen.SquareTileFrame(k, l, resetFrame: true);
                            return;
                        }
                    }
                }
            }
        }
    }
}