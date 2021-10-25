using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    [Obsolete("Replace with Texture2D dictionaries")]
    /// <summary>
    /// Texture Enum Array
    /// </summary>
    /// <typeparam name="TEnum">An enum</typeparam>
    public class TEA<TEnum>
    {
        public string FilePath { get; }

        public string FileNames { get; }

        private readonly bool[] _loaded;
        private readonly Texture2D[] _textures;
        private const string callbackTexture = "AQMod/" + TextureCache.Error;

        public TEA(TEnum count, string filePath, string fileNames)
        {
            _loaded = new bool[count.GetHashCode()];
            _textures = new Texture2D[count.GetHashCode()];
            FilePath = filePath;
            FileNames = fileNames;
        }

        public Texture2D this[TEnum type]
        {
            get => GetAsset(type);
        }

        protected virtual string GetTexturePath(int id)
        {
            return FilePath + "/" + FileNames + "_" + id;
        }

        public virtual bool ContainsTexture(TEnum type, bool loadTexture)
        {
            if (loadTexture)
            {
                GetAsset(type);
            }
            return _loaded[type.GetHashCode()];
        }

        public virtual Texture2D GetAsset(TEnum type)
        {
            int id = type.GetHashCode();
            if (_textures[id] == null)
            {
                string path = GetTexturePath(id);
                _loaded[id] = ModContent.TextureExists(path);
                _textures[id] = _loaded[id] ? ModContent.GetTexture(path) : ModContent.GetTexture(callbackTexture);
            }
            return _textures[id];
        }
    }
}