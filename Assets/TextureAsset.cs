using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public struct TextureAsset : ITextureComponent
    {
        private readonly string _path;
        private const string throwbackTexturePath = "AQMod/" + AQTextures.Error;
        private static readonly bool _debug = false;

        private Ref<Texture2D> _texture;
        private bool _loaded;
        private bool _loadedProperly;

        public bool IsNull => _loaded;
        public string Path => _path;
        public bool Loaded => _loaded;
        public bool LoadedProperly => _loadedProperly;

        internal static TextureAsset FromT<T>(string extra)
        {
            return FromType(typeof(T), extra);
        }

        internal static TextureAsset FromType(Type t, string extra)
        {
            var textureAsset = new TextureAsset(t.Namespace.Replace(".", "/") + "/" + t.Name + extra);
            return textureAsset;
        }

        internal TextureAsset(string path)
        {
            _path = path;
            _loaded = false;
            _loadedProperly = false;
            _texture = null;
            if (_debug)
            {
                LoadValue(null);
            }
        }

        public void LoadValue(object context)
        {
            if (!_loaded)
            {
                _loadedProperly = ModContent.TextureExists(_path);
                _texture = new Ref<Texture2D>(_loadedProperly ? ModContent.GetTexture(_path) : ModContent.GetTexture(throwbackTexturePath));
                _loaded = true;
            }
        }

        public Ref<Texture2D> GetRef(object context)
        {
            return _texture;
        }
    }
}