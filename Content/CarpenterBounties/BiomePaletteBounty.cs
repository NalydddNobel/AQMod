using Aequus.Common;
using Aequus.Items;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Content.CarpenterBounties
{
    public class BiomePaletteBounty : CarpenterBounty
    {
        public override bool CheckConditions(ConditionInfo info, out string message)
        {
            message = Language.GetTextValue(ReplyKey + ".NoHouses");
            var s = GameShaders.Hair.GetShaderFromItemId(ItemID.BiomeHairDye) as LegacyHairShaderData;
            var oldSceneMetrics = Main.SceneMetrics;

            var area = info.SamplingArea.Fluffize(10);

            var screenLoc = Main.screenPosition;
            var playerLoc = Main.LocalPlayer.position;
            Main.LocalPlayer.position = info.SamplingArea.Center().ToWorldCoordinates();
            Main.screenPosition = Main.LocalPlayer.position.Floor();

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

                var comparisonColor = s.GetColor(Main.LocalPlayer, Color.White);
                var hslCompare = Main.rgbToHsl(comparisonColor);
                var allowedPaints = new HashSet<byte>();
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

                var colorTable = new Dictionary<ushort, Dictionary<Rectangle, Color>>();

                var housingWalls = new Dictionary<Point, List<Point>>();

                for (int i = 0; i < info.Width; i++)
                {
                    for (int j = 0; j < info.Height; j++)
                    {
                        if (info[i, j].IsFullySolid || info[i, j].WallType == WallID.None || !Main.wallHouse[info[i, j].WallType] || info[i, j].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor))
                        {
                            continue;
                        }

                        foreach (var l in housingWalls.Values)
                        {
                            if (l.Contains(new Point(i, j)))
                                goto Continue;
                        }

                        var pendingList = CarpenterSystem.FindWallTiles(info.Map, i, j);
                        if (pendingList.Count < 30)
                            continue;
                        housingWalls.Add(new Point(i, j), pendingList);

                    Continue:
                        continue;
                    }
                }

                if (housingWalls.Count == 0)
                {
                    Main.screenPosition = screenLoc;
                    Main.LocalPlayer.position = playerLoc;
                    Main.SceneMetrics = oldSceneMetrics;
                    return true;
                }

                message = Language.GetTextValue(ReplyKey + ".ImproperlyColoredHouses");
                foreach (var house in housingWalls)
                {
                    if (CheckHousing(info.Map, house.Value, hslCompare, allowedPaints, colorTable))
                    {
                        int furnCount = CarpenterSystem.CountDecorInsideHouse(info.Map, house.Value, null);
                        if (furnCount < 5)
                        {
                            message = NotEnoughFurniture();
                            continue;
                        }
                        message = Language.GetTextValue(ReplyKey + ".Complete");
                        Main.screenPosition = screenLoc;
                        Main.LocalPlayer.position = playerLoc;
                        Main.SceneMetrics = oldSceneMetrics;
                        return true;
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
            return false;
        }

        public bool CheckHousing(TileMapCache map, List<Point> walls, Vector3 hslCompare, HashSet<byte> allowedPaints, Dictionary<ushort, Dictionary<Rectangle, Color>> colorLookups)
        {
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
            return credit / tilesGivingCredit + wallCreditAdd / 4f > 0.5f;
        }

        public Color ColorLookup(ushort tileID, Rectangle frame, Dictionary<ushort, Dictionary<Rectangle, Color>> colorLookups)
        {
            if (colorLookups.TryGetValue(tileID, out var lookupInner))
            {
                if (lookupInner.TryGetValue(frame, out var lookupColor))
                {
                    return lookupColor;
                }
            }

            Main.instance.LoadTiles(tileID);
            var texture = TextureAssets.Tile[tileID];
            if (texture == null || texture.Value == null || !texture.IsLoaded || texture.IsDisposed)
                return Color.White;

            var colorDictionary = new Dictionary<Color, int>();

            var colors = AequusHelpers.Get2DColorArr(texture.Value);
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
                return Color.White;

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

            var dominantColor = Color.White;
            int dominantColorAmt = 0;
            foreach (var pair in colorDictionary)
            {
                if (pair.Value > dominantColorAmt)
                {
                    dominantColorAmt = pair.Value;
                    dominantColor = pair.Key;
                }
            }
            return dominantColor;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults<OmniPaint>();
        }
    }
}
