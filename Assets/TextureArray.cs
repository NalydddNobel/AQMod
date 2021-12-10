using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public struct TextureArray : ITextureComponent
    {
        private readonly string _path;
        private const string throwbackTexturePath = "AQMod/" + AQTextures.Error;
        private static readonly bool _debug = false;

        private Ref<Texture2D>[] _textures;
        private bool[] _loaded;
        private bool[] _loadedProperly;

        public bool IsNull => false; 
        public string Path => _path;

        public bool Loaded(int index)
        {
            return _loaded[index];
        }

        public bool LoadedProperly(int index)
        {
            return _loadedProperly[index];
        }

        internal static TextureArray FromT<T>(int length, string extra = "")
        {
            return FromType(typeof(T), length, extra);
        }

        internal static TextureArray FromType(Type t, int length, string extra = "")
        {
            var textureAsset = new TextureArray(t.Namespace.Replace(".", "/") + "/" + t.Name + extra,length);
            return textureAsset;
        }

        internal TextureArray(string path, int length)
        {
            _path = path;
            _loaded = new bool[length];
            _loadedProperly = new bool[length];
            _textures = new Ref<Texture2D>[length];
            if (_debug)
            {
                LoadValue(null);
            }
        }

        private string getPath(object context)
        {
            return _path + context.ToString();
        }

        public void LoadValue(object context)
        {
            if (!_loaded[(int)context])
            {
                string path = getPath(context);
                _loadedProperly[(int)context] = ModContent.TextureExists(_path);
                _textures[(int)context] = new Ref<Texture2D>(_loadedProperly[(int)context] ? ModContent.GetTexture(_path) : ModContent.GetTexture(throwbackTexturePath));
                _loaded[(int)context] = true;
            }
        }

        public Ref<Texture2D> GetRef(object context)
        {
            return _textures[(int)context];
        }
    }
}