using Aequus.Common.Tiles;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Items.Materials.PearlShards;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Tiles.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus {
    public partial class AequusTile : GlobalTile, IPostSetupContent, IAddRecipes {
        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

        public const int ShadowOrbDrops_Aequus = 5;

        public static readonly List<PearlsTile> PearlsToGenerate = new();

        public struct IndestructibleCircle {
            private Vector2 measurementCenterPoint;
            private Point centerPoint;
            public Point CenterPoint {
                get => centerPoint;
                set {
                    centerPoint = value;
                    measurementCenterPoint = value.ToVector2();
                }
            }
            public float tileRadius;

            public bool InPoint(int i, int j) {
                return Vector2.Distance(measurementCenterPoint, new Vector2(i, j)) < tileRadius;
            }
        }

        private static List<IndestructibleCircle> CheckCircles;
        public static List<IndestructibleCircle> Circles { get; private set; }

        public static Dictionary<Point, Func<Color>> PylonColors { get; private set; }

        internal static bool[] All;

        public override void Load() {
            Load_Drawing();
            Load_Veinminer();
            CheckCircles = new List<IndestructibleCircle>();
            Circles = new List<IndestructibleCircle>();
            PylonColors = new Dictionary<Point, Func<Color>>();
            LoadHooks();
        }

        #region Hooks
        private static void LoadHooks() {
            //On.Terraria.GameContent.Tile_Entities.TEDisplayDoll.Draw += TEDisplayDoll_Draw;
            Terraria.On_WorldGen.PlaceTile += WorldGen_PlaceTile;
            Terraria.On_WorldGen.UpdateWorld_OvergroundTile += WorldGen_UpdateWorld_OvergroundTile;
            Terraria.On_WorldGen.UpdateWorld_UndergroundTile += WorldGen_UpdateWorld_UndergroundTile;
            Terraria.On_WorldGen.QuickFindHome += WorldGen_QuickFindHome;
        }

        private static void TEDisplayDoll_Draw(Terraria.GameContent.Tile_Entities.On_TEDisplayDoll.orig_Draw orig, Terraria.GameContent.Tile_Entities.TEDisplayDoll self, int tileLeftX, int tileTopY) {
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

        private static bool WorldGen_PlaceTile(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style) {
            if (Type >= TileID.Count && TileLoader.GetTile(Type) is TileHooks.IOnPlaceTile onPlaceTile) {
                var val = onPlaceTile.OnPlaceTile(i, j, mute, forced, plr, style);
                if (val.HasValue)
                    return val.Value;
            }
            return orig(i, j, Type, mute, forced, plr, style);
        }

        private static void WorldGen_QuickFindHome(On_WorldGen.orig_QuickFindHome orig, int npc) {
            bool solid = Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()];
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
            orig(npc);
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = solid;
        }
        #endregion

        public void PostSetupContent(Aequus aequus) {
            All = new bool[TileLoader.TileCount];
            for (int i = 0; i < All.Length; i++) {
                All[i] = true;
            }
        }

        public void AddRecipes(Aequus aequus) {
            LoadPylonColorsSet();
        }

        public static void LoadPylonColorsSet() {
            if (Aequus.InfoLogs) {
                Aequus.Instance.Logger.Info("Loading pylons colors...");
            }
            var val = Aequus.GetContentFile("PylonColors");
            foreach (var modDict in val) {
                if (modDict.Key == "Vanilla") {
                    foreach (var pylonColor in modDict.Value) {
                        var clr = Helper.ReadColor(pylonColor.Value);
                        PylonColors[new Point(TileID.TeleportationPylon, int.Parse(pylonColor.Key))] = () => clr;
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod)) {
                    if (Aequus.InfoLogs) {
                        Aequus.Instance.Logger.Info($"Loading pylon colors for {modDict.Key}...");
                    }
                    foreach (var pylonColor in modDict.Value) {
                        int style = 0;
                        string pylonName = pylonColor.Key;
                        if (pylonName.Contains(';')) {
                            var split = pylonName.Split(';');
                            pylonName = split[0];
                            style = int.Parse(split[1]);
                        }
                        if (mod.TryFind<ModPylon>(pylonName, out var pylon)) {
                            if (Aequus.InfoLogs) {
                                Aequus.Instance.Logger.Info($"{pylonName}/{style}/{pylon.Type}: {pylonColor.Value}");
                            }
                            var clr = Helper.ReadColor(pylonColor.Value);
                            PylonColors[new Point(pylon.Type, style)] = () => clr;
                        }
                        else if (Aequus.InfoLogs) {
                            Aequus.Instance.Logger.Error($"Could not find {pylonName}...");
                        }
                    }
                }
            }

            PylonColors[new Point(TileID.TeleportationPylon, 8)] = () => Main.DiscoColor;
        }

        public override void Unload() {
            PearlsToGenerate.Clear();
            VeinmineCondition.Clear();
            CheckCircles?.Clear();
            CheckCircles = null;
            Circles?.Clear();
            Circles = null;
        }

        internal static void UpdateIndestructibles() {
            CheckCircles.Clear();
            CheckCircles.AddRange(Circles);
            Circles.Clear();
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged) {
            if (WorldGen.gen)
                return true;

            foreach (var c in CheckCircles) {
                if (c.InPoint(i, j))
                    return false;
            }
            foreach (var s in DemonSiegeSystem.ActiveSacrifices) {
                if (s.Value.ProtectedTiles().Contains(i, j))
                    return false;
            }
            return true;
        }

        public void PlayerTileKillEffects(int i, int j, int type) {
            var player = Main.player[Player.FindClosest(new(i * 16f, j * 16f), 16, 16)];
            if (player.dead || player.ghost) {
                return;
            }

            Vector2 tilePos = new(i * 16f, j * 16f);
            Vector2 tileCenter = new(i * 16f + 8f, j * 16f + 8f);
            float distanceSquared = player.DistanceSQ(new(i * 16f + 8f, j * 16f + 8f));
            var aequus = player.Aequus();
            if (TileID.Sets.Ore[type]) {
                if (distanceSquared < 10000 && Collision.CanHitLine(player.position, player.width, player.height, tilePos + Vector2.Normalize(player.Center - tileCenter) * 16f, 16, 16))
                    ProcVeinminer(i, j, type, player, aequus);
            }
        }

        private void CrimsonOrbDrops(int i, int j) {
            int c = OrbDrop();
            switch (c) {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CrimsonCandle>());
                    break;
            }
        }
        private void CorruptionOrbDrops(int i, int j) {
            int c = OrbDrop();
            switch (c) {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CorruptionCandle>());
                    break;
            }
        }
        private int OrbDrop() {
            return AequusWorld.shadowOrbsBrokenTotal < ShadowOrbDrops_Aequus ? AequusWorld.shadowOrbsBrokenTotal : WorldGen.genRand.Next(ShadowOrbDrops_Aequus);
        }
        public override void Drop(int i, int j, int type) {
            switch (type) {
                case TileID.Heart: {

                        if (Main.tile[i, j].TileFrameX != 0 && Main.tile[i, j].TileFrameY != 0) {
                            break;
                        }

                        if (WorldGen.genRand.NextBool(15)) {
                            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 32, 32), ModContent.ItemType<HealthCursor>());
                        }

                        break;
                    }

                case TileID.ShadowOrbs: {

                        if (Main.tile[i, j].TileFrameX % 36 != 0 && Main.tile[i, j].TileFrameY % 36 != 0) {
                            break;
                        }

                        if (Main.tile[i, j].TileFrameX < 36) {
                            CorruptionOrbDrops(i, j);
                        }
                        else {
                            CrimsonOrbDrops(i, j);
                        }
                        AequusWorld.shadowOrbsBrokenTotal++;

                        break;
                    }
            }
        }
    }
}