using Aequus.Items;
using Aequus.Items.Tools;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Content.CarpenterBounties
{
    public class UnderworldFountainBounty : FountainBounty
    {
        public override int LiquidWanted => LiquidID.Lava;

        public override bool CheckConditions(ConditionInfo info, out string message)
        {
            message = "";
            if (!FindWaterfallsPass(info.Map, out var waterfalls))
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NoWaterfalls");
                return false;
            }
            var surroundingRectangle = new Rectangle(info.Width, info.Height, 1, 1);
            foreach (var w in waterfalls)
            {
                if (surroundingRectangle.X > w.X)
                {
                    surroundingRectangle.X = w.X;
                }
                if (surroundingRectangle.Y > w.Y)
                {
                    surroundingRectangle.Y = w.Y;
                }
            }
            foreach (var w in waterfalls)
            {
                if (surroundingRectangle.X < w.X)
                {
                    int width = w.X - surroundingRectangle.X + 1;
                    if (width > surroundingRectangle.Width)
                        surroundingRectangle.Width = width;
                }
                if (surroundingRectangle.Y < w.Y)
                {
                    int height = w.Y - surroundingRectangle.Y + 1;
                    if (height > surroundingRectangle.Height)
                        surroundingRectangle.Height = height;
                }
            }
            if (surroundingRectangle.Height < 15)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.ShortWaterfalls");
                return false;
            }
            surroundingRectangle.X--;
            surroundingRectangle.Width += 2;
            surroundingRectangle.Y--;
            surroundingRectangle.Height += 2;

            FindCraftableTilesPass(info.Map, surroundingRectangle, out var tiles);

            if (tiles.Count == 0 || tiles.Count < 12)
            {
                message = Language.GetTextValue(LanguageKey + ".Reply.NoCraftedBlocks");
                return false;
            }

            surroundingRectangle = new Rectangle(info.Width, info.Height, 1, 1);
            foreach (var t in tiles)
            {
                if (surroundingRectangle.X > t.X)
                {
                    surroundingRectangle.X = t.X;
                }
                if (surroundingRectangle.Y > t.Y)
                {
                    surroundingRectangle.Y = t.Y;
                }
            }
            foreach (var t in tiles)
            {
                if (surroundingRectangle.X < t.X)
                {
                    int width = t.X - surroundingRectangle.X + 1;
                    if (width > surroundingRectangle.Width)
                        surroundingRectangle.Width = width;
                }
                if (surroundingRectangle.Y < t.Y)
                {
                    int height = t.Y - surroundingRectangle.Y + 1;
                    if (height > surroundingRectangle.Height)
                        surroundingRectangle.Height = height;
                }
            }

            int middleWidth = (int)Math.Round(surroundingRectangle.Width / 2f);
            for (int i = 0; i < middleWidth; i++)
            {
                for (int j = 0; j < surroundingRectangle.Height; j++)
                {
                    var point1 = new Point(surroundingRectangle.X + i, surroundingRectangle.Y + j);
                    var point2 = new Point(surroundingRectangle.X + surroundingRectangle.Width - 1 - i, surroundingRectangle.Y + j);
                    if (tiles.Contains(point1) || tiles.Contains(point2))
                    {
                        if (info[point1].TileType != info[point2].TileType)
                        {
                            var misc = info[point1].Misc;
                            misc.TileColor = PaintID.DeepRedPaint;
                            info[point1] = new TileDataCache(info[point1].Type, info[point1].Liquid, misc, info[point1].Wall, info[point1].Aequus);

                            misc = info[point2].Misc;
                            misc.TileColor = PaintID.DeepRedPaint;
                            info[point2] = new TileDataCache(info[point2].Type, info[point2].Liquid, misc, info[point2].Wall, info[point1].Aequus);

                            message = Language.GetTextValue(LanguageKey + ".Reply.Completed");
                            return true;
                        }
                    }
                }
            }

            message = Language.GetTextValue(LanguageKey + ".Reply.IsSymmetric");
            return false;
        }

        public override bool IsBountyAvailable()
        {
            return false;
        }

        public override Item ProvideBountyRewardItem()
        {
            return AequusItem.SetDefaults<AdvancedRuler>(); // Lava Resistant Gloves -- Allows you to place tiles in lava
        }
    }
}