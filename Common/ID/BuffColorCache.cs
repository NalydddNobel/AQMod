using AQMod.Assets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace AQMod.Common.ID
{
    public static class BuffColorCache
    {
        private static Dictionary<ushort, Color> buffToColor;
        private static Dictionary<ushort, Color> itemToColor;
        private static Dictionary<ushort, ushort> itemToBuff;

        public static void RegisterColorItemBuffLink(int item, int buff, Color color)
        {
            RegisterColorItemBuffLink((ushort)item, (ushort)buff, color);
        }
        public static void RegisterColorItemBuffLink(ushort item, ushort buff, Color color)
        {
            if (buffToColor.ContainsKey(buff))
            {
                buffToColor[buff] = color;
            }
            else
            {
                buffToColor.Add(buff, color);
            }
            if (itemToColor.ContainsKey(item))
            {
                itemToColor[item] = color;
            }
            else
            {
                itemToColor.Add(item, color);   
            }
            if (itemToBuff.ContainsKey(item))
            {
                itemToBuff[item] = buff;
            }
            else
            {
                itemToBuff.Add(item, buff);   
            }
        }

        public static void RegisterBuffColor(int buff, Color color)
        {
            RegisterBuffColor((ushort)buff, color);
        }
        public static void RegisterBuffColor(ushort buff, Color color)
        {
            if (buffToColor.ContainsKey(buff))
            {
                buffToColor[buff] = color;
            }
            else
            {
                buffToColor.Add(buff, color);
            }
        }

        internal static void Init()
        {
            itemToColor = new Dictionary<ushort, Color>();
            buffToColor = new Dictionary<ushort, Color>();
            itemToBuff = new Dictionary<ushort, ushort>();

            RegisterColorItemBuffLink(ItemID.ObsidianSkinPotion, BuffID.ObsidianSkin, new Color(160, 60, 240, 255));
            RegisterColorItemBuffLink(ItemID.RegenerationPotion, BuffID.Regeneration, new Color(255, 100, 160, 255));
            RegisterColorItemBuffLink(ItemID.SwiftnessPotion, BuffID.Swiftness, new Color(160, 240, 100, 255));
            RegisterColorItemBuffLink(ItemID.GillsPotion, BuffID.Gills, new Color(100, 160, 230, 255));
            RegisterColorItemBuffLink(ItemID.IronskinPotion, BuffID.Ironskin, new Color(235, 255, 100, 255));
            RegisterColorItemBuffLink(ItemID.ManaRegenerationPotion, BuffID.ManaRegeneration, new Color(255, 80, 150, 255));
            RegisterColorItemBuffLink(ItemID.MagicPowerPotion, BuffID.MagicPower, new Color(79, 16, 164, 255));
            RegisterColorItemBuffLink(ItemID.SpelunkerPotion, BuffID.Spelunker, new Color(255, 245, 150, 255));
            RegisterColorItemBuffLink(ItemID.ThornsPotion, BuffID.Thorns, new Color(165, 255, 80, 255));

            RegisterBuffColor(BuffID.OnFire, new Color(255, 180, 100, 255));
            RegisterBuffColor(BuffID.Burning, new Color(255, 180, 100, 255));
            RegisterBuffColor(BuffID.Lovestruck, new Color(255, 180, 200, 255));
            RegisterBuffColor(BuffID.Midas, new Color(255, 180, 90, 255));
            RegisterBuffColor(BuffID.Slimed, new Color(180, 180, 255, 255));
            RegisterBuffColor(BuffID.Wet, new Color(180, 180, 255, 255));
            RegisterBuffColor(BuffID.Ichor, new Color(255, 255, 160, 255));
            RegisterBuffColor(BuffID.BetsysCurse, new Color(255, 220, 20, 255));
            RegisterBuffColor(BuffID.CursedInferno, new Color(160, 255, 160, 255));
            RegisterBuffColor(BuffID.ShadowFlame, new Color(200, 60, 255, 255));
        }

        internal static void Unload()
        {
            itemToColor = null;
        }

        public static Color GetColorFromItemID(int item)
        {
            if (itemToColor.TryGetValue((ushort)item, out var color2))
                return color2;
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
                itemToColor.Add((ushort)item, color);
                return color;
            }
            catch (Exception e)
            {
                var log = AQMod.GetInstance().Logger;
                log.Error("Error when trying to get a color from {ItemID:" + item + "}");
                log.Error(e);

                itemToColor.Add((ushort)item, defaultColor);
                return defaultColor;
            }
        }

        public static Color GetColorFromBuffID(int buff)
        {
            return GetColorFromBuffID(buff, Color.White);
        }
        public static Color GetColorFromBuffID(int buff, Color defaultColor)
        {
            if (buffToColor.TryGetValue((ushort)buff, out var color2))
                return color2;
            return defaultColor;
        }
    }
}