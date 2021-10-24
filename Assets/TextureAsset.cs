using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    [Obsolete("Replace with ref textures")]
    public sealed class TextureAsset : AssetItem<Texture2D>
    {
        private readonly string _path;
        private const string throwbackTexturePath = "AQMod/" + TextureCache.Error;
        private static readonly bool _debug = false;

        internal TextureAsset(string path)
        {
            _path = path;
            if (_debug)
            {
                LoadAsset();
            }
        }

        internal static TextureAsset FromT<T>(string extra)
        {
            return FromType(typeof(T), extra);
        }

        internal static TextureAsset FromType(Type t, string extra)
        {
            var textureAsset = new TextureAsset(t.Namespace.Replace(".", "/") + "/" + t.Name + extra);
            if (_debug)
                AQMod.Instance.Logger.Debug("Creating asset: " + textureAsset._path);
            return textureAsset;
        }

        public override void LoadAsset()
        {
            if (!_loaded)
            {
                _loadedProperly = ModContent.TextureExists(_path);
                _asset = new Ref<Texture2D>(_loadedProperly ? ModContent.GetTexture(_path) : ModContent.GetTexture(throwbackTexturePath));
                _loaded = true;
            }
        }

        public string Path()
        {
            return _path;
        }
    }
}