using Aequus.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public class PotionColorsDatabase : ILoadable, IPostSetupContent, IAddRecipes
    {
        internal static List<Color> ItemColorForBuffsBlacklist;
        public static Dictionary<int, Color> ItemToBuffColor { get; private set; }
        public static Dictionary<int, Color> BuffToColor { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            ItemColorForBuffsBlacklist = new List<Color>()
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

            BuffToColor = new Dictionary<int, Color>();
            ItemToBuffColor = new Dictionary<int, Color>();
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Aequus.LogMore)
            {
                Aequus.Instance.Logger.Info("Loading potion colors...");
            }
            var val = Aequus.GetContentFile("PotionColors");
            foreach (var modDict in val)
            {
                if (modDict.Key == "Vanilla")
                {
                    foreach (var potionColor in modDict.Value)
                    {
                        if (BuffID.Search.TryGetId(potionColor.Key, out int buffID))
                        {
                            BuffToColor[buffID] = AequusHelpers.ReadColor(potionColor.Value);
                            continue;
                        }
                        aequus.Logger.Error($"Buff {potionColor.Key} does not exist.");
                    }
                }
                else if (ModLoader.TryGetMod(modDict.Key, out var mod))
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Info($"Loading custom wall to item ID table entries for {modDict.Key}...");
                    }
                    foreach (var potionColor in modDict.Value)
                    {
                        if (mod.TryFind<ModBuff>(potionColor.Key, out var modItem))
                        {
                            BuffToColor[modItem.Type] = AequusHelpers.ReadColor(potionColor.Value);
                        }
                    }
                }
            }
            foreach (var i in ContentSamples.ItemsByType)
            {
                if (i.Value.buffType > 0)
                {
                    foreach (var b in BuffToColor)
                    {
                        if (i.Value.buffType == b.Key)
                        {
                            ItemToBuffColor[i.Key] = b.Value;
                            break;
                        }
                    }
                }
            }
        }

        void ILoadable.Unload()
        {
            ItemToBuffColor?.Clear();
            ItemToBuffColor = null;
        }

        public static Color GetColorFromItemID(int item)
        {
            if (ItemToBuffColor.TryGetValue(item, out var color))
                return color;

            if (ContentSamples.ItemsByType[item].buffType > 0 && BuffToColor.TryGetValue(ContentSamples.ItemsByType[item].buffType, out var buffColor))
            {
                ItemToBuffColor[item] = buffColor;
                return buffColor;
            }

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
                        if (clrs[index].A == 255 && !ItemColorForBuffsBlacklist.Contains(clrs[index]))
                            colors.Add(clrs[index]);
                    }
                }
                var color = new Color(255, 255, 255, 255)
                {
                    R = (byte)colors.GetSpecific((r) => r.R).Mean(),
                    G = (byte)colors.GetSpecific((g) => g.G).Mean(),
                    B = (byte)colors.GetSpecific((b) => b.B).Mean()
                };

                ItemToBuffColor.Add(item, color);
                return color;
            }
            catch (Exception e)
            {
                var log = Aequus.Instance.Logger;
                log.Error("Error when trying to get a color from {ItemID:" + item + "}");
                log.Error(e);

                ItemToBuffColor.Add(item, defaultColor);
                return defaultColor;
            }
        }
    }
}