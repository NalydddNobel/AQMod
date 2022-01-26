using AQMod.Assets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace AQMod.Content
{
    public static class StarbyteColorCache
    {
        private static Dictionary<ushort, Color> _colorsCache;

        internal static void Init()
        {
            _colorsCache = new Dictionary<ushort, Color>
            {
                [(ushort)ItemID.ObsidianSkinPotion] = new Color(90, 72, 168, 255),
                [(ushort)ItemID.RegenerationPotion] = new Color(255, 56, 162, 255),
                [(ushort)ItemID.SwiftnessPotion] = new Color(134, 240, 10, 255),
                [(ushort)ItemID.GillsPotion] = new Color(10, 119, 230, 255),
                [(ushort)ItemID.IronskinPotion] = new Color(230, 222, 10, 255),
                [(ushort)ItemID.ManaRegenerationPotion] = new Color(164, 16, 103, 255),
                [(ushort)ItemID.MagicPowerPotion] = new Color(79, 16, 164, 255),
                [(ushort)ItemID.SpelunkerPotion] = new Color(184, 143, 9, 255),
                [(ushort)ItemID.ThornsPotion] = new Color(134, 164, 16, 255),
            };
        }

        internal static void Unload()
        {
            _colorsCache = null;
        }

        public static Color GetColor(int item)
        {
            if (_colorsCache.TryGetValue((ushort)item, out var color2))
                return color2;
            switch (item)
            {
                default:
                    {
                        try
                        {
                            var texture = TextureGrabber.GetItem(item);
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
                                    if (clrs[index].A == 255)
                                        colors.Add(clrs[index]);
                                }
                            }
                            var mixedColor = new Color(255, 255, 255, 255);
                            var color = new Color(255, 255, 255, 255);
                            foreach (var c in colors)
                            {
                                mixedColor = Color.Lerp(color, c, 0.25f);
                            }
                            float minDarkness = mixedColor.ToVector3().Length();
                            foreach (var c in colors)
                            {
                                if (c.ToVector3().Length() >= minDarkness)
                                    color = Color.Lerp(color, c, 0.25f);
                            }
                            _colorsCache.Add((ushort)item, color);
                            return color;
                        }
                        catch (Exception e)
                        {
                            var aQMod = AQMod.GetInstance();
                            aQMod.Logger.Error(e.Message);
                            aQMod.Logger.Error(e.StackTrace);

                            var color = new Color(255, 255, 255, 255);
                            _colorsCache.Add((ushort)item, color);
                            return color;
                        }
                    }
            }
        }
    }
}
