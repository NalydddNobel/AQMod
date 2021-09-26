using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AQMod.Assets.Textures
{
    /// <summary>
    /// Texture Enum Array
    /// </summary>
    /// <typeparam name="TEnum">An enum</typeparam>
    public class TEA<TEnum>
    {
        public string FilePath { get; }

        public string FileNames { get; }

        private readonly Texture2D[] _textures;

        public TEA(TEnum count, string filePath, string fileNames)
        {
            _textures = new Texture2D[count.GetHashCode()];
            FilePath = filePath;
            FileNames = fileNames;
        }

        public Texture2D this[TEnum type]
        {
            get => GetAsset(type);
        }

        public virtual string GetTexturePath(int id)
        {
            return FilePath + "/" + FileNames + "_" + id;
        }

        public virtual Texture2D GetAsset(TEnum type)
        {
            int id = type.GetHashCode();
            if (_textures[id] == null)
            {
                string path = GetTexturePath(id);
                _textures[id] = ModContent.TextureExists(path) ? ModContent.GetTexture(path) : ModContent.GetTexture("AQMod/Textures/Assets/eggshells");
            }
            return _textures[id];
        }
    }
}