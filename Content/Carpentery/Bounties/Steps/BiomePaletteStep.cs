using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Content.Carpentery.Bounties.Steps
{
    public class BiomePaletteStep : Step
    {
        public class Interest : StepInterest
        {
            public Color comparisonColor;
            public Vector3 hslCompare;
            public HashSet<byte> allowedPaints;

            public Dictionary<Point, List<Point>> givenHouses;
            public Dictionary<ushort, Dictionary<Rectangle, Color>> colorTableCache;

            public override void CompileInterestingPoints(StepInfo info)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                var s = GameShaders.Hair.GetShaderFromItemId(ItemID.BiomeHairDye) as LegacyHairShaderData;
                var oldSceneMetrics = Main.SceneMetrics;

                var area = info.SamplingArea.Fluffize(10);

                var screenLoc = Main.screenPosition;
                var playerLoc = Main.LocalPlayer.position;
                Main.LocalPlayer.position = info.SamplingArea.Center().ToWorldCoordinates();
                Main.screenPosition = Main.LocalPlayer.position.Floor();

                allowedPaints = new HashSet<byte>();
                colorTableCache = new Dictionary<ushort, Dictionary<Rectangle, Color>>();
                try
                {
                    info.SwapWorldSample();

                    AequusWorld.TileCountsMultiplier = 5;
                    Main.SceneMetrics = new SceneMetrics();
                    Main.SceneMetrics.ScanAndExportToMain(new SceneMetricsScanSettings()
                    { BiomeScanCenterPositionInWorld = Main.LocalPlayer.position, VisualScanArea = null, ScanOreFinderData = false, });
                    Main.LocalPlayer.UpdateBiomes();
                    int oldBGStyle = Main.bgStyle;
                    Main.bgStyle = CaptureBiome.GetCaptureBiome(-1).BackgroundIndex;
                    Main.waterStyle = Main.CalculateWaterStyle(ignoreFountains: true);

                    info.SwapWorldSample();

                    comparisonColor = s.GetColor(Main.LocalPlayer, Color.White);
                    hslCompare = Main.rgbToHsl(comparisonColor);
                    for (byte i = 0; i < 32; i++)
                    {
                        var hsl = Main.rgbToHsl(WorldGen.paintColor(i));
                        if (hsl.X == 0f)
                            continue;
                        if ((hsl.X - hslCompare.X).Abs() <= 0.11f)
                        {
                            allowedPaints.Add(i);
                        }
                    }
                    Main.bgStyle = oldBGStyle;
                    Main.LocalPlayer.ForceUpdateBiomes();
                }
                catch
                {
                }

                Main.screenPosition = screenLoc;
                Main.LocalPlayer.position = playerLoc;
                Main.SceneMetrics = oldSceneMetrics;
            }
        }

        public float MinCredit;

        public BiomePaletteStep(float minCredit = 0.5f) : base()
        {
            MinCredit = minCredit;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new Interest());
        }

        public bool CheckHousing(TileMapCache map, List<Point> walls, Vector3 hslCompare, HashSet<byte> allowedPaints, Dictionary<ushort, Dictionary<Rectangle, Color>> colorLookups)
        {
            if (Main.netMode == NetmodeID.Server)
                return true;

            float credit = 0f;
            int tilesGivingCredit = 0;
            float wallCredit = 0f;
            int wallsGivingCredit = 0;
            var offsets = new Point[] { Point.Zero, new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
            foreach (var point in walls)
            {
                if (allowedPaints.Contains((byte)map[point].WallColor))
                {
                    wallCredit += 1f;
                    wallsGivingCredit++;
                }

                for (int i = 0; i < offsets.Length; i++)
                {
                    var p = point + offsets[i];
                    if (!map.InSceneRenderedMap(p))
                        continue;
                    if (map[p].IsFullySolid)
                    {
                        tilesGivingCredit++;
                        if (allowedPaints.Contains((byte)map[p].TileColor))
                        {
                            credit += 1f;
                            continue;
                        }
                        var frame = new Rectangle(map[p].TileFrameX, map[p].TileFrameY, 16, 16);
                        var color = ColorLookup(map[p].TileType, frame, colorLookups);

                        var hsl = Main.rgbToHsl(color);
                        if (hsl.X == 0f)
                            continue;
                        if ((hsl.X - hslCompare.X).Abs() <= 0.11f)
                        {
                            credit += 1f;
                            continue;
                        }
                    }
                }
            }
            if (tilesGivingCredit <= 10)
                return false;
            float wallCreditAdd = 0f;
            if (wallsGivingCredit > 0)
            {
                wallCreditAdd = wallCredit / wallCreditAdd;
            }
            return credit / tilesGivingCredit + wallCreditAdd / 4f > MinCredit;
        }

        public Color ColorLookup(ushort tileID, Rectangle frame, Dictionary<ushort, Dictionary<Rectangle, Color>> colorLookups)
        {
            var dominantColor = Color.White;
            if (Main.netMode == NetmodeID.Server)
                return dominantColor;

            if (colorLookups.TryGetValue(tileID, out var lookupInner))
            {
                if (lookupInner.TryGetValue(frame, out var lookupColor))
                {
                    dominantColor = lookupColor;
                    return dominantColor;
                }
            }
            try
            {
                Main.instance.LoadTiles(tileID);
                var texture = TextureAssets.Tile[tileID];
                if (texture == null || texture.Value == null || !texture.IsLoaded || texture.IsDisposed)
                    return dominantColor;

                var colorDictionary = new Dictionary<Color, int>();

                var colors = texture.Value.Get2DColorArr();
                for (int i = frame.X; i < frame.X + frame.Width; i++)
                {
                    for (int j = frame.Y; j < frame.Y + frame.Width; j++)
                    {
                        try
                        {
                            var clr = colors[i, j];
                            if (colorDictionary.ContainsKey(clr))
                            {
                                colorDictionary[clr]++;
                            }
                            else
                            {
                                colorDictionary.Add(clr, 1);
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                if (colorDictionary.Count == 0)
                    return dominantColor;

                foreach (var pair in colorDictionary)
                {
                    var hsl = Main.rgbToHsl(pair.Key);
                    foreach (var pair2 in colorDictionary)
                    {
                        if (pair2.Key != pair.Key)
                        {
                            var hsl2 = Main.rgbToHsl(pair2.Key);
                            if ((hsl.X - hsl2.X).Abs() < 0.1f && (hsl.Y - hsl2.Y).Abs() < 0.1f)
                            {
                                colorDictionary[pair2.Key] += colorDictionary[pair.Key];
                            }
                        }
                    }
                }

                int dominantColorAmt = 0;
                foreach (var pair in colorDictionary)
                {
                    if (pair.Value > dominantColorAmt)
                    {
                        dominantColorAmt = pair.Value;
                        dominantColor = pair.Key;
                    }
                }
            }
            catch
            {
            }
            return dominantColor;
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var interest = info.GetInterest<Interest>();
            interest.Update(info);
            var result = new StepResult("ImproperlyColoredHouses");
            foreach (var house in interest.givenHouses.Values)
            {
                if (CheckHousing(info.Map, house, interest.hslCompare, interest.allowedPaints, interest.colorTableCache))
                {
                    result.success = true;
                    break;
                }
            }
            return result;
        }
    }
}