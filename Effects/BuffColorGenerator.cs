using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects
{
    public static class BuffColorGenerator
    {
        public static Color GetColorFromItemID(int item)
        {
            if (AQMod.Sets.ItemToColor.TryGetValue(item, out var color))
                return color;
            return TryGetColorFromItemSprite(item);
        }

        private static Color TryGetColorFromItemSprite(int item)
        {
            return TryGetColorFromItemSprite(item, Color.White);
        }
        private static Color TryGetColorFromItemSprite(int item, Color defaultColor)
        {
            try
            {
                var texture = Main.itemTexture[item];
                var clrs = new Color[texture.Width * texture.Height];
                texture.GetData(clrs, 0, clrs.Length);
                int pixelPaddingX = 4;
                if (texture.Width <= 6)
                {
                    pixelPaddingX = 0;
                }
                else if (texture.Width <= 8)
                {
                    pixelPaddingX = 2;
                }
                int pixelPaddingY = 4;
                if (texture.Height <= 6)
                {
                    pixelPaddingY = 0;
                }
                else if (texture.Height <= 8)
                {
                    pixelPaddingY = 2;
                }
                int textureHeightHalf = texture.Height / 2;
                var frame = new Rectangle(pixelPaddingX, textureHeightHalf + pixelPaddingY, texture.Width - pixelPaddingX * 2, textureHeightHalf - pixelPaddingY);
                var colors = new List<Color>();
                for (int i = frame.X; i < frame.X + frame.Width; i++)
                {
                    for (int j = frame.Y; j < frame.Y + frame.Height; j++)
                    {
                        int index = j * texture.Width + i;
                        if (clrs[index].A == 255 && !AQMod.Sets.ItemColorBlacklist.Contains(clrs[index]))
                            colors.Add(clrs[index]);
                    }
                }
                var color = new Color(255, 255, 255, 255);

                color.R = (byte)AQUtils.Mean(colors.GetSpecific((r) => r.R));
                color.G = (byte)AQUtils.Mean(colors.GetSpecific((g) => g.G));
                color.B = (byte)AQUtils.Mean(colors.GetSpecific((b) => b.B));

                AQMod.Sets.ItemToColor.Add(item, color);
                return color;
            }
            catch (Exception e)
            {
                var log = AQMod.Instance.Logger;
                log.Error("Error when trying to get a color from {ItemID:" + item + "}");
                log.Error(e);

                AQMod.Sets.ItemToColor.Add(item, defaultColor);
                return defaultColor;
            }
        }

        public static Color GetColorFromBuffID(int buff)
        {
            return GetColorFromBuffID(buff, Color.White);
        }
        public static Color GetColorFromBuffID(int buff, Color defaultColor)
        {
            if (AQMod.Sets.BuffToColor.TryGetValue(buff, out var color2))
                return color2;
            return defaultColor;
        }
    }
}