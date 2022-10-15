using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class PaintsRenderer : ILoadable
    {
        public struct TextureVariantKey
        {
            public int TileType;
            public int TileStyle;
            public string Texture;
            public int PaintColor;

            public TextureVariantKey(string texture, int tileType, int tileStyle, int paintColor)
            {
                TileType = tileType;
                TileStyle = tileStyle;
                Texture = texture;
                PaintColor = paintColor;
            }

            public TextureVariantKey(int i, int j, string texture, int paintColor)
            {
                TileType = Main.tile[i, j].TileType;
                TileStyle = 0;
                Texture = texture;
                PaintColor = paintColor;
            }

            public TextureVariantKey(Point p, string texture, int paintColor) : this(p.X, p.Y, texture, paintColor)
            {
            }

            public bool Equals(TextureVariantKey other)
            {
                return Texture == other.Texture && PaintColor == other.PaintColor;
            }

            public override bool Equals(object obj)
            {
                if (obj is TextureVariantKey key)
                {
                    return Equals(key);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Texture.GetHashCode() + PaintColor;
            }

            public static bool operator ==(TextureVariantKey left, TextureVariantKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TextureVariantKey left, TextureVariantKey right)
            {
                return !left.Equals(right);
            }
        }

        public class TextureRenderTargetHolder : TilePaintSystemV2.ARenderTargetHolder
        {
            public TextureVariantKey Key;

            public override void Prepare()
            {
                PrepareTextureIfNecessary(ModContent.Request<Texture2D>(Key.Texture, AssetRequestMode.ImmediateLoad).Value);
            }

            public override void PrepareShader()
            {
                PrepareShader(Key.PaintColor, TreePaintSystemData.GetTileSettings(Key.TileType, Key.TileStyle));
            }
        }

        public static Dictionary<TextureVariantKey, TextureRenderTargetHolder> Renderers { get; private set; }
        public static List<TilePaintSystemV2.ARenderTargetHolder> Requests { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Renderers = new Dictionary<TextureVariantKey, TextureRenderTargetHolder>();
            Requests = new List<TilePaintSystemV2.ARenderTargetHolder>();
            On.Terraria.GameContent.TilePaintSystemV2.PrepareAllRequests += TilePaintSystemV2_PrepareAllRequests;
        }

        private void TilePaintSystemV2_PrepareAllRequests(On.Terraria.GameContent.TilePaintSystemV2.orig_PrepareAllRequests orig, TilePaintSystemV2 self)
        {
            orig(self);
            if (Requests.Count != 0)
            {
                for (int i = 0; i < Requests.Count; i++)
                {
                    Requests[i].Prepare();
                }
                Requests.Clear();
            }
        }

        void ILoadable.Unload()
        {
            if (Renderers != null)
            {
                try
                {
                    foreach (var value in Renderers.Values)
                    {
                        value.Clear();
                    }
                    Renderers.Clear();
                }
                catch
                {
                }
            }
            Renderers = null;
        }

        public static void Reset()
        {
            Main.QueueMainThreadAction(() =>
            {
                try
                {
                    foreach (var value in Renderers.Values)
                    {
                        value.Clear();
                    }
                    Renderers.Clear();
                }
                catch
                {
                }
            });
        }

        public static void RequestTile(ref TextureVariantKey lookupKey)
        {
            if (!Renderers.TryGetValue(lookupKey, out var value))
            {
                value = new TextureRenderTargetHolder
                {
                    Key = lookupKey
                };
                Renderers.Add(lookupKey, value);
            }
            if (!value.IsReady)
            {
                Requests.Add(value);
            }
        }

        public static Texture2D TryGetPaintedTexture(TextureVariantKey lookupKey)
        {
            if (Renderers.TryGetValue(lookupKey, out var value) && value.IsReady)
            {
                return value.Target;
            }
            RequestTile(ref lookupKey);
            return ModContent.Request<Texture2D>(lookupKey.Texture, AssetRequestMode.ImmediateLoad).Value;
        }
        public static Texture2D TryGetPaintedTexture(Tile tile, string texture)
        {
            if (tile.TileColor == 0)
                return ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad).Value;
            return TryGetPaintedTexture(new TextureVariantKey(texture, tile.TileType, 0, tile.TileColor));
        }
        public static Texture2D TryGetPaintedTexture(int i, int j, string texture)
        {
            return TryGetPaintedTexture(Main.tile[i, j], texture);
        }
        public static Texture2D TryGetPaintedTexture(Point p, string texture)
        {
            return TryGetPaintedTexture(p.X, p.Y, texture);
        }
    }
}
