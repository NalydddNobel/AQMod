using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public sealed class TextureAsset
    {
        private readonly string _path;
        private const string throwbackTexturePath = "AQMod/" + OldTextureCache.Error;
        private static readonly bool _debug = false;

        private Ref<Texture2D> _texture;
        private bool _loaded;
        private bool _loadedProperly;

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
            return textureAsset;
        }

        public void LoadAsset()
        {
            if (!_loaded)
            {
                _loadedProperly = ModContent.TextureExists(_path);
                _texture = new Ref<Texture2D>(_loadedProperly ? ModContent.GetTexture(_path) : ModContent.GetTexture(throwbackTexturePath));
                _loaded = true;
            }
        }

        public string Path()
        {
            return _path;
        }

        public Ref<Texture2D> GetAsset()
        {
            LoadAsset();
            return _texture;
        }
        public Texture2D GetValue()
        {
            return GetAsset().Value;
        }
        public bool Loaded()
        {
            return _loaded;
        }
        public bool LoadedProperly()
        {
            return _loadedProperly;
        }
    }
}