using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class BuffColorDatabase : ILoadable
    {
        internal static List<Color> ItemColorBlacklist;

        public static Dictionary<int, Color> ItemToColor { get; private set; }
        public static Dictionary<int, Color> BuffToColor { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            ItemColorBlacklist = new List<Color>()
            {
                new Color(0, 0, 0, 255),
                new Color(255, 255, 255, 255),
                new Color(49, 49, 59, 255),
                new Color(82, 83, 97, 255),
                new Color(121, 123, 142, 255),
                new Color(153, 156, 180, 255),
                new Color(186, 189, 218, 255),
                new Color(226, 229, 255, 255),
            };

            BuffToColor = new Dictionary<int, Color>()
            {
                [BuffID.ObsidianSkin] = new Color(160, 60, 240, 255),
                [BuffID.Regeneration] = new Color(255, 100, 160, 255),
                [BuffID.Swiftness] = new Color(160, 240, 100, 255),
                [BuffID.Gills] = new Color(100, 160, 230, 255),
                [BuffID.Ironskin] = new Color(235, 255, 100, 255),
                [BuffID.ManaRegeneration] = new Color(255, 80, 150, 255),
                [BuffID.MagicPower] = new Color(79, 16, 164, 255),
                [BuffID.Spelunker] = new Color(255, 245, 150, 255),
                [BuffID.Thorns] = new Color(165, 255, 80, 255),

                [BuffID.OnFire] = new Color(255, 180, 100, 255),
                [BuffID.Burning] = new Color(255, 180, 100, 255),
                [BuffID.Lovestruck] = new Color(255, 180, 200, 255),
                [BuffID.Midas] = new Color(255, 180, 90, 255),
                [BuffID.Slimed] = new Color(180, 180, 255, 255),
                [BuffID.Wet] = new Color(180, 180, 255, 255),
                [BuffID.Ichor] = new Color(255, 255, 160, 255),
                [BuffID.BetsysCurse] = new Color(255, 220, 20, 255),
                [BuffID.CursedInferno] = new Color(160, 255, 160, 255),
                [BuffID.ShadowFlame] = new Color(200, 60, 255, 255),
            };

            ItemToColor = new Dictionary<int, Color>()
            {
                [ItemID.ObsidianSkinPotion] = BuffToColor[BuffID.ObsidianSkin],
                [ItemID.RegenerationPotion] = BuffToColor[BuffID.Regeneration],
                [ItemID.SwiftnessPotion] = BuffToColor[BuffID.Swiftness],
                [ItemID.GillsPotion] = BuffToColor[BuffID.Gills],
                [ItemID.IronskinPotion] = BuffToColor[BuffID.Ironskin],
                [ItemID.ManaRegenerationPotion] = BuffToColor[BuffID.ManaRegeneration],
                [ItemID.MagicPowerPotion] = BuffToColor[BuffID.MagicPower],
                [ItemID.SpelunkerPotion] = BuffToColor[BuffID.Spelunker],
                [ItemID.ThornsPotion] = BuffToColor[BuffID.Thorns],
            };

            ItemToColor = new Dictionary<int, Color>();
        }

        void ILoadable.Unload()
        {
            ItemToColor?.Clear();
            ItemToColor = null;
        }

        public static Color GetColorFromItemID(int item)
        {
            if (ItemToColor.TryGetValue(item, out var color))
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
                var texture = TextureAssets.Item[item].Value;
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
                        if (clrs[index].A == 255 && !ItemColorBlacklist.Contains(clrs[index]))
                            colors.Add(clrs[index]);
                    }
                }
                var color = new Color(255, 255, 255, 255)
                {
                    R = (byte)AequusHelpers.Mean(colors.GetSpecific((r) => r.R)),
                    G = (byte)AequusHelpers.Mean(colors.GetSpecific((g) => g.G)),
                    B = (byte)AequusHelpers.Mean(colors.GetSpecific((b) => b.B))
                };

                ItemToColor.Add(item, color);
                return color;
            }
            catch (Exception e)
            {
                var log = Aequus.Instance.Logger;
                log.Error("Error when trying to get a color from {ItemID:" + item + "}");
                log.Error(e);

                ItemToColor.Add(item, defaultColor);
                return defaultColor;
            }
        }

        public static Color GetColorFromBuffID(int buff)
        {
            return GetColorFromBuffID(buff, Color.White);
        }
        public static Color GetColorFromBuffID(int buff, Color defaultColor)
        {
            if (BuffToColor.TryGetValue(buff, out var color2))
                return color2;
            return defaultColor;
        }
    }
}